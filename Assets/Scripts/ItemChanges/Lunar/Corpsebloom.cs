using System;
using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Hex3Rebalance.Init;
using Hex3Rebalance.Utils;

namespace Hex3Rebalance.ItemChanges
{
    public static class Corpsebloom
    {
        private static float ExplosionRange;
        private static float ExplosionRangeStack;
        private static float AntiRegenStack;
        private static float PercentHealthThreshold;
        private static GameObject corpseExplosionPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>("Assets/AssetBundle/Prefabs/Corpsebloom/CorpseExplosionPrefab.prefab");
        private static Material corpseExplosionMaterial = Assets.mainAssetBundle.LoadAsset<Material>("Assets/AssetBundle/Prefabs/Corpsebloom/CorpseExplosionMaterial.mat");
        private static GameObject crocoSplashPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoDiseaseImpactEffect.prefab").WaitForCompletion();
        public static void Init()
        {
            if (Main.debugMode)
            {
                corpseExplosionPrefab.GetComponentInChildren<Renderer>().gameObject.AddComponent<MaterialControllerComponents.HGControllerFinder>();
            }
            if (!ItemDebugLog.PrintItemChange(Configs.Corpsebloom_Enable.Value, "Lunar", "Corpsebloom"))
            {
                return;
            }
            ExplosionRange = Configs.Corpsebloom_ExplosionRange.Value;
            ExplosionRangeStack = Configs.Corpsebloom_ExplosionRangeStack.Value;
            AntiRegenStack = Configs.Corpsebloom_AntiRegenStack.Value;
            PercentHealthThreshold = Configs.Corpsebloom_PercentHealthThreshold.Value / 100f;

            AddLang();
            AddHooks();
        }

        private static void AddLang()
        {
            LanguageAPI.Add("ITEM_REPEATHEAL_PICKUP", "Passive health regeneration is reversed, building up into a poisonous blast.");
            LanguageAPI.Add("ITEM_REPEATHEAL_DESC", string.Format("<style=cDeath>All health regeneration is inverted</style>, slowly <style=cDeath>draining your health</style> over time <style=cStack>(+{2}% drain speed per stack)</style>. Every time you’re drained of <style=cDeath>{3}%</style> of your <style=cIsHealing>max health</style>, emit a <style=cIsHealing>poisoning wave</style> within a <style=cIsHealing>{0}m</style> <style=cStack>(+{1}m per stack)</style> radius.", ExplosionRange, ExplosionRangeStack, AntiRegenStack * 100f, PercentHealthThreshold * 100f));
        }

        private static void AddHooks()
        {
            // Manage item behavior
            void GetStatCoefficients(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
            {
                if (body.inventory && body.inventory.GetItemCount(RoR2Content.Items.RepeatHeal) > 0 && body.healthComponent)
                {
                    if (!body.GetComponent<CorpsebloomBehavior>())
                    {
                        body.AddItemBehavior<CorpsebloomBehavior>(1);
                        body.GetComponent<CorpsebloomBehavior>().percentHealthThreshold = PercentHealthThreshold;
                    }
                    CorpsebloomBehavior behavior = body.GetComponent<CorpsebloomBehavior>();
                    behavior.stack = body.inventory.GetItemCount(RoR2Content.Items.RepeatHeal);
                    if (body.healthComponent.health <= 5f) // Disable negative regen at low health to prevent lethal
                    {
                        args.regenMultAdd -= 1;
                        body.GetComponent<CorpsebloomBehavior>().noRegen = true;
                    }
                    else
                    {
                        args.regenMultAdd -= 1 + (1 + (AntiRegenStack * (body.inventory.GetItemCount(RoR2Content.Items.RepeatHeal) - 1)));
                        body.GetComponent<CorpsebloomBehavior>().noRegen = false;
                    }
                    behavior.currentRegen = body.regen;
                }
                if (body.inventory && body.inventory.GetItemCount(RoR2Content.Items.RepeatHeal) <= 0 && body.GetComponent<CorpsebloomBehavior>())
                {
                    UnityEngine.Object.Destroy(body.GetComponent<CorpsebloomBehavior>());
                }
            }
            // Render original corpsebloom function useless
            IL.RoR2.HealthComponent.OnInventoryChanged += (il) =>
            {
                ILCursor ilcursor = new ILCursor(il);
                if (ilcursor.TryGotoNext(
                    x => x.MatchLdfld("RoR2.HealthComponent/ItemCounts", "repeatHeal"),
                    x => x.MatchLdcI4(0),
                    x => x.MatchCgtUn()
                ))
                {
                    ilcursor.Index += 9;
                    ilcursor.Remove();
                    ilcursor.Emit(OpCodes.Ldc_I4_0);
                }
                else
                {
                    Debug.LogError(Main.ModName + " Corpsebloom CharacterBody.RecalculateStats hook failed.");
                }
            };
            RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients;
        }

        public class CorpsebloomBehavior : CharacterBody.ItemBehavior
        {
            public float currentRegen = 0f;
            public float regenBuildup = 0f;
            public float percentHealthThreshold = PercentHealthThreshold;
            public float regenTimer = 0f;
            public float dirtCooldown = 1f;
            public bool dirted = false;
            public bool noRegen = false;
            public TemporaryOverlay temporaryOverlay;

            void FixedUpdate()
            {
                regenTimer += Time.fixedDeltaTime;
                dirtCooldown += Time.fixedDeltaTime;
                if (body.healthComponent.health <= 5f && dirtCooldown > 1f)
                {
                    // Marking all stats dirty at low hp (should) ensure that the negative regen will stop in time before it becomes lethal
                    body.MarkAllStatsDirty();
                    dirtCooldown = 0f;
                    dirted = true;
                }
                if (body.healthComponent.health > 5f && dirted)
                {
                    // Reduces lag between exiting low health and resuming negative regen
                    body.MarkAllStatsDirty();
                    dirted = false;
                }
                // ^ Seems messy but it'll do

                // Regen timer is 1 second to stay in time with actual regen
                if (regenTimer >= 1f && noRegen == false)
                {
                    if (regenBuildup / (body.healthComponent.fullCombinedHealth * percentHealthThreshold) > 0.05f)
                    { 
                        // Update player poison overlay
                        if (!temporaryOverlay)
                        {
                            temporaryOverlay = body.gameObject.AddComponent<TemporaryOverlay>();
                            temporaryOverlay.duration = 99f;
                            temporaryOverlay.animateShaderAlpha = true;
                            temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, regenBuildup / (body.healthComponent.fullCombinedHealth * percentHealthThreshold), 99f, regenBuildup / (body.healthComponent.fullCombinedHealth * percentHealthThreshold));
                            temporaryOverlay.destroyComponentOnEnd = true;
                            temporaryOverlay.originalMaterial = corpseExplosionMaterial;
                            temporaryOverlay.AddToCharacerModel(body.gameObject.GetComponent<ModelLocator>().modelTransform.GetComponentInParent<CharacterModel>());
                        }
                        else
                        {
                            temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, regenBuildup / (body.healthComponent.fullCombinedHealth * percentHealthThreshold), 99f, regenBuildup / (body.healthComponent.fullCombinedHealth * percentHealthThreshold));
                        }
                    }
                    float currentRegenPositive = Math.Abs(currentRegen);
                    regenBuildup += currentRegenPositive;
                    regenTimer = 0f;
                }
                // Explode and reset
                if (regenBuildup >= body.healthComponent.fullCombinedHealth * percentHealthThreshold)
                {
                    Explode();
                    if (temporaryOverlay)
                    {
                        Destroy(temporaryOverlay);
                    }
                    regenBuildup = 0f;
                }
            }

            void Explode()
            {
                GameObject corpseExplosion = UnityEngine.Object.Instantiate(corpseExplosionPrefab);
                float explodeRadius = ExplosionRange + (ExplosionRangeStack * (stack - 1));
                corpseExplosion.GetComponent<ObjectScaleCurve>().baseScale *= explodeRadius * 2;
                corpseExplosion.transform.position = body.corePosition;
                corpseExplosion.transform.rotation = UnityEngine.Random.rotation;
                Util.PlaySound("Play_acrid_shift_land", corpseExplosion);

                EffectManager.SpawnEffect(crocoSplashPrefab, new EffectData
                {
                    origin = body.corePosition,
                    rotation = Quaternion.identity
                }, true);

                BlastAttack blastAttack = new BlastAttack
                {
                    position = body.corePosition,
                    baseDamage = 0f,
                    baseForce = 0f,
                    radius = explodeRadius,
                    attacker = body.gameObject,
                    inflictor = null,
                    teamIndex = TeamComponent.GetObjectTeam(body.gameObject),
                    crit = false,
                    procChainMask = default,
                    procCoefficient = 0f,
                    damageColorIndex = DamageColorIndex.Poison,
                    falloffModel = BlastAttack.FalloffModel.None,
                    damageType = DamageType.AOE,
                    attackerFiltering = AttackerFiltering.NeverHitSelf
                };
                BlastAttack.Result affectedEnemies = blastAttack.Fire();
                foreach (BlastAttack.HitPoint hitpoint in affectedEnemies.hitPoints)
                {
                    if (hitpoint.hurtBox && hitpoint.hurtBox.healthComponent && hitpoint.hurtBox.healthComponent.body)
                    {
                        DotController.InflictDot(hitpoint.hurtBox.healthComponent.gameObject, body.gameObject, DotController.DotIndex.Poison, 10f, 1f);
                    }
                }
            }

            void OnDestroy()
            {
                if (temporaryOverlay)
                {
                    Destroy(temporaryOverlay);
                }
            }
        }
    }
}
