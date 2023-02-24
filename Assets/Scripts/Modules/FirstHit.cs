using System.Collections.Generic;
using RoR2;
using UnityEngine;

namespace Hex3Rebalance.Modules
{
    public static class FirstHit
    {
        // Used with first-hit items to track who has hit a victim
        private static bool InflictFirstHit(CharacterBody attacker, CharacterBody victim)
        {
            if (!victim.gameObject.GetComponent<FirstHitTracker>())
            {
                victim.gameObject.AddComponent<FirstHitTracker>();
            }
            FirstHitTracker tracker = victim.gameObject.GetComponent<FirstHitTracker>();

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
