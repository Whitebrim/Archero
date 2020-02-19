// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System.Reflection;
    using System.Text.RegularExpressions;

    [CustomPropertyDrawer(typeof(DisplayInsteadInPlaymodeAttribute))]
    public class DisplayInsteadInPlaymodeDrawer : PropertyDrawer
    {
        private const string tooltip = "Changes to this value are not being saved when exiting play mode.";
        private static readonly Regex labelFormattingRegex = new Regex("((?<=[a-z])[A-Z0-9])|_");

        private static Texture2D playmodeIcon;
        private static Texture2D playmodeIconOff;
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            playmodeIcon = Resources.Load<Texture2D>("Icon-Playmode-On");
            playmodeIconOff = Resources.Load<Texture2D>("Icon-Playmode-Off");
        }

        private PropertyInfo replacementProperty;
        private FieldInfo replacementField;
        private string formattedInspectorPropertyName;

        private void InitializeIfNeeded(object target)
        {
            var diipAttribute = (DisplayInsteadInPlaymodeAttribute)attribute;

            if (diipAttribute.replacementName != null && replacementProperty == null)
            {
                var bindingFlags = BindingFlags.SetProperty |
                    BindingFlags.GetProperty |
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic;
                replacementProperty = target.GetType().GetProperty(diipAttribute.replacementName, bindingFlags);

                if (replacementProperty != null)
                {
                    formattedInspectorPropertyName = FormatUnityInspectorPropertyLabel(replacementProperty.Name);
                }
                else
                {
                    replacementField = target.GetType().GetField(diipAttribute.replacementName, bindingFlags);

                    if (replacementField != null)
                    {
                        formattedInspectorPropertyName = FormatUnityInspectorPropertyLabel(replacementField.Name);
                    }
                    else
                    {
                        Debug.LogError("Could not find replacement property or field to draw.");
                    }
                }
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var serializedObject = property.serializedObject;
            var target = serializedObject.targetObject;

            var diipAttribute = (DisplayInsteadInPlaymodeAttribute)attribute;

            InitializeIfNeeded(target);

            if (Application.isPlaying)
            {
                if (replacementProperty != null || replacementField != null)
                {
                    property.isExpanded = false;

                    var playmodeLabel = new GUIContent(label);
                    if (target is ScriptableObject)
                    {
                        playmodeLabel.image = playmodeIcon;
                        // Tooltips are, by design, not displayed in play mode.
                        /*
                         * var originalTooltip = !string.IsNullOrEmpty(label.tooltip) ? label.tooltip + "\n\n" : "";
                         * playmodeLabel.tooltip = originalTooltip + tooltip;
                         */
                    }

                    if (replacementProperty != null)
                    {
                        playmodeLabel.text = formattedInspectorPropertyName;
                        var value = replacementProperty.GetValue(target, null);
                        SodaEditorHelpers.DisplayValueField(position, playmodeLabel, ref value, replacementProperty.PropertyType);
                        replacementProperty.SetValue(target, value, null);
                    }
                    else
                    {
                        playmodeLabel.text = formattedInspectorPropertyName;
                        var value = replacementField.GetValue(target);
                        SodaEditorHelpers.DisplayValueField(position, playmodeLabel, ref value, replacementField.FieldType);
                        replacementField.SetValue(target, value);
                    }
                }
                else
                {
                    GUI.enabled = false;
                    EditorGUI.PropertyField(position, property, label);
                    GUI.enabled = true;
                }
            }
            else
            {
                var editmodeLabel = new GUIContent(label);
                if (target is ScriptableObject)
                {
                    editmodeLabel.text = formattedInspectorPropertyName;
                    editmodeLabel.image = playmodeIconOff;
                    var attributeTooltip = !string.IsNullOrEmpty(diipAttribute.tooltip) ? diipAttribute.tooltip + "\n\n" : "";
                    editmodeLabel.tooltip = attributeTooltip + tooltip;
                }
                EditorGUI.PropertyField(position, property, editmodeLabel);
            }
        }

        private static string FormatUnityInspectorPropertyLabel(string propertyName)
        {
            var result = labelFormattingRegex.Replace(propertyName, match => " " + (match.Value == "_" ? "" : match.Value));
            result = result.Trim();
            result = (result[0] + "").ToUpper() + result.Substring(1);
            return result;
        }
    }
}
