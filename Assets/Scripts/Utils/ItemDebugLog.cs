using UnityEngine;

namespace Hex3Rebalance.Utils
{
    public static class ItemDebugLog
    {
        public static bool PrintItemChange(bool configEnable, string itemTier, string itemName)
        {
            if (configEnable)
            {
                Debug.Log(Main.ModName + ": (" + itemTier + ") " + itemName + "");
                return true;
            }
            else
            {
                Debug.Log(Main.ModName + ": (" + itemTier + ") " + itemName + " disabled, cancelling changes");
                return false;
            }
        }
    }
}
