using UMM;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MarketSettings))]
public class MarketSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var target = base.target as MarketSettings;

        EditorGUILayout.LabelField($"Selected Market: {target.GetSelectedMarket()}");

        EditorGUILayout.BeginHorizontal("box");

        if (GUILayout.Button("Activate Cafe Bazaar"))
            target.ActivateCafeBazaar();

        if (GUILayout.Button("Activate Myket"))
            target.ActivateMyket();

        EditorGUILayout.EndHorizontal();
    }
}