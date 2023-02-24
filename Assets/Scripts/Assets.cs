using R2API;
using R2API.ScriptableObjects;
using RoR2;
using RoR2.ContentManagement;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Hex3Rebalance.Init
{
    public static class Assets
    {
        public const string assetBundleName = "hex3rebalanceassets";
        public const string contentPackName = "H3RContentPack";

        public static AssetBundle mainAssetBundle = null;
        public static ContentPack mainContentPack = null;
        public static SerializableContentPack serialContentPack = null;

        public static List<EffectDef> effectDefs = new List<EffectDef>();

        public static void Init()
        {
            LoadAssetBundle();
        }

        public static void LoadAssetBundle()
        {
            if (mainAssetBundle == null)
            {
                var path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                mainAssetBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(path, assetBundleName));

                if (!mainAssetBundle)
                {
                    Debug.LogError(Main.ModName + ": AssetBundle not found. Mod will stop loading.");
                    Main.cancel = true;
                    return;
                }
                LoadContentPack();
            }
        }
        public static void LoadContentPack()
        {
            R2API.ContentManagement.R2APIContentManager.AddPreExistingSerializableContentPack(mainAssetBundle.LoadAsset<R2APISerializableContentPack>(contentPackName));
        }
    }

    public class ContentPackProvider : IContentPackProvider
    {
        public static SerializableContentPack serializedContentPack;
        public static ContentPack contentPack;

        public static string contentPackName = null;
        public string identifier
        {
            get
            {
                return Main.ModName;
            }
        }

        public static void Initialize()
        {
            contentPackName = Assets.contentPackName;
            ContentManager.collectContentPackProviders += AddCustomContent;
        }

        private static void AddCustomContent(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(new ContentPackProvider());
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(contentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }
    }
}
