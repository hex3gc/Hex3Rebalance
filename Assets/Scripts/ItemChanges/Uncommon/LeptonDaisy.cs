using RoR2;
using R2API;
using UnityEngine;
using MonoMod.Cil;
using Hex3Rebalance.Init;
using Hex3Rebalance.Utils;

namespace Hex3Rebalance.ItemChanges
{
    public static class LeptonDaisy
    {
        private static float WeakDuration;
        public static void Init()
        {
            if (!ItemDebugLog.PrintItemChange(Configs.LeptonDaisy_Enable.Value, "Uncommon", "Lepton Daisy"))
            {
                return;
            }
            WeakDuration = Configs.LeptonDaisy_WeakDuration.Value;

            AddLang();
            AddHooks();
        }

        private static void AddLang()
        {
            LanguageAPI.Add("ITEM_TPHEALINGNOVA_PICKUP", "Periodically release a nova during the Teleporter event which heals you and weakens enemies.");
            LanguageAPI.Add("ITEM_TPHEALINGNOVA_DESC", string.Format("Release a <style=cIsHealing>healing nova</style> during the Teleporter event, <style=cIsHealing>healing</style> allies for <style=cIsHealing>50%</style> of their maximum health and <style=cIsDamage>weakening</style> enemies for <style=cIsDamage>{0}</style> seconds. Occurs <style=cIsHealing>1</style> <style=cStack>(+1 per stack)</style> times.", WeakDuration));
        }

        private static void AddHooks()
        {
            // Add weaken debuff to valid targets
            void HealPulse_HealTarget(On.EntityStates.TeleporterHealNovaController.TeleporterHealNovaPulse.HealPulse.orig_HealTarget orig, object self, HealthComponent target)
            {
                if (target.body && target.body.teamComponent)
                {
                    TeamIndex teamIndex = target.body.teamComponent.teamIndex;
                    if (teamIndex == TeamIndex.Monster || teamIndex == TeamIndex.Lunar || teamIndex == TeamIndex.Void)
                    {
                        target.body.AddTimedBuff(RoR2Content.Buffs.Weak, WeakDuration);
                    }
                    if (teamIndex == TeamIndex.Player)
                    {
                        orig(self, target);
                    }
                }
            }

            // Remove team mask to allow enemy targeting
            IL.EntityStates.TeleporterHealNovaController.TeleporterHealNovaPulse.HealPulse.Update += (il) =>
            {
                ILCursor ilcursor = new ILCursor(il);
                if (ilcursor.TryGotoNext(
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<EntityStates.TeleporterHealNovaController.TeleporterHealNovaPulse.HealPulse>("teamMask"),
                    x => x.MatchCallvirt<SphereSearch>("FilterCandidatesByHurtBoxTeam")
                ))
                {
                    ilcursor.RemoveRange(3);
                }
                else
                {
                    Debug.LogError(Main.ModName + " Lepton Daisy TeleporterHealNovaPulse.HealPulse.Update hook failed.");
                }
            };

            On.EntityStates.TeleporterHealNovaController.TeleporterHealNovaPulse.HealPulse.HealTarget += HealPulse_HealTarget;
        }
    }
}
