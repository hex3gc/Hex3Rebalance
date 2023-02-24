using System.Collections.Generic;
using RoR2;
using UnityEngine;

namespace Hex3Rebalance.Modules
{
    public static class FirstHit
    {
        // Used with first-hit items to track who has hit a victim
        public static bool InflictFirstHit(GameObject attacker, GameObject victim)
        {
            if (!victim.GetComponent<FirstHitTracker>())
            {
                victim.AddComponent<FirstHitTracker>();
            }
            FirstHitTracker tracker = victim.GetComponent<FirstHitTracker>();

            if (!tracker.attackerList.Contains(attacker.GetInstanceID()))
            {
                tracker.attackerList.Add(attacker.GetInstanceID());
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class FirstHitTracker : MonoBehaviour
    {
        public List<int> attackerList = new List<int>();
    }
}
