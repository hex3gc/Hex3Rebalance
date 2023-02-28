using RoR2;
using R2API;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Hex3Rebalance.Init;
using Hex3Rebalance.Utils;

namespace Hex3Rebalance.ItemChanges
{
    internal static class BustlingFungus
    {
        private static float Radius;
        private static float RadiusStack;
        private static float HealFraction;
        private static float HealFractionStack;
        private static float Interval;
        public static bool WindDown;
        internal static void Init()
        {
            if (!ItemDebugLog.PrintItemChange(Configs.BustlingFungus_Enable.Value, "Common", "Bustling Fungus"))
            {
                return;
            }
            Radius = Configs.BustlingFungus_Radius.Value;
            RadiusStack = Configs.BustlingFungus_RadiusStack.Value;
            HealFraction = Configs.BustlingFungus_HealFraction.Value / 100f;
            HealFractionStack = Configs.BustlingFungus_HealFractionStack.Value / 100f;
            Interval = Configs.BustlingFungus_Interval.Value;
            WindDown = Configs.BustlingFungus_WindDown.Value;

            AddLang();
            AddHooks();
        }

        private static void AddLang()
        {
            LanguageAPI.Add("ITEM_MUSHROOM_PICKUP", "Periodically leave behind a fungal healing zone.");
            LanguageAPI.Add("ITEM_MUSHROOM_DESC", string.Format("Every <style=cIsUtility>10</style> seconds, leave behind a <style=cIsHealing>fungal zone</style> that heals <style=cStack>3%</style> <style=cIsHealing>(+2% per stack)</style> max health every second to all allies within <style=cIsHealing>4m</style> <style=cStack>(+2m per stack)</style>."));
        }

        private static void AddHooks()
        {
            // Remove vanilla Bustling Fungus function by making it return immediately
            IL.RoR2.Items.MushroomBodyBehavior.FixedUpdate += (il) =>
            {
                ILCursor ilcursor = new ILCursor(il);
                if (ilcursor.TryGotoNext(
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<RoR2.Items.BaseItemBodyBehavior>("stack"),
                    x => x.MatchStloc(0),
                    x => x.MatchLdloc(0)
                ))
                {
                    ilcursor.Index -= 3;
                    ilcursor.Emit(OpCodes.Ret);
                }
                else
                {
                    Debug.LogError(Main.ModName + " Bustling Fungus Items.MushroomBodyBehavior.FixedUpdate hook failed.");
                }
            };

            // Create zones
            void MushroomBodyBehavior_FixedUpdate(On.RoR2.Items.MushroomBodyBehavior.orig_FixedUpdate orig, RoR2.Items.MushroomBodyBehavior self)
            {
                orig(self);

                // Add MushroomTimer to keep track of refreshes
                if (!self.body.GetComponent<MushroomTimer>())
                {
                    self.body.AddItemBehavior<MushroomTimer>(self.stack);
                }
                if (!UnityEngine.Networking.NetworkServer.active)
                {
                    return;
                }

                MushroomTimer mushroomTimer = self.body.GetComponent<MushroomTimer>();

                mushroomTimer.interval += Time.fixedDeltaTime;
                if (WindDown && mushroomTimer.interval >= Interval - 0.4f && self.mushroomHealingWard)
                {
                    // Expiration animation/shrink
                    self.mushroomHealingWard.Networkradius = 0f;
                }
                if (mushroomTimer.interval >= Interval)
                {
                    // Destroy existing mushroom ward
                    UnityEngine.Object.Destroy(self.mushroomWardGameObject);

                    // Create a new one
                    self.mushroomWardGameObject = UnityEngine.Object.Instantiate<GameObject>(RoR2.Items.MushroomBodyBehavior.mushroomWardPrefab, self.body.footPosition, Quaternion.identity);
                    self.mushroomWardTeamFilter = self.mushroomWardGameObject.GetComponent<TeamFilter>();
                    self.mushroomWardTeamFilter.teamIndex = self.body.teamComponent.teamIndex;
                    self.mushroomHealingWard = self.mushroomWardGameObject.GetComponent<HealingWard>();
                    self.mushroomHealingWard.interval = 0.25f;
                    self.mushroomHealingWard.healFraction = (HealFraction + (HealFractionStack * (float)(self.stack - 1))) * self.mushroomHealingWard.interval;
                    self.mushroomHealingWard.healPoints = 0f;
                    self.mushroomHealingWard.Networkradius = self.body.radius + Radius + (RadiusStack * (float)self.stack);
                    UnityEngine.Networking.NetworkServer.Spawn(self.mushroomWardGameObject);
                    mushroomTimer.interval = 0f;
                }
            }

            On.RoR2.Items.MushroomBodyBehavior.FixedUpdate += MushroomBodyBehavior_FixedUpdate;
        }

        internal class MushroomTimer : CharacterBody.ItemBehavior
        {
            internal float interval = 0f;
        }
    }
}
