using System;
using RoR2;
using R2API;
using UnityEngine;
using MonoMod.Cil;
using Hex3Rebalance.Init;
using Hex3Rebalance.Utils;

namespace Hex3Rebalance.ItemChanges
{
    public static class StunGrenade
    {
        private static float ExplosionChance;
        private static float ExplosionRadius;
        private static float ExplosionRadiusStack;
        private static float ExplosionDamage;
        private static GameObject stunExplosionPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>("Assets/AssetBundle/Prefabs/StunGrenade/StunExplosionPrefab.prefab");
        public static void Init(string itemName, string itemTier)
        {
            if (Main.debugMode)
            {
                stunExplosionPrefab.GetComponentInChildren<Renderer>().gameObject.AddComponent<MaterialControllerComponents.HGControllerFinder>();
            }
            if (Configs.StunGrenade_Enable.Value)
            {
                Debug.Log(Main.ModName + ": (" + itemTier + ") " + itemName + "");
            }
            else
            {
                Debug.Log(Main.ModName + ": (" + itemTier + ") " + itemName + " disabled, cancelling changes");
                return;
            }
            ExplosionChance = Configs.StunGrenade_ExplosionChance.Value;
            ExplosionRadius = Configs.StunGrenade_ExplosionRadius.Value;
            ExplosionRadiusStack = Configs.StunGrenade_ExplosionRadiusStack.Value;
            ExplosionDamage = Configs.StunGrenade_ExplosionDamage.Value;

            AddLang();
            AddHooks();
        }

        private static void AddLang()
        {
            LanguageAPI.Add("ITEM_STUNCHANCEONHIT_PICKUP", "Chance to create a stunning explosion on hit.");
            LanguageAPI.Add("ITEM_STUNCHANCEONHIT_DESC", string.Format("<style=cIsDamage>{0}%</style> <style=cStack>(+{0}% per stack)</style> chance on hit to create an <style=cIsDamage>explosion</style> for <style=cIsDamage>{3}% base damage</style>, which stuns enemies in a <style=cIsUtility>{1}m</style> radius <style=cStack>(+{2}m per stack)</style>.", ExplosionChance, ExplosionRadius, ExplosionRadiusStack, ExplosionDamage));
        }

        private static void AddHooks()
        {
            // Delete existing stun grenade behavior
            IL.RoR2.SetStateOnHurt.OnTakeDamageServer += (il) =>
            {
                ILCursor ilcursor = new ILCursor(il);
                if (ilcursor.TryGotoNext(
                    x => x.MatchLdstr("Prefabs/Effects/ImpactEffects/ImpactStunGrenade")
                ))
                {
                    ilcursor.RemoveRange(12);
                }
                else
                {
                    Debug.LogError(Main.ModName + "Stun Grenade IL.RoR2.SetStateOnHurt.OnTakeDamageServer hook failed.");
                }
            };

            // Add substitute behavior
            void CharacterBody_OnTakeDamageServer(On.RoR2.CharacterBody.orig_OnTakeDamageServer orig, CharacterBody self, DamageReport damageReport)
            {
                orig(self, damageReport);
                if (damageReport.attackerMaster && damageReport.attackerMaster.inventory && damageReport.attackerBody)
                {
                    int itemCount = damageReport.attackerMaster.inventory.GetItemCount(RoR2Content.Items.StunChanceOnHit);
                    if (itemCount > 0 && Util.CheckRoll(Util.ConvertAmplificationPercentageIntoReductionPercentage(ExplosionChance * (float)itemCount * damageReport.damageInfo.procCoefficient), damageReport.attackerMaster))
                    {
                        DamageInfo damageInfo = damageReport.damageInfo;
                        GameObject stunExplosion = UnityEngine.Object.Instantiate(stunExplosionPrefab);
                        float stunRadius = ExplosionRadius + (ExplosionRadiusStack * (itemCount - 1));
                        stunExplosion.GetComponent<ObjectScaleCurve>().baseScale *= stunRadius * 2;
                        stunExplosion.transform.position = damageReport.damageInfo.position;
                        stunExplosion.transform.rotation = UnityEngine.Random.rotation;
                        Util.PlaySound("Play_commando_M2_grenade_explo", stunExplosion);

                        EffectData effectData = new EffectData
                        {
                            origin = damageInfo.position
                        };
                        EffectManager.SpawnEffect(HealthComponent.AssetReferences.crowbarImpactEffectPrefab, effectData, true);

                        BlastAttack blastAttack = new BlastAttack
                        {
                            position = damageInfo.position,
                            baseDamage = damageReport.attackerBody.baseDamage * (ExplosionDamage / 100f),
                            baseForce = 0f,
                            radius = stunRadius,
                            attacker = damageReport.attacker,
                            inflictor = null,
                            teamIndex = TeamComponent.GetObjectTeam(damageReport.attacker),
                            crit = damageInfo.crit,
                            procChainMask = default,
                            procCoefficient = 0f,
                            damageColorIndex = DamageColorIndex.Item,
                            falloffModel = BlastAttack.FalloffModel.None,
                            damageType = DamageType.AOE,
                            attackerFiltering = AttackerFiltering.NeverHitSelf
                        };
                        BlastAttack.Result affectedEnemies = blastAttack.Fire();
                        foreach (BlastAttack.HitPoint hitpoint in affectedEnemies.hitPoints)
                        {
                            if (hitpoint.hurtBox && hitpoint.hurtBox.healthComponent && hitpoint.hurtBox.healthComponent.body)
                            {
                                if (hitpoint.hurtBox.healthComponent.gameObject.TryGetComponent(out SetStateOnHurt hurtState))
                                {
                                    float radiusSquare = (float)Math.Pow(stunRadius, 2);
                                    float stunDurationProportion = hitpoint.distanceSqr / radiusSquare;
                                    if (stunDurationProportion < 1)
                                    {
                                        hurtState.SetStun(1f - stunDurationProportion);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            On.RoR2.CharacterBody.OnTakeDamageServer += CharacterBody_OnTakeDamageServer;
        }
    }
}
