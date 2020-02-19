// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(RuntimeSetElement))]
    [CanEditMultipleObjects]
    public class RuntimeSetElementEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Don't draw the default inspector in order to hide the "Script" property line
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_runtimeSet"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}
