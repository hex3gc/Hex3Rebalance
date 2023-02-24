using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using BepInEx;
using R2API.Utils;
using R2API;
using UnityEngine;
using Hex3Rebalance.Init;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace Hex3Rebalance
{
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [BepInDependency(R2API.R2API.PluginGUID, R2API.R2API.PluginVersion)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [R2APISubmoduleDependency(nameof(ItemAPI), nameof(LanguageAPI), nameof(RecalculateStatsAPI), nameof(PrefabAPI))]
    public class Main : BaseUnityPlugin
    {
        public const string ModGuid = "com.Hex3.Hex3Rebalance";
        public const string ModName = "Hex3Rebalance";
        public const string ModVer = "0.1.0";

        public static Dictionary<string, string> ShaderLookup = new Dictionary<string, string>()
        {
            {"stubbed hopoo games/deferred/standard", "shaders/deferred/hgstandard"},
            {"stubbed hopoo games/fx/cloud intersection remap", "shaders/fx/hgintersectioncloudremap"},
            {"stubbed hopoo games/fx/cloud remap", "shaders/fx/hgcloudremap"}
        };

        public static bool debugMode = true; // DISABLE BEFORE BUILD

        public static Main instance;

        public static bool cancel = false;

        private void Awake()
        {
            instance = this;
            Assets.Init();
            if (cancel) return;
            var materialAssets = Assets.mainAssetBundle.LoadAllAssets<Material>();
            foreach (Material material in materialAssets)
            {
                if (!material.shader.name.StartsWith("Stubbed Hopoo Games"))
                {
                    continue;
                }
                var replacementShader = Resources.Load<Shader>(ShaderLookup[material.shader.name.ToLower()]);
                if (replacementShader)
                {
                    material.shader = replacementShader;
                }
            }

            Configs.Init();
            Items.Init();
        }
    }
}