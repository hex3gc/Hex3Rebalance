using RoR2;
using R2API;
using UnityEngine;
using MonoMod.Cil;
using Hex3Rebalance.Init;
using Mono.Cecil.Cil;
using UnityEngine.AddressableAssets;
using Hex3Rebalance.Utils;

namespace Hex3Rebalance.ItemChanges
{
    public static class HuntersHarpoon
    {
        private static float SpeedPerKill;
        private static int SpeedCap;
        private static int SpeedCapStack;
        private static int MountainShrineStack;
        private static int MountainShrineAdditionalStack;
        private static Sprite harpoonBuffSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/MoveSpeedOnKill/texBuffKillMoveSpeed.tif").WaitForCompletion();
        private static GameObject harpoonEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/MoveSpeedOnKill/MoveSpeedOnKillActivate.prefab").WaitForCompletion();
        private static BuffDef harpoonStacksBuff;
        private static ItemDef harpoonHidden;
        public static void Init()
        {
            if (!ItemDebugLog.PrintItemChange(Configs.HuntersHarpoon_Enable.Value, "Uncommon", "Hunters Harpoon"))
            {
                return;
            }
            SpeedPerKill = Configs.HuntersHarpoon_SpeedPerKill.Value;
            SpeedCap = Configs.HuntersHarpoon_SpeedCap.Value;
            SpeedCapStack = Configs.HuntersHarpoon_SpeedCapStack.Value;
            MountainShrineStack = Configs.HuntersHarpoon_MountainShrineStack.Value;
            MountainShrineAdditionalStack = Configs.HuntersHarpoon_MountainShrineAdditionalStack.Value;

            AddItems();
            AddBuffs();
            AddLang();
            AddHooks();
        }

        public static void AddBuffs()
        {
            harpoonStacksBuff = ScriptableObject.CreateInstance<BuffDef>();
            harpoonStacksBuff.buffColor = new Color(1f, 1f, 1f);
            harpoonStacksBuff.canStack = true;
            harpoonStacksBuff.isDebuff = false;
            harpoonStacksBuff.name = "Hunter's Harpoon Speed";
            harpoonStacksBuff.isHidden = false;
            harpoonStacksBuff.iconSprite = harpoonBuffSprite;
            ContentAddition.AddBuffDef(harpoonStacksBuff);
        }

        private static void AddItems()
        {
            // Hidden item to keep track of harpoon speed stacks
            harpoonHidden = ScriptableObject.CreateInstance<ItemDef>();
            harpoonHidden.name = "HarpoonHidden";
            harpoonHidden.nameToken = "H3R_HARPOONHIDDEN_NAME";
            harpoonHidden.pickupToken = "H3R_HARPOONHIDDEN_PICKUP";
            harpoonHidden.descriptionToken = "H3R_HARPOONHIDDEN_DESC";
            harpoonHidden.loreToken = "H3R_HARPOONHIDDEN_LORE";
            harpoonHidden.tags = new ItemTag[] { ItemTag.CannotCopy, ItemTag.CannotSteal, ItemTag.CannotDuplicate };
            harpoonHidden.deprecatedTier = ItemTier.NoTier;
            harpoonHidden.canRemove = false;
            harpoonHidden.hidden = true;
            ItemAPI.Add(new CustomItem(harpoonHidden, new ItemDisplayRuleDict()));
        }

        private static void AddLang()
        {
            LanguageAPI.Add("ITEM_MOVESPEEDONKILL_PICKUP", "Killing boss enemies grants permanent movement speed. <style=cShrine>The Challenge Of The Mountain calls...</style>");
            LanguageAPI.Add("ITEM_MOVESPEEDONKILL_DESC", string.Format("Killing a boss enemy <style=cIsUtility>permanently increases your movement speed</style> by <style=cIsUtility>{0}%</style> up to a maximum of <style=cIsUtility>{1}%</style> <style=cStack>(+{2}% per stack)</style>. <style=cShrine>Mountain shrines gain {3} stacks</style> <style=cStack>(+{4} per stack)</style> <style=cShrine>of effectiveness.</style>", SpeedPerKill, SpeedPerKill * SpeedCap, SpeedPerKill * SpeedCapStack, MountainShrineStack, MountainShrineAdditionalStack));
        }

        private static void AddHooks()
        {
            // Bypass Hunter's Harpoon function
            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor ilcursor = new ILCursor(il);
                if (ilcursor.TryGotoNext(
                    x => x.MatchLdloc(17),
                    x => x.MatchLdsfld("RoR2.DLC1Content/Items", "MoveSpeedOnKill")
                ))
                {
                    ilcursor.Index += 5;
                    ilcursor.Remove();
                    ilcursor.Emit(OpCodes.Ldc_I4, 30000);
                }
                else
                {
                    Debug.LogError(Main.ModName + " Hunters Harpoon GlobalEventManager.OnCharacterDeath hook failed.");
                }
            };

            // Grant buff stacks on kill
            void DeathRewards_OnKilledServer(On.RoR2.DeathRewards.orig_OnKilledServer orig, DeathRewards self, DamageReport damageReport)
            {
                if (damageReport.attacker && self.characterBody && self.characterBody.isBoss)
                {
                    CharacterBody attackerBody = damageReport.attacker.GetComponent<CharacterBody>();
                    if (attackerBody && attackerBody.inventory)
                    {
                        int itemCount = attackerBody.inventory.GetItemCount(DLC1Content.Items.MoveSpeedOnKill);
                        if (itemCount > 0 && attackerBody.inventory.GetItemCount(harpoonHidden) < (SpeedCap + (SpeedCapStack * (itemCount - 1))))
                        {
                            // Spawn effect
                            EffectData effectData = new EffectData();
                            effectData.origin = attackerBody.corePosition;
                            bool flag = false;
                            if (attackerBody.characterMotor)
                            {
                                Vector3 moveDirection = attackerBody.characterMotor.moveDirection;
                                if (moveDirection != Vector3.zero)
                                {
                                    effectData.rotation = Util.QuaternionSafeLookRotation(moveDirection);
                                    flag = true;
                                }
                            }
                            if (!flag)
                            {
                                effectData.rotation = attackerBody.transform.rotation;
                            }
                            EffectManager.SpawnEffect(harpoonEffectPrefab, effectData, true);

                            // Give hidden item stack
                            attackerBody.inventory.GiveItem(harpoonHidden);
                        }
                    }
                }
                orig(self, damageReport);
            }

            // Apply extra mountain shrine stacks
            void TeleporterInteraction_AddShrineStack(On.RoR2.TeleporterInteraction.orig_AddShrineStack orig, TeleporterInteraction self)
            {
                orig(self);
                if (Util.GetItemCountGlobal(DLC1Content.Items.MoveSpeedOnKill.itemIndex, true) > 0)
                {
                    for (int i = 0; i < MountainShrineStack + ((Util.GetItemCountGlobal(DLC1Content.Items.MoveSpeedOnKill.itemIndex, true) - 1) * MountainShrineAdditionalStack); i++)
                    {
                        orig(self);
                    }
                }
            }

            // Apply buff stats and visual stacks
            void GetStatCoefficients(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
            {
                if (body.inventory)
                {
                    int hiddenItemCount = body.inventory.GetItemCount(harpoonHidden);
                    if (hiddenItemCount > 0)
                    {
                        args.moveSpeedMultAdd += (SpeedPerKill / 100f) * hiddenItemCount;
                        body.SetBuffCount(harpoonStacksBuff.buffIndex, hiddenItemCount);
                    }
                }
            }

            On.RoR2.DeathRewards.OnKilledServer += DeathRewards_OnKilledServer;
            On.RoR2.TeleporterInteraction.AddShrineStack += TeleporterInteraction_AddShrineStack;
            RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients;
        }
    }
}
