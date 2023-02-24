using System;
using RoR2;
using R2API;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Hex3Rebalance.Init;

namespace Hex3Rebalance.ItemChanges
{
    public static class FocusedConvergence
    {
        private static float RadiusIncrease;
        private static float RadiusIncreaseStack;
        private static float SpeedIncrease;
        private static float SpeedIncreaseStack;
        private static float BackslideIncrease;
        private static float BackslideIncreaseStack;
        private static float ShrinkFactor;
        public static void Init(string itemName, string itemTier)
        {
            if (Configs.FocusedConvergence_Enable.Value)
            {
                Debug.Log(Main.ModName + ": (" + itemTier + ") " + itemName + "");
            }
            else
            {
                Debug.Log(Main.ModName + ": (" + itemTier + ") " + itemName + " disabled, cancelling changes");
                return;
            }

            RadiusIncrease = Configs.FocusedConvergence_RadiusIncrease.Value / 100f;
            RadiusIncreaseStack = Configs.FocusedConvergence_RadiusIncreaseStack.Value / 100f;
            SpeedIncrease = Configs.FocusedConvergence_SpeedIncrease.Value / 100f;
            SpeedIncreaseStack = Configs.FocusedConvergence_SpeedIncreaseStack.Value / 100f;
            BackslideIncrease = Configs.FocusedConvergence_BackslideIncrease.Value / 100f;
            BackslideIncreaseStack = Configs.FocusedConvergence_BackslideIncreaseStack.Value / 100f;
            ShrinkFactor = Configs.FocusedConvergence_ShrinkFactor.Value;

            AddLang();
            AddHooks();
        }

        private static void AddLang()
        {
            LanguageAPI.Add("ITEM_FOCUSEDCONVERGENCE_PICKUP", "Teleporter and holdout zones charge faster, <style=cDeath>but they gradually shrink, draining charge while you're outside of them.</style>");
            LanguageAPI.Add("ITEM_FOCUSEDCONVERGENCE_DESC", string.Format("Teleporters and holdout zones charge <style=cIsUtility>{0}%</style> <style=cStack>(+{1}% per stack)</style> faster, <style=cDeath>but their radius gradually shrinks over time.</style> Standing outside of holdout radius diminishes charge by <style=cDeath>{4}% per second</style> <style=cStack>(+{5}% per stack)</style>.", SpeedIncrease * 100f, SpeedIncreaseStack * 100f, RadiusIncrease * 100f, RadiusIncreaseStack * 100f, BackslideIncrease * 100f, BackslideIncreaseStack * 100f));
        }

        private static void AddHooks()
        {
            // Change holdout radius
            IL.RoR2.HoldoutZoneController.FocusConvergenceController.ApplyRadius += (il) =>
            {
                ILCursor ilcursor = new ILCursor(il);
                if (ilcursor.TryGotoNext(
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<HoldoutZoneController.FocusConvergenceController>("currentFocusConvergenceCount"),
                    x => x.MatchLdcI4(0)
                ))
                {
                    ilcursor.Index += 4;
                    ilcursor.RemoveRange(10);
                    ilcursor.Emit(OpCodes.Ldarg_1);
                    ilcursor.Emit(OpCodes.Ldarg_1);
                    ilcursor.Emit(OpCodes.Ldind_R4);
                    ilcursor.EmitDelegate<Func<float, float>>((radius) =>
                    {
                        radius *= 1f + RadiusIncrease + (RadiusIncreaseStack * ((float)Util.GetItemCountForTeam(TeamIndex.Player, RoR2Content.Items.FocusConvergence.itemIndex, true) - 1));
                        return radius;
                    });
                    ilcursor.Emit(OpCodes.Stind_R4);
                }
                else
                {
                    Debug.LogError(Main.ModName + " Focused Convergence HoldoutZoneController.FocusConvergenceController.ApplyRadius hook failed.");
                }
            };

            // Change holdout charge speed
            IL.RoR2.HoldoutZoneController.FocusConvergenceController.ApplyRate += (il) =>
            {
                ILCursor ilcursor = new ILCursor(il);
                if (ilcursor.TryGotoNext(
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<HoldoutZoneController.FocusConvergenceController>("currentFocusConvergenceCount"),
                    x => x.MatchLdcI4(0)
                ))
                {
                    ilcursor.Index += 4;
                    ilcursor.RemoveRange(12);
                    ilcursor.Emit(OpCodes.Ldarg_1);
                    ilcursor.Emit(OpCodes.Ldarg_1);
                    ilcursor.Emit(OpCodes.Ldind_R4);
                    ilcursor.EmitDelegate<Func<float, float>>((rate) =>
                    {
                        rate *= 1f + SpeedIncrease + (SpeedIncreaseStack * (Util.GetItemCountForTeam(TeamIndex.Player, RoR2Content.Items.FocusConvergence.itemIndex, true) - 1));
                        return rate;
                    });
                    ilcursor.Emit(OpCodes.Stind_R4);
                }
                else
                {
                    Debug.LogError(Main.ModName + " Focused Convergence HoldoutZoneController.FocusConvergenceController.ApplyRate hook failed.");
                }
            };

            // Apply backslide to holdout zones
            void HoldoutZoneController_OnEnable(On.RoR2.HoldoutZoneController.orig_OnEnable orig, HoldoutZoneController self)
            {
                // Soul Pillar values
                // self.chargeRadiusDelta = -11f;
                // self.dischargeRate = 0.1f;

                orig(self);
                if (Util.GetItemCountForTeam(TeamIndex.Player, RoR2Content.Items.FocusConvergence.itemIndex, true) > 0)
                {
                    self.chargeRadiusDelta = -(self.baseRadius * ShrinkFactor) * (1f + RadiusIncrease + (RadiusIncreaseStack * ((float)Util.GetItemCountForTeam(TeamIndex.Player, RoR2Content.Items.FocusConvergence.itemIndex, true) - 1)));
                    self.dischargeRate += BackslideIncrease + (BackslideIncreaseStack * (Util.GetItemCountForTeam(TeamIndex.Player, RoR2Content.Items.FocusConvergence.itemIndex, true) - 1));
                }
            }

            On.RoR2.HoldoutZoneController.OnEnable += HoldoutZoneController_OnEnable;
        }
    }
}
