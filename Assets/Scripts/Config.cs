using BepInEx.Configuration;

namespace Hex3Rebalance.Init
{
    public static class Configs
    {
        // Common
        public static ConfigEntry<bool> StunGrenade_Enable;
        public static ConfigEntry<float> StunGrenade_ExplosionChance;
        public static ConfigEntry<float> StunGrenade_ExplosionRadius;
        public static ConfigEntry<float> StunGrenade_ExplosionRadiusStack;
        public static ConfigEntry<float> StunGrenade_ExplosionDamage;

        public static ConfigEntry<bool> BustlingFungus_Enable;
        public static ConfigEntry<float> BustlingFungus_Radius;
        public static ConfigEntry<float> BustlingFungus_RadiusStack;
        public static ConfigEntry<float> BustlingFungus_HealFraction;
        public static ConfigEntry<float> BustlingFungus_HealFractionStack;
        public static ConfigEntry<float> BustlingFungus_Interval;
        public static ConfigEntry<bool> BustlingFungus_WindDown;

        // Uncommon
        public static ConfigEntry<bool> LeptonDaisy_Enable;
        public static ConfigEntry<float> LeptonDaisy_WeakDuration;

        public static ConfigEntry<bool> HuntersHarpoon_Enable;
        public static ConfigEntry<float> HuntersHarpoon_SpeedPerKill;
        public static ConfigEntry<int> HuntersHarpoon_SpeedCap;
        public static ConfigEntry<int> HuntersHarpoon_SpeedCapStack;
        public static ConfigEntry<int> HuntersHarpoon_MountainShrineStack;
        public static ConfigEntry<int> HuntersHarpoon_MountainShrineAdditionalStack;

        // Lunar
        public static ConfigEntry<bool> Corpsebloom_Enable;
        public static ConfigEntry<float> Corpsebloom_ExplosionRange;
        public static ConfigEntry<float> Corpsebloom_ExplosionRangeStack;
        public static ConfigEntry<float> Corpsebloom_AntiRegenStack;
        public static ConfigEntry<float> Corpsebloom_PercentHealthThreshold;

        public static ConfigEntry<bool> LightFluxPauldron_Enable;
        public static ConfigEntry<float> LightFluxPauldron_MoveSpeedStack;
        public static ConfigEntry<float> LightFluxPauldron_CooldownReductionStack;
        // public static ConfigEntry<float> LightFluxPauldron_HealthReductionStack;

        public static ConfigEntry<bool> StoneFluxPauldron_Enable;
        public static ConfigEntry<float> StoneFluxPauldron_HealthIncrease;
        public static ConfigEntry<float> StoneFluxPauldron_HealthIncreaseStack;
        public static ConfigEntry<float> StoneFluxPauldron_RegenAdd;
        public static ConfigEntry<float> StoneFluxPauldron_RegenAddStack;
        public static ConfigEntry<float> StoneFluxPauldron_HealingReduce;
        public static ConfigEntry<float> StoneFluxPauldron_HealingReduceStack;

        public static ConfigEntry<bool> FocusedConvergence_Enable;
        public static ConfigEntry<float> FocusedConvergence_RadiusIncrease;
        public static ConfigEntry<float> FocusedConvergence_RadiusIncreaseStack;
        public static ConfigEntry<float> FocusedConvergence_SpeedIncrease;
        public static ConfigEntry<float> FocusedConvergence_SpeedIncreaseStack;
        public static ConfigEntry<float> FocusedConvergence_BackslideIncrease;
        public static ConfigEntry<float> FocusedConvergence_BackslideIncreaseStack;
        public static ConfigEntry<float> FocusedConvergence_ShrinkFactor;

        // Void Common
        public static ConfigEntry<bool> NeedleTick_Enable;
        public static ConfigEntry<float> NeedleTick_Damage;
        public static ConfigEntry<float> NeedleTick_DamageStack;

        // Gameplay
        public static ConfigEntry<bool> VoidCradles;
        public static ConfigEntry<float> VoidCradles_Duration;

        public static void Init()
        {
            // Common
            StunGrenade_Enable = Main.instance.Config.Bind<bool>(new ConfigDefinition("Items (Common) - Stun Grenade", "Enable changes"), true, new ConfigDescription("Toggle changes to this item."));
            StunGrenade_ExplosionChance = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Common) - Stun Grenade", "Explosion chance"), 5f, new ConfigDescription("Percent chance that a stunning explosion will occur on hit (Stacking hyperbolically)"));
            StunGrenade_ExplosionRadius = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Common) - Stun Grenade", "Explosion radius"), 1.5f, new ConfigDescription("Base radius of stun grenade explosion, in meters"));
            StunGrenade_ExplosionRadiusStack = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Common) - Stun Grenade", "Explosion radius per stack"), 0.5f, new ConfigDescription("Radius of stun grenade explosion, in meters, per additional stack"));
            StunGrenade_ExplosionDamage = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Common) - Stun Grenade", "Explosion damage"), 25f, new ConfigDescription("Percent of base damage that the explosion deals"));

            BustlingFungus_Enable = Main.instance.Config.Bind<bool>(new ConfigDefinition("Items (Common) - Bustling Fungus", "Enable changes"), true, new ConfigDescription("Toggle changes to this item."));
            BustlingFungus_Radius = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Common) - Bustling Fungus", "Zone radius"), 4f, new ConfigDescription("Mushroom zone radius in meters"));
            BustlingFungus_RadiusStack = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Common) - Bustling Fungus", "Zone radius stack"), 2f, new ConfigDescription("Mushroom zone radius in meters, per additional stack"));
            BustlingFungus_HealFraction = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Common) - Bustling Fungus", "Heal fraction"), 2.5f, new ConfigDescription("Mushroom zone healing percent"));
            BustlingFungus_HealFractionStack = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Common) - Bustling Fungus", "Heal fraction stack"), 2f, new ConfigDescription("Mushroom zone healing percent, per additional stack"));
            BustlingFungus_Interval = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Common) - Bustling Fungus", "Zone drop interval"), 10f, new ConfigDescription("Seconds between mushroom zones being deployed"));
            BustlingFungus_WindDown = Main.instance.Config.Bind<bool>(new ConfigDefinition("Items (Common) - Bustling Fungus", "Zone expiry shrink"), true, new ConfigDescription("Toggle shrinking before zone expiry, in case it causes issues at short intervals."));

            // Uncommon
            LeptonDaisy_Enable = Main.instance.Config.Bind<bool>(new ConfigDefinition("Items (Uncommon) - Lepton Daisy", "Enable changes"), true, new ConfigDescription("Toggle changes to this item."));
            LeptonDaisy_WeakDuration = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Uncommon) - Lepton Daisy", "Weaken duration"), 6f, new ConfigDescription("How long enemies are weakened by the healing nova, in seconds."));

            HuntersHarpoon_Enable = Main.instance.Config.Bind<bool>(new ConfigDefinition("Items (Uncommon) - Hunters Harpoon", "Enable changes"), true, new ConfigDescription("Toggle changes to this item."));
            HuntersHarpoon_SpeedPerKill = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Uncommon) - Hunters Harpoon", "Speed per kill"), 3f, new ConfigDescription("Percent speed boost added per kill"));
            HuntersHarpoon_SpeedCap = Main.instance.Config.Bind<int>(new ConfigDefinition("Items (Uncommon) - Hunters Harpoon", "Speed cap"), 10, new ConfigDescription("Maximum stacks of buff applied"));
            HuntersHarpoon_SpeedCapStack = Main.instance.Config.Bind<int>(new ConfigDefinition("Items (Uncommon) - Hunters Harpoon", "Speed cap stack"), 10, new ConfigDescription("Maximum stacks of buff applied, per additional stack"));
            HuntersHarpoon_MountainShrineStack = Main.instance.Config.Bind<int>(new ConfigDefinition("Items (Uncommon) - Hunters Harpoon", "Mountain shrine addition"), 1, new ConfigDescription("Extra mountain shrine stacks added"));
            HuntersHarpoon_MountainShrineAdditionalStack = Main.instance.Config.Bind<int>(new ConfigDefinition("Items (Uncommon) - Hunters Harpoon", "Mountain shrine additional stacks"), 1, new ConfigDescription("Extra mountain shrine stacks added, per additional stack"));

            // Lunar
            Corpsebloom_Enable = Main.instance.Config.Bind<bool>(new ConfigDefinition("Items (Lunar) - Corpsebloom", "Enable changes"), true, new ConfigDescription("Toggle changes to this item."));
            Corpsebloom_ExplosionRange = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Lunar) - Corpsebloom", "Poison burst radius"), 20f, new ConfigDescription("Base poison burst radius in meters"));
            Corpsebloom_ExplosionRangeStack = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Lunar) - Corpsebloom", "Poison burst radius per stack"), 5f, new ConfigDescription("Base poison burst radius in meters, per additional stack"));
            Corpsebloom_AntiRegenStack = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Lunar) - Corpsebloom", "Anti-regen multiplier per stack"), 0.5f, new ConfigDescription("Inverted regen is boosted by this multiplier per additional stack"));
            Corpsebloom_PercentHealthThreshold = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Lunar) - Corpsebloom", "Poison burst anti-regen threshold"), 100f, new ConfigDescription("Percent of the holder's max health required to create a poison burst"));

            LightFluxPauldron_Enable = Main.instance.Config.Bind<bool>(new ConfigDefinition("Items (Lunar) - Light Flux Pauldron", "Enable changes"), true, new ConfigDescription("Toggle changes to this item."));
            LightFluxPauldron_MoveSpeedStack = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Lunar) - Light Flux Pauldron", "Movement speed percent add"), 30f, new ConfigDescription("Additional move speed granted by the item in percentage."));
            LightFluxPauldron_CooldownReductionStack = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Lunar) - Light Flux Pauldron", "Percent utility cooldown reduction"), 50f, new ConfigDescription("Utility cooldown reduction in percentage"));
            // LightFluxPauldron_HealthReductionStack = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Lunar) - Light Flux Pauldron", "Percent healthbar reduction"), 50f, new ConfigDescription("Max health reduction in percentage"));

            StoneFluxPauldron_Enable = Main.instance.Config.Bind<bool>(new ConfigDefinition("Items (Lunar) - Stone Flux Pauldron", "Enable changes"), true, new ConfigDescription("Toggle changes to this item."));
            StoneFluxPauldron_HealthIncrease = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Lunar) - Stone Flux Pauldron", "Health increase"), 100f, new ConfigDescription("Max health increase in percentage"));
            StoneFluxPauldron_HealthIncreaseStack = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Lunar) - Stone Flux Pauldron", "Health increase stack"), 100f, new ConfigDescription("Max health increase in percentage, per additional stack."));
            StoneFluxPauldron_RegenAdd = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Lunar) - Stone Flux Pauldron", "Regen add"), 8f, new ConfigDescription("HP per second to regenerate"));
            StoneFluxPauldron_RegenAddStack = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Lunar) - Stone Flux Pauldron", "Regen add stack"), 4f, new ConfigDescription("HP per second to regenerate, per additional stack."));
            StoneFluxPauldron_HealingReduce = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Lunar) - Stone Flux Pauldron", "Healing decrease"), 50f, new ConfigDescription("Healing decrease in percentage"));
            StoneFluxPauldron_HealingReduceStack = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Lunar) - Stone Flux Pauldron", "Healing decrease stack"), 50f, new ConfigDescription("Healing decrease in percentage, per additional stack."));

            FocusedConvergence_Enable = Main.instance.Config.Bind<bool>(new ConfigDefinition("Items (Lunar) - Focused Convergence", "Enable changes"), true, new ConfigDescription("Toggle changes to this item."));
            FocusedConvergence_RadiusIncrease = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Lunar) - Focused Convergence", "Radius increase"), 10f, new ConfigDescription("Radius increase in percentage"));
            FocusedConvergence_RadiusIncreaseStack = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Lunar) - Focused Convergence", "Radius increase stack"), 5f, new ConfigDescription("Radius increase in percentage, per additional stack."));
            FocusedConvergence_SpeedIncrease = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Lunar) - Focused Convergence", "Charge speed increase"), 30f, new ConfigDescription("Charge speed increase in percentage"));
            FocusedConvergence_SpeedIncreaseStack = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Lunar) - Focused Convergence", "Charge speed increase stack"), 30f, new ConfigDescription("Charge speed increase in percentage, per additional stack."));
            FocusedConvergence_BackslideIncrease = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Lunar) - Focused Convergence", "Charge backslide"), 5f, new ConfigDescription("How much percentage per second is lost while outside of the holdout range"));
            FocusedConvergence_BackslideIncreaseStack = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Lunar) - Focused Convergence", "Charge backslide stack"), 2.5f, new ConfigDescription("How much percentage per second is lost while outside of the holdout range, per stack"));
            FocusedConvergence_ShrinkFactor = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Lunar) - Focused Convergence", "Shrink factor"), 0.5f, new ConfigDescription("Multiplier for how much of the zone's base radius is deducted while charging"));

            // Void Common
            NeedleTick_Enable = Main.instance.Config.Bind<bool>(new ConfigDefinition("Items (Void Common) - NeedleTick", "Enable changes"), true, new ConfigDescription("Toggle changes to this item."));
            NeedleTick_Damage = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Void Common) - NeedleTick", "Damage"), 100f, new ConfigDescription("Percent total damage dealt by Collapse"));
            NeedleTick_DamageStack = Main.instance.Config.Bind<float>(new ConfigDefinition("Items (Void Common) - NeedleTick", "Damage per stack"), 50f, new ConfigDescription("Percent total damage dealt by Collapse per additional stack"));

            // Gameplay
            VoidCradles = Main.instance.Config.Bind<bool>(new ConfigDefinition("Gameplay - Void Cradles", "Enable changes"), true, new ConfigDescription("Toggle changes to this interactable."));
            VoidCradles_Duration = Main.instance.Config.Bind<float>(new ConfigDefinition("Gameplay - Void Cradles", "Debuff duration"), 15f, new ConfigDescription("How long void cradle anti-heal and regen lasts in seconds"));
        }
    }
}
