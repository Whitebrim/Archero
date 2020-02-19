// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using System.Reflection;
    using UnityEngine;
    using UnityEditor;
    
    [CustomPropertyDrawer(typeof(ScopedVariableBase), true)]
    public class ScopedVariablePropertyDrawer : PropertyDrawer
    {
        private static readonly GUIContent[] options = new GUIContent[] { new GUIContent("Global Variable"), new GUIContent("Local Value") };
        private static readonly object[] noParameters = new object[0];
        private static GUIStyle popupStyle;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(popupStyle == null)
            {
                popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
                popupStyle.imagePosition = ImagePosition.ImageOnly;
            }

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            var modeRect = new Rect(position.x + 4, position.y + 4, 20, EditorGUIUtility.singleLineHeight);
            var valueRect = new Rect(position.x + 25, position.y, position.width - 25, EditorGUIUtility.singleLineHeight);
            
            EditorGUI.BeginChangeCheck();

            // Draw reference mode (useConstant) property
            var useLocalProperty = property.FindPropertyRelative("useLocal");
            var useLocal = useLocalProperty.boolValue;
            useLocal = EditorGUI.Popup(modeRect, GUIContent.none, useLocal ? 1 : 0, options, popupStyle) == 1;
            useLocalProperty.boolValue = useLocal;

            // Draw value property
            if(useLocal)
            {
                EditorGUIUtility.labelWidth = 100;
                var localValueProperty = property.FindPropertyRelative("localValue");
                if (localValueProperty.hasChildren)
                {
                    valueRect.xMin += 10;
                }
                EditorGUI.PropertyField(valueRect, localValueProperty, new GUIContent("Value"), true);
                EditorGUIUtility.labelWidth = 0;
            }
            else
            {
                EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("globalVariable"), GUIContent.none);
            }

            if(EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();

                if(Application.isPlaying)
                {
                    UpdateObject(property);
                }
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var lines = 1;

            var useLocalProperty = property.FindPropertyRelative("useLocal");
            var useLocal = useLocalProperty.boolValue;
            if (useLocal)
            {
                var localValueProperty = property.FindPropertyRelative("localValue");
                if (localValueProperty.isExpanded)
                {
                    lines = SodaEditorHelpers.GetNumberOfPropertyChildren(localValueProperty) + 1;
                }
            }

            return lines * (EditorGUIUtility.singleLineHeight + 2);
        }

        /// <summary>
        /// Updates the runtime object, including a proper update of the monitoring state and a notification for all objects monitoring it
        /// </summary>
        private static void UpdateObject(SerializedProperty property)
        {
            var targetObject = property.serializedObject.targetObject;
            var targetObjectType = targetObject.GetType();
            var field = targetObjectType.GetField(property.propertyPath, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            // Happens once when changing a thing at runtime
            // But the method will be called again, so it works
            if(field == null) return;

            var targetScopedValue = (ScopedVariableBase)field.GetValue(targetObject);
            targetScopedValue.InvokeOnChangeEvent();
            targetScopedValue.UpdateGlobalVariableMonitoring();
        }
    }
}
