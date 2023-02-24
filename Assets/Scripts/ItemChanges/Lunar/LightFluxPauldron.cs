using RoR2;
using R2API;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Hex3Rebalance.Init;

namespace Hex3Rebalance.ItemChanges
{
    public static class LightFluxPauldron
    {
        private static float MoveSpeedStack;
        private static float CooldownReductionStack;
        public static void Init(string itemName, string itemTier)
        {
            if (Configs.LightFluxPauldron_Enable.Value)
            {
                Debug.Log(Main.ModName + ": (" + itemTier + ") " + itemName + "");
            }
            else
            {
                Debug.Log(Main.ModName + ": (" + itemTier + ") " + itemName + " disabled, cancelling changes");
                return;
            }
            MoveSpeedStack = Configs.LightFluxPauldron_MoveSpeedStack.Value / 100f;
            CooldownReductionStack = Configs.LightFluxPauldron_CooldownReductionStack.Value / 100f;

            AddLang();
            AddHooks();
        }

        private static void AddLang()
        {
            LanguageAPI.Add("ITEM_HALFATTACKSPEEDHALFCOOLDOWNS_PICKUP", "Increase your mobility, <style=cDeath>but greatly reduce your maximum health.</style>");
            LanguageAPI.Add("ITEM_HALFATTACKSPEEDHALFCOOLDOWNS_DESC", string.Format("Gain <style=cIsUtility>{0}%</style> <style=cStack>(+{0}% per stack)</style> <style=cIsUtility>movement speed</style> and reduce <style=cIsUtility>utility skill cooldown</style> by <style=cIsUtility>{1}%</style> <style=cStack>(-{1}% per stack)</style>. <style=cDeath>Your maximum health is reduced by {2}%</style>. <style=cStack>(-{2}% per stack)</style>", MoveSpeedStack * 100f, CooldownReductionStack * 100f, 50f));
        }

        private static void AddHooks()
        {
            // Apply curse penalty (Todo: Make curse fraction configurable... learn math)
            void GetStatCoefficients(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
            {
                if (body.inventory && body.inventory.GetItemCount(DLC1Content.Items.HalfAttackSpeedHalfCooldowns) > 0 && body.healthComponent)
                {
                    args.baseCurseAdd += Mathf.Pow(2f, (float)body.inventory.GetItemCount(DLC1Content.Items.HalfAttackSpeedHalfCooldowns)) - 1;
                }
            }
            // Render original light flux function useless
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor ilcursor = new ILCursor(il);
                if (ilcursor.TryGotoNext(
                    x => x.MatchLdarg(0),
                    x => x.MatchCall("RoR2.CharacterBody", "get_inventory"),
                    x => x.MatchLdsfld("RoR2.DLC1Content/Items", "HalfAttackSpeedHalfCooldowns")
                ))
                {
                    ilcursor.RemoveRange(4);
                    ilcursor.Emit(OpCodes.Ldc_I4, 0);
                }
                else
                {
                    Debug.LogError(Main.ModName + " Light Flux Pauldron CharacterBody.RecalculateStats hook failed.");
                }
            };

            // Add attack speed
            void GetStatCoefficients2(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
            {
                if (body.inventory && body.inventory.GetItemCount(DLC1Content.Items.HalfAttackSpeedHalfCooldowns) > 0)
                {
                    args.moveSpeedMultAdd += MoveSpeedStack * body.inventory.GetItemCount(DLC1Content.Items.HalfAttackSpeedHalfCooldowns);
                }
            }

            // Add cooldown reduction
            void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
            {
                orig(self);
                if (self.inventory && self.inventory.GetItemCount(DLC1Content.Items.HalfAttackSpeedHalfCooldowns) > 0 && self.skillLocator && self.skillLocator.GetSkill(SkillSlot.Utility))
                {
                    float cooldownCoeff = 1f;
                    GenericSkill skill = self.skillLocator.GetSkill(SkillSlot.Utility);
                    for (int i = 0; i < self.inventory.GetItemCount(DLC1Content.Items.HalfAttackSpeedHalfCooldowns); i++)
                    {
                        cooldownCoeff *= CooldownReductionStack;
                    }
                    skill.cooldownScale *= cooldownCoeff;
                }
            }

            RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients;
            RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients2;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }
    }
}
