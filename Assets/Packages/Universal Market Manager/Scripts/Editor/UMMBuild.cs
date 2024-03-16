using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UMM;
namespace UMM
{
    public class UMMBuild : IPreprocessBuildWithReport
    {
        public const string BUILDALL_ADDRESS = "UMM/Build for Myket and Bazaar";

        public int callbackOrder => 0;

        [MenuItem(BUILDALL_ADDRESS)]
        public static void BuildForMyketAndBazaar()
        {
            string baseBuildPath = EditorUtility.OpenFolderPanel("Choose Base Build Path", "", "");

            if (string.IsNullOrEmpty(baseBuildPath))
            {
                Debug.Log("Build path not selected.");
                return;
            }
            ChangeScriptingBackendToIl2CP();
            string[] markets = new string[] { "UMM_MYKET", "UMM_BAZAAR" };

            string[] scenes = GetBuildScenes();
            
            for (int m = 0; m < markets.Length; m++)
            {
                string market = markets[m];

                MarketSettings marketSettings = MarketSettings.Instance;
                string marketName = "";
                if (market == "UMM_MYKET")
                {
                    marketName = "Myket";
                    marketSettings.ActivateMyket();
                }
                else if (market == "UMM_BAZAAR")
                {
                    marketName = "Bazaar";
                    marketSettings.ActivateCafeBazaar();
                }
                string marketFolderPath = Path.Combine(baseBuildPath, marketName);

                if (!Directory.Exists(marketFolderPath))
                {
                    Directory.CreateDirectory(marketFolderPath);
                }

                
                
                // string path = Path.Combine(marketFolderPath, $"{PlayerSettings.productName}-{architecture}.apk");
                 BuildPipeline.BuildPlayer(scenes, marketFolderPath, BuildTarget.Android, BuildOptions.None);

                
            }
        }

        internal static void ChangeScriptingBackendToIl2CP()
        {
            if (!PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android).Equals(ScriptingImplementation.IL2CPP))
            {
                PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
                Debug.Log("Scripting backend changed to IL2CPP.");
            }
        }
        internal static string[] GetBuildScenes()
        {
             return EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();
        }
        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.Android)
            {
                Debug.LogError("This script only supports Android builds.");
                throw new System.Exception("This script only supports Android builds.");
            }
        }
    }
}