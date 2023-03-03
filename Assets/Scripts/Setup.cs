using Hex3Rebalance.Interactables;
using Hex3Rebalance.ItemChanges;
using Hex3Rebalance.Modules;

namespace Hex3Rebalance.Init
{
    public static class Setup
    {
        public static void Init()
        {
            // Common
            BustlingFungus.Init();
            StunGrenade.Init();
            // Uncommon
            LeptonDaisy.Init();
            HuntersHarpoon.Init();
            // Lunar
            Corpsebloom.Init();
            LightFluxPauldron.Init();
            StoneFluxPauldron.Init();
            FocusedConvergence.Init();
            // Void Common
            NeedleTick.Init();

            // Gameplay
            VoidCradle.Init();

            // Interactables
            ShrineOfRevelation.Init();
        }
    }
}
