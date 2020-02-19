// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;

    [CustomEditor(typeof(Savestate))]
    public class SavestateEditor : Editor
    {
        private ReorderableList list;

        private void OnEnable()
        {
            InitializeList();
        }

        private void InitializeList()
        {
            var canEditListItems = !Application.isPlaying;

            list = new ReorderableList(serializedObject,
                serializedObject.FindProperty("entries"),
                canEditListItems, true, canEditListItems, canEditListItems);

            list.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Entries");
            };

            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;

                var keyProperty = element.FindPropertyRelative("key");
                var valueProperty = element.FindPropertyRelative("value");

                var keyWidth = 150;
                var padding = 10;
                var valueX = keyWidth + padding;
                var valueWidth = rect.width - valueX;

                var normalColor = GUI.color;
                if (string.IsNullOrEmpty(keyProperty.stringValue))
                {
                    GUI.color *= new Color(1, 0.5f, 0.5f, 1);
                }
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, keyWidth, rect.height), keyProperty, GUIContent.none);
                GUI.color = normalColor;

                EditorGUI.PropertyField(new Rect(valueX + 30, rect.y, valueWidth, rect.height), valueProperty, GUIContent.none);
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
