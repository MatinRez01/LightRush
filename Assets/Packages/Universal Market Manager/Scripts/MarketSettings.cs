using System.Linq;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.Build;
#endif
using UnityEngine;

namespace UMM
{
    [CreateAssetMenu(menuName = "UMM/Intents Settings")]
    public class MarketSettings : ScriptableObject
    {
        public const string TAB_ADDRESS = "UMM/Market Settings";

        [Tooltip("It's optional. Fill it if you wanted to use OpenDeveloperApps method")]
        [SerializeField] private string BazaarDeveloperID;

        public string GetBazaarDeveloperId { get => BazaarDeveloperID; }

        #region Editor Methods
#if UNITY_EDITOR
        public void ActivateCafeBazaar()
        {
            if (Application.isPlaying)
                return;

            if (EditorApplication.isCompiling)
                return;

            PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Android, out string[] defines);
            var definesList = defines.ToList();

            foreach (var def in definesList.ToList())
            {
                if (def.Contains("UMM_MYKET"))
                    definesList.Remove("UMM_MYKET");
            }

            definesList.Add("UMM_BAZAAR");

            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Android, definesList.ToArray());
        }

        public void ActivateMyket()
        {
            if (Application.isPlaying)
                return;

            if (EditorApplication.isCompiling)
                return;

            PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Android, out string[] defines);
            var definesList = defines.ToList();

            foreach (var def in definesList.ToList())
            {
                if (def.Contains("UMM_BAZAAR"))
                    definesList.Remove("UMM_BAZAAR");
            }

            definesList.Add("UMM_MYKET");

            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Android, definesList.ToArray());
        }

        public string GetSelectedMarket()
        {
            PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Android, out string[] defines);
            var definesList = defines.ToList();

            foreach (var def in definesList.ToList())
            {
                if (def.Contains("UMM_BAZAAR"))
                    return "Bazaar";

                if (def.Contains("UMM_MYKET"))
                    return "Myket";
            }

            return "None";
        }
#endif
        #endregion

        #region Singleton
        private static MarketSettings instance;
        public static MarketSettings Instance { get => instance == null ? instance = Resources.Load<MarketSettings>("Market Settings") : instance; }
        #endregion
    }
}