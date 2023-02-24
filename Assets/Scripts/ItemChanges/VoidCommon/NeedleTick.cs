using RoR2;
using R2API;
using UnityEngine;
using MonoMod.Cil;
using Hex3Rebalance.Init;

namespace Hex3Rebalance.ItemChanges
{
    public static class NeedleTick
    {
        public static void Init(string itemName, string itemTier)
        {
            if (Configs.NeedleTick_Enable.Value)
            {
                Debug.Log(Main.ModName + ": (" + itemTier + ") " + itemName + "");
            }
            else
            {
                Debug.Log(Main.ModName + ": (" + itemTier + ") " + itemName + " disabled, cancelling changes");
                return;
            }

            AddLang();
            AddHooks();
        }

        private static void AddLang()
        {
            LanguageAPI.Add("ITEM_BLEEDONHITVOID_PICKUP", "Your first hit on an enemy inflicts Collapse, dealing a percentage of that hit's damage after a short delay.");
            LanguageAPI.Add("ITEM_BLEEDONHITVOID_DESC", string.Format("Your first hit on an enemy inflicts Collapse, which deals 100% (+50%) TOTAL damage after a short delay."));
        }

        private static void AddHooks()
        {
            void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
            {
                orig(self, damageInfo, victim);
            }

            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        }
    }
}
