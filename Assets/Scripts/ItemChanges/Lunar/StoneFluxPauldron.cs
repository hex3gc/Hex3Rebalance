using RoR2;
using R2API;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Hex3Rebalance.Init;
using Hex3Rebalance.Utils;

namespace Hex3Rebalance.ItemChanges
{
    public static class StoneFluxPauldron
    {
        private static float HealthIncrease;
        private static float HealthIncreaseStack;
        private static float RegenAdd;
        private static float RegenAddStack;
        private static float HealingReduce;
        private static float HealingReduceStack;
        public static void Init()
        {
            if (!ItemDebugLog.PrintItemChange(Configs.StoneFluxPauldron_Enable.Value, "Lunar", "Stone Flux Pauldron"))
            {
                return;
            }
            HealthIncrease = Configs.StoneFluxPauldron_HealthIncrease.Value / 100f;
            HealthIncreaseStack = Configs.StoneFluxPauldron_HealthIncreaseStack.Value / 100f;
            RegenAdd = Configs.StoneFluxPauldron_RegenAdd.Value;
            RegenAddStack = Configs.StoneFluxPauldron_RegenAddStack.Value;
            HealingReduce = Configs.StoneFluxPauldron_HealingReduce.Value / 100f;
            HealingReduceStack = Configs.StoneFluxPauldron_HealingReduceStack.Value / 100f;

            AddLang();
            AddHooks();
        }

        private static void AddLang()
        {
            LanguageAPI.Add("ITEM_HALFSPEEDDOUBLEHEALTH_PICKUP", "Massively increase your maximum HP and regeneration, <style=cDeath>but greatly reduce your healing.</style>");
            LanguageAPI.Add("ITEM_HALFSPEEDDOUBLEHEALTH_DESC", string.Format("Increase your <style=cIsHealing>maximum health</style> by <style=cIsHealing>{0}%</style> <style=cStack>(+{1}% per stack)</style> and <style=cIsHealing>enhance regeneration</style> by <style=cIsHealing>{2} HP/s</style> <style=cStack>(+{3} per stack)</style>. <style=cDeath>All of your healing is reduced by {4}%</style> <style=cStack>(-{5}% per stack)</style><style=cDeath>.</style>", HealthIncrease * 100f, HealthIncreaseStack * 100f, RegenAdd, RegenAddStack, HealingReduce * 100f, HealingReduceStack * 100f));
        }

        private static void AddHooks()
        {
            // Render original stone flux function useless
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor ilcursor = new ILCursor(il);
                if (ilcursor.TryGotoNext(
                    x => x.MatchLdarg(0),
                    x => x.MatchCall("RoR2.CharacterBody", "get_inventory"),
                    x => x.MatchLdsfld("RoR2.DLC1Content/Items", "HalfSpeedDoubleHealth")
                ))
                {
                    ilcursor.RemoveRange(4);
                    ilcursor.Emit(OpCodes.Ldc_I4, 0);
                }
                else
                {
                    Debug.LogError(Main.ModName + " Stone Flux Pauldron CharacterBody.RecalculateStats hook failed.");
                }
            };

            // Alter stats
            void GetStatCoefficients(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
            {
                if (body.inventory)
                {
                    int itemCount = body.inventory.GetItemCount(DLC1Content.Items.HalfSpeedDoubleHealth);
                    if (itemCount > 0)
                    {
                        args.healthMultAdd += HealthIncrease + (HealthIncreaseStack * (itemCount - 1));
                        args.baseRegenAdd += RegenAdd + (RegenAddStack * (itemCount - 1));
                    }
                }
            }

            // Reduce healing on item owners
            float HealthComponent_Heal(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask procChainMask, bool nonRegen)
            {
                if (self.body && self.body.inventory)
                {
                    int itemCount = self.body.inventory.GetItemCount(DLC1Content.Items.HalfSpeedDoubleHealth);
                    if (itemCount > 0 && nonRegen)
                    {
                        float healingLeftFraction = 1f - HealingReduce;
                        for (int i = 0; i < itemCount - 1; i++)
                        {
                            healingLeftFraction -= healingLeftFraction * HealingReduceStack;
                        }
                        amount *= healingLeftFraction;
                    }
                }
                return orig(self, amount, procChainMask, nonRegen);
            }

            RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients;
            On.RoR2.HealthComponent.Heal += HealthComponent_Heal;
        }
    }
}
