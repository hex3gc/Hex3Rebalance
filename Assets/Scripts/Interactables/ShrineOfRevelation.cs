using Hex3Rebalance.Init;
using Hex3Rebalance.Utils;
using R2API;
using RoR2;
using RoR2.Networking;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Hex3Rebalance.Interactables
{
    public static class ShrineOfRevelation
    {
        private static int MaxUses;
        private static int UseCostMultiplier;
        private static GameObject shrineRevelationWhitePrefab = Assets.mainAssetBundle.LoadAsset<GameObject>("Assets/AssetBundle/Prefabs/ShrineOfRevelation/revelationPrefab.prefab");
        private static GameObject shrineRevelationGreenPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>("Assets/AssetBundle/Prefabs/ShrineOfRevelation/revelationPrefabGreen.prefab");
        private static GameObject shrineRevelationRedPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>("Assets/AssetBundle/Prefabs/ShrineOfRevelation/revelationPrefabRed.prefab");

        public static void Init()
        {
            if (Main.debugMode)
            {
                foreach (Renderer renderer in shrineRevelationWhitePrefab.GetComponentsInChildren<Renderer>())
                {
                    renderer.gameObject.AddComponent<MaterialControllerComponents.HGControllerFinder>();
                }
            }

            if (Configs.ShrineOfRevelationWhite_Enable.Value)
            {
                CreateShrine(shrineRevelationWhitePrefab, "shrineOfRevelationWhite", Configs.ShrineOfRevelationWhite_AppearOnStages.Value, Configs.ShrineOfRevelationWhite_DirectorCost.Value, Configs.ShrineOfRevelationWhite_DirectorWeight.Value);
            }
            if (Configs.ShrineOfRevelationGreen_Enable.Value)
            {
                CreateShrine(shrineRevelationGreenPrefab, "shrineOfRevelationGreen", Configs.ShrineOfRevelationGreen_AppearOnStages.Value, Configs.ShrineOfRevelationGreen_DirectorCost.Value, Configs.ShrineOfRevelationGreen_DirectorWeight.Value);
            }
            if (Configs.ShrineOfRevelationRed_Enable.Value)
            {
                CreateShrine(shrineRevelationRedPrefab, "shrineOfRevelationRed", Configs.ShrineOfRevelationRed_AppearOnStages.Value, Configs.ShrineOfRevelationRed_DirectorCost.Value, Configs.ShrineOfRevelationRed_DirectorWeight.Value);
            }

            AddHooks();

            LanguageAPI.Add("SHRINE_REVELATION_NAME", "Shrine Of Revelation");
            LanguageAPI.Add("SHRINE_REVELATION_CONTEXT", "Revamp");
        }

        private static InteractableSpawnCard CreateShrine(GameObject shrinePrefab, string shrineName, string stagesToInclude, int cost, int weight)
        {
            Debug.Log(Main.ModName + ": Creating shrine " + shrineName);

            InteractableSpawnCard interactableSpawnCard = ScriptableObject.CreateInstance<InteractableSpawnCard>();
            interactableSpawnCard.name = shrineName;
            interactableSpawnCard.prefab = shrinePrefab;
            interactableSpawnCard.sendOverNetwork = true;
            interactableSpawnCard.hullSize = HullClassification.Golem;
            interactableSpawnCard.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            interactableSpawnCard.requiredFlags = RoR2.Navigation.NodeFlags.None;
            interactableSpawnCard.forbiddenFlags = RoR2.Navigation.NodeFlags.NoShrineSpawn;
            interactableSpawnCard.directorCreditCost = cost;
            interactableSpawnCard.occupyPosition = true;
            interactableSpawnCard.orientToFloor = false;
            interactableSpawnCard.skipSpawnWhenSacrificeArtifactEnabled = false;

            DirectorCard directorCard = new DirectorCard
            {
                selectionWeight = weight,
                spawnCard = interactableSpawnCard,
            };

            foreach (DirectorAPI.Stage stage in GetStages(stagesToInclude))
            {
                DirectorAPI.Helpers.AddNewInteractableToStage(directorCard, DirectorAPI.InteractableCategory.Shrines, stage);
            }

            return interactableSpawnCard;
        }

        private static List<DirectorAPI.Stage> GetStages(string stagesToInclude)
        {
            List<DirectorAPI.Stage> stageList = new List<DirectorAPI.Stage>();
            // S1
            if (stagesToInclude.Contains("1"))
            {
                stageList.Add(DirectorAPI.Stage.DistantRoost);
                stageList.Add(DirectorAPI.Stage.SiphonedForest);
                stageList.Add(DirectorAPI.Stage.TitanicPlains);
            }
            // S2
            if (stagesToInclude.Contains("2"))
            {
                stageList.Add(DirectorAPI.Stage.AbandonedAqueduct);
                stageList.Add(DirectorAPI.Stage.AphelianSanctuary);
                stageList.Add(DirectorAPI.Stage.WetlandAspect);
            }
            // S3
            if (stagesToInclude.Contains("3"))
            {
                stageList.Add(DirectorAPI.Stage.RallypointDelta);
                stageList.Add(DirectorAPI.Stage.ScorchedAcres);
                stageList.Add(DirectorAPI.Stage.SulfurPools);
            }
            // S4
            if (stagesToInclude.Contains("4"))
            {
                stageList.Add(DirectorAPI.Stage.AbyssalDepths);
                stageList.Add(DirectorAPI.Stage.SirensCall);
                stageList.Add(DirectorAPI.Stage.SunderedGrove);
            }
            // S5
            if (stagesToInclude.Contains("5"))
            {
                stageList.Add(DirectorAPI.Stage.SkyMeadow);
            }

            return stageList;
        }

        private static void AddHooks()
        {
            // Ensure activator can't buy shrine without having enough items
            Interactability PurchaseInteraction_GetInteractability(On.RoR2.PurchaseInteraction.orig_GetInteractability orig, PurchaseInteraction self, Interactor activator)
            {
                ShrineOfRevelationBehavior revelationBehavior = self.gameObject.GetComponent<ShrineOfRevelationBehavior>();
                CharacterBody body = activator.GetComponent<CharacterBody>();
                if (revelationBehavior && body && body.inventory)
                {
                    if (revelationBehavior.shrineTier == 1 && body.inventory.GetTotalItemCountOfTier(ItemTier.Tier1) <= 0)
                    {
                        return Interactability.ConditionsNotMet;
                    }
                    if (revelationBehavior.shrineTier == 2 && body.inventory.GetTotalItemCountOfTier(ItemTier.Tier2) <= 0)
                    {
                        return Interactability.ConditionsNotMet;
                    }
                    if (revelationBehavior.shrineTier == 3 && body.inventory.GetTotalItemCountOfTier(ItemTier.Tier3) <= 0)
                    {
                        return Interactability.ConditionsNotMet;
                    }
                }
                return orig(self, activator);
            }

            On.RoR2.PurchaseInteraction.GetInteractability += PurchaseInteraction_GetInteractability;
        }
    }
}
