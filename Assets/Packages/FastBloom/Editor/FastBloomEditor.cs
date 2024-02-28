using UnityEditor;

[CustomEditor(typeof(FastBloom))]
[CanEditMultipleObjects]
public class MobilePostProcessEditor : Editor
{
    SerializedProperty material;
    SerializedProperty setBloomIterations;
    SerializedProperty bloomIterations;
    SerializedProperty bloomDiffusion;
    SerializedProperty bloomColor;
    SerializedProperty bloomAmount;
    SerializedProperty bloomThreshold;
    SerializedProperty bloomSoftness;

    void OnEnable()
    {
        setBloomIterations = serializedObject.FindProperty("SetBloomIterations");
        bloomIterations = serializedObject.FindProperty("BloomIterations");
        bloomDiffusion = serializedObject.FindProperty("BloomDiffusion");
        bloomColor = serializedObject.FindProperty("BloomColor");
        bloomAmount = serializedObject.FindProperty("BloomAmount");
        bloomThreshold = serializedObject.FindProperty("BloomThreshold");
        bloomSoftness = serializedObject.FindProperty("BloomSoftness");
        material = serializedObject.FindProperty("material");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(setBloomIterations);
        if (setBloomIterations.boolValue)
            EditorGUILayout.PropertyField(bloomIterations);
        EditorGUILayout.PropertyField(bloomDiffusion);
        EditorGUILayout.PropertyField(bloomColor);
        EditorGUILayout.PropertyField(bloomAmount);
        EditorGUILayout.PropertyField(bloomThreshold);
        EditorGUILayout.PropertyField(bloomSoftness);
        EditorGUILayout.PropertyField(material);
        serializedObject.ApplyModifiedProperties();
    }
}