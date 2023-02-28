using RoR2;
using R2API;
using R2API.Networking;
using UnityEngine;
using MonoMod.Cil;
using Hex3Rebalance.Init;
using Hex3Rebalance.Modules;
using Hex3Rebalance.Utils;

namespace Hex3Rebalance.ItemChanges
{
    public static class NeedleTick
    {
        private static float Damage;
        private static float DamageStack;
        public static void Init()
        {
            if (!ItemDebugLog.PrintItemChange(Configs.FocusedConvergence_Enable.Value, "Void Common", "NeedleTick"))
            {
                return;
            }
            Damage = Configs.NeedleTick_Damage.Value / 100f;
            DamageStack = Configs.NeedleTick_DamageStack.Value / 100f;

            AddLang();
            AddHooks();
        }

        private static void AddLang()
        {
            LanguageAPI.Add("ITEM_BLEEDONHITVOID_PICKUP", "Your first hit on an enemy inflicts Collapse, dealing a percentage of that hit's damage after a short delay. <style=cIsVoid>Corrupts all Tri-Tip Daggers.</style>");
            LanguageAPI.Add("ITEM_BLEEDONHITVOID_DESC", string.Format("Your first hit on an enemy inflicts <style=cIsDamage>Collapse</style>, which deals <style=cIsDamage>{0}%</style> <style=cStack>(+{1}% per stack)</style> TOTAL damage after a short delay. <style=cIsVoid>Corrupts all Tri-Tip Daggers.</style>", Damage * 100f, DamageStack * 100f));
        }

        private static void AddHooks()
        {
            // Remove the existing needletick effect completely
            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                ILCursor ilcursor = new ILCursor(il);
                if (ilcursor.TryGotoNext(
                    x => x.MatchLdarg(1),
                    x => x.MatchLdfld<DamageInfo>("procChainMask"),
                    x => x.MatchStloc(25)
                ))
                {
                    ilcursor.RemoveRange(18);
                }
                else
                {
                    Debug.LogError(Main.ModName + " Needletick RoR2.GlobalEventManager.OnHitEnemy hook failed.");
                }
            };

            // Add collapse on first hit
            void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
            {
                orig(self, damageInfo, victim);
                if (damageInfo.damage > 0f && !damageInfo.rejected && damageInfo.attacker && damageInfo.attacker != victim && victim.GetComponent<CharacterBody>() && damageInfo.attacker.GetComponent<CharacterBody>() && FirstHit.InflictFirstHit(damageInfo.attacker, victim))
                {
                    CharacterBody victimBody = victim.GetComponent<CharacterBody>();
                    CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                    if (victimBody.healthComponent && attackerBody.inventory && attackerBody.inventory.GetItemCount(DLC1Content.Items.BleedOnHitVoid) > 0)
                    {
                        victimBody.healthComponent.ApplyDot(damageInfo.attacker, DotController.DotIndex.Fracture, 3f, 0.25f *  ((Damage + (DamageStack * (attackerBody.inventory.GetItemCount(DLC1Content.Items.BleedOnHitVoid) - 1))) * (damageInfo.damage / attackerBody.baseDamage)));
                    }
                }
            }

            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        }
    }
}
