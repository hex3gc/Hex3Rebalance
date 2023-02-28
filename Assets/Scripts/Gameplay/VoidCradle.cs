using System.Collections.Generic;
using Hex3Rebalance.Init;
using R2API;
using RoR2;
using RoR2.EntityLogic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Hex3Rebalance.Modules
{
    public static class VoidCradle
    {
        private static float Duration;
        private static GameObject voidFogPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VoidFogMildEffect.prefab").WaitForCompletion();
        public static BuffDef cradleAntiHeal;
        public static void Init()
        {
            if (Configs.VoidCradles.Value)
            {
                Debug.Log(Main.ModName + " Void Cradles");
            }
            else
            {
                Debug.Log(Main.ModName + " Void Cradle changes disabled");
                return;
            }
            Duration = Configs.VoidCradles_Duration.Value;

            AddBuffs();
            AddHooks();
        }

        public static void AddBuffs()
        {
            cradleAntiHeal = ScriptableObject.CreateInstance<BuffDef>();
            cradleAntiHeal.buffColor = new Color(1f, 1f, 1f);
            cradleAntiHeal.canStack = false;
            cradleAntiHeal.isDebuff = true;
            cradleAntiHeal.name = "Void Cradle Anti-heal";
            cradleAntiHeal.isHidden = true;
            ContentAddition.AddBuffDef(cradleAntiHeal);
        }

        private static void AddHooks()
        {
            // Keeps track of all anti-heal VFX in play
            Dictionary<GameObject, CharacterBody> vfxInstances = new Dictionary<GameObject, CharacterBody>();

            // Apply anti-heal debuff and create vfx on interaction
            void PurchaseInteraction_OnInteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
            {
                if (self.contextToken == "VOID_CHEST_CONTEXT" && self.available && self.CanBeAffordedByInteractor(activator))
                {
                    // Remove expired vfx instances from the dictionary
                    List<GameObject> keysToRemove = new List<GameObject>();
                    foreach (GameObject key in vfxInstances.Keys)
                    {
                        if (!key)
                        {
                            keysToRemove.Add(key);
                        }
                        if (key && key.GetComponent<Timer>().stopwatch >= Duration + 2f)
                        {
                            keysToRemove.Add(key);
                        }
                    }
                    foreach (GameObject key in keysToRemove)
                    {
                        vfxInstances.Remove(key);
                    }

                    CharacterBody body = activator.gameObject.GetComponent<CharacterBody>();
                    if (body)
                    {
                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(voidFogPrefab, body.corePosition, Quaternion.identity);
                        TemporaryVisualEffect tempEffect = gameObject.GetComponent<TemporaryVisualEffect>();
                        tempEffect.parentTransform = body.coreTransform;
                        tempEffect.healthComponent = body.healthComponent;
                        tempEffect.radius = body.radius;
                        tempEffect.visualState = TemporaryVisualEffect.VisualState.Enter;
                        Timer timer = gameObject.AddComponent<Timer>();

                        vfxInstances.Add(gameObject, body);

                        LocalCameraEffect cameraEffect = gameObject.GetComponent<LocalCameraEffect>();
                        if (cameraEffect)
                        {
                            cameraEffect.targetCharacter = body.gameObject;
                        }

                        body.AddTimedBuff(cradleAntiHeal, Duration);
                    }
                }
                orig(self, activator);
            }

            // Cancel vfx on buff loss
            void GetStatCoefficients(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
            {
                if (!body.HasBuff(cradleAntiHeal) && vfxInstances.ContainsValue(body))
                {
                    foreach (GameObject key in vfxInstances.Keys)
                    {
                        if (vfxInstances.TryGetValue(key, out CharacterBody value) && value == body)
                        {
                            TemporaryVisualEffect tempEffect = key.GetComponent<TemporaryVisualEffect>();
                            tempEffect.visualState = TemporaryVisualEffect.VisualState.Exit;
                        }
                    }
                }
            }

            // Block all healing during duration
            float HealthComponent_Heal(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask procChainMask, bool nonRegen)
            {
                if (self.body && self.body.HasBuff(cradleAntiHeal))
                {
                    return 0f;
                }
                else
                {
                    return orig(self, amount, procChainMask, nonRegen);
                }
            }

            On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteraction_OnInteractionBegin;
            RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients;
            On.RoR2.HealthComponent.Heal += HealthComponent_Heal;
        }
    }
}
