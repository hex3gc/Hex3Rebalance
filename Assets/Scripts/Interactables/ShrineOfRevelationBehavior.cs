using Hex3Rebalance.Init;
using RoR2;
using RoR2.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Hex3Rebalance.Interactables 
{
    [RequireComponent(typeof(PurchaseInteraction))]
    public class ShrineOfRevelationBehavior : NetworkBehaviour
    {
        public int shrineTier; // 1 = Common, 2 = Uncommon, 3 = Legendary
        public Transform symbolTransform;
        private int maxPurchaseCount;
        private float costMultiplierPerPurchase;
        private PurchaseInteraction purchaseInteraction;
        private int purchaseCount;
        private float refreshTimer;
        private float refreshDuration;
        private bool waitingForRefresh;

        public override int GetNetworkChannel()
        {
            return QosChannelIndex.defaultReliable.intVal;
        }

        private void Start()
        {
            purchaseInteraction = base.GetComponent<PurchaseInteraction>();
            maxPurchaseCount = Configs.ShrineOfRevelation_MaxUses.Value;
            costMultiplierPerPurchase = Configs.ShrineOfRevelation_UseCostMultiplier.Value;
            purchaseCount = 0;
            refreshTimer = 0f;
            refreshDuration = 2f;
            waitingForRefresh = false;
        }

        public void FixedUpdate()
        {
            if (waitingForRefresh)
            {
                refreshTimer += Time.fixedDeltaTime;
                if (refreshTimer >= refreshDuration && purchaseCount < maxPurchaseCount)
                {
                    purchaseInteraction.SetAvailable(true);
                    purchaseInteraction.Networkcost = (int)((float)purchaseInteraction.cost * costMultiplierPerPurchase);
                    waitingForRefresh = false;
                }
            }
        }

        [Server]
        public void AddShrineStack(Interactor interactor)
        {
            CharacterBody body = interactor.GetComponent<CharacterBody>();

            // Randomize body's item tier
            if (body && body.inventory)
            {
                Xoroshiro128Plus rng = new Xoroshiro128Plus(Run.instance.stageRng.nextUlong);
                List<ItemIndex> currentInventory = new List<ItemIndex>(body.inventory.itemAcquisitionOrder);
                foreach (ItemIndex item in currentInventory)
                {
                    ItemDef itemdef = ItemCatalog.GetItemDef(item);
                    List<ItemIndex> itemList = new List<ItemIndex>();
                    switch (shrineTier)
                    {
                        case 1: itemList = ItemCatalog.tier1ItemList; break;
                        case 2: itemList = ItemCatalog.tier2ItemList; break;
                        case 3: itemList = ItemCatalog.tier3ItemList; break;
                    }
                    Util.ShuffleList(itemList, rng);
                    rng.Next();

                    // Validate item tiers
                    if (shrineTier == 1 && itemdef.tier != ItemTier.Tier1)
                    {
                        continue;
                    }
                    if (shrineTier == 2 && itemdef.tier != ItemTier.Tier2)
                    {
                        continue;
                    }
                    if (shrineTier == 3 && itemdef.tier != ItemTier.Tier3)
                    {
                        continue;
                    }

                    // Remove item and give same amount of new one
                    int count = body.inventory.GetItemCount(itemdef);
                    body.inventory.RemoveItem(itemdef, count);
                    body.inventory.GiveItem(itemList.First(), count);
                }
            }

            this.waitingForRefresh = true;
            EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
            {
                origin = base.transform.position,
                rotation = Quaternion.identity,
                scale = 1f,
                color = new Color(1f, 1f, 1f)
            }, true);

            purchaseCount++;
            refreshTimer = 0f;
            if (purchaseCount >= maxPurchaseCount)
            {
                symbolTransform.gameObject.SetActive(false);
                purchaseInteraction.SetAvailable(false);
            }
        }
    }
}