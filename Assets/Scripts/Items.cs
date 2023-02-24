using Hex3Rebalance.ItemChanges;

namespace Hex3Rebalance.Init
{
    public static class Items
    {
        public static void Init()
        {
            // Common
            BustlingFungus.Init("Bustling Fungus", "Common");
            StunGrenade.Init("Stun Grenade", "Common");
            // Uncommon
            LeptonDaisy.Init("Lepton Daisy", "Uncommon");
            // Lunar
            Corpsebloom.Init("Corpsebloom", "Lunar");
            LightFluxPauldron.Init("Light Flux Pauldron", "Lunar");
            StoneFluxPauldron.Init("Stone Flux Pauldron", "Lunar");
            FocusedConvergence.Init("Focused Convergence", "Lunar");
            // Void Common
            NeedleTick.Init("NeedleTick", "Void Common");
        }
    }
}
