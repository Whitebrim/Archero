// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using UnityEngine;
    using UnityEditor;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(GlobalVariableBase), editorForChildClasses: true)]
    public class GlobalVariableEditor : Editor
    {
        protected virtual string subtitle { get { return target.GetType().Name; } }

        protected virtual void OnEnable()
        {
            EditorApplication.update += () => Repaint();
        }

        protected virtual void OnDisable()
        {
            EditorApplication.update -= () => Repaint();
        }

        public override void OnInspectorGUI()
        {
            SodaEditorHelpers.DisplayInspectorSubtitle(subtitle);

            var originalValueProperty = serializedObject.FindProperty("originalValue");
            var descriptionProperty = serializedObject.FindProperty("description");

            serializedObject.Update();

            if (originalValueProperty != null)
            {
                SodaEditorHelpers.DisplayExpandablePropertyField(originalValueProperty);
            }
            else
            {
                EditorGUILayout.HelpBox("The type of the value this GlobalVariable is representing does not seem to be serializable.", MessageType.Error);
            }

            serializedObject.DisplayAllPropertiesExcept(false, originalValueProperty, descriptionProperty);

            EditorGUILayout.PropertyField(descriptionProperty);

            serializedObject.ApplyModifiedProperties();

            if (Application.isPlaying)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Objects responding to onChangeValue event");
                if (targets.Length == 1)
                {
                    var globalVariableTarget = (GlobalVariableBase)target;
                    SodaEventDrawer.DisplayListeners(globalVariableTarget.GetOnChangeEvent());
                }
                else
                {
                    EditorGUILayout.HelpBox("Cannot display when multiple GlobalVariables are selected.", MessageType.Warning);
                }
            }
        }
    }
}
