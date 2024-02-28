using UnityEditor;
using UMM;

public class UMMEditorWindow : Editor
{
    [MenuItem(MarketSettings.TAB_ADDRESS)]
    public static void OpenMarketSettings()
    {
        Selection.activeObject = MarketSettings.Instance;
    }
}