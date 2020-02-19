// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public static class SodaEditorHelpers
    {
        #region Initialization of valueFieldActions
        private static Dictionary<Type, Func<Rect, GUIContent, object, object>> valueFieldActions;

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            valueFieldActions = new Dictionary<Type, Func<Rect, GUIContent, object, object>>();

            valueFieldActions.Add(typeof(bool), (rect, label, obj) => EditorGUI.Toggle(rect, label, (bool)obj));
            valueFieldActions.Add(typeof(int), (rect, label, obj) => EditorGUI.IntField(rect, label, (int)obj));
            valueFieldActions.Add(typeof(long), (rect, label, obj) => EditorGUI.LongField(rect, label, (long)obj));
            valueFieldActions.Add(typeof(uint), (rect, label, obj) =>
            {
                var value = EditorGUI.LongField(rect, label, (uint)obj);
                if (value < 0)
                {
                    value = 0;
                }
                if (value > uint.MaxValue)
                {
                    value = uint.MaxValue;
                }
                return (uint)value;
            });
            valueFieldActions.Add(typeof(float), (rect, label, obj) => EditorGUI.FloatField(rect, label, (float)obj));
            valueFieldActions.Add(typeof(double), (rect, label, obj) => EditorGUI.DoubleField(rect, label, (double)obj));
            valueFieldActions.Add(typeof(string), (rect, label, obj) => EditorGUI.TextField(rect, label, (string)obj));
            valueFieldActions.Add(typeof(Vector2), (rect, label, obj) => EditorGUI.Vector2Field(rect, label, (Vector2)obj));
            valueFieldActions.Add(typeof(Vector3), (rect, label, obj) => EditorGUI.Vector3Field(rect, label, (Vector3)obj));
            valueFieldActions.Add(typeof(Vector4), (rect, label, obj) => EditorGUI.Vector4Field(rect, label, (Vector4)obj));
            valueFieldActions.Add(typeof(Vector2Int), (rect, label, obj) => EditorGUI.Vector2IntField(rect, label, (Vector2Int)obj));
            valueFieldActions.Add(typeof(Vector3Int), (rect, label, obj) => EditorGUI.Vector3IntField(rect, label, (Vector3Int)obj));
            valueFieldActions.Add(typeof(Color), (rect, label, obj) => EditorGUI.ColorField(rect, label, (Color)obj));
            valueFieldActions.Add(typeof(AnimationCurve), (rect, label, obj) => EditorGUI.CurveField(rect, label, (AnimationCurve)obj));
            valueFieldActions.Add(typeof(Bounds), (rect, label, obj) => EditorGUI.BoundsField(rect, label, (Bounds)obj));
            valueFieldActions.Add(typeof(Gradient), (rect, label, obj) => EditorGUI.GradientField(rect, label, (Gradient)obj));
            valueFieldActions.Add(typeof(Rect), (rect, label, obj) => EditorGUI.RectField(rect, label, (Rect)obj));
            // Add more when needed
        }
        #endregion

        /// <summary>
        /// Displays a subtitle in the inspector. Used for ScriptableObjects.
        /// </summary>
        /// <param name="text">The subtitle to display.</param>
        public static void DisplayInspectorSubtitle(string text)
        {
#if UNITY_2019_1_OR_NEWER
            // We're not drawing that label in Unity 2019.x because we cannot do it without risking drawing over something else.
#else
            const int verticalPosition = 22;
            GUI.Label(new Rect(43, verticalPosition, 200, 32), text, EditorStyles.miniLabel);
#endif
        }

        /// <summary>
        /// Displays a field for a multitude of possible object types.
        /// Comparable to EditorGUI.PropertyField, but without the need to supply a serialized property.
        /// </summary>
        /// <param name="position">The position in the UI.</param>
        /// <param name="label">The label to display in the UI.</param>
        /// <param name="value">The current value of the property.</param>
        /// <param name="type">The type of the property.</param>
        public static void DisplayValueField(Rect position, GUIContent label, ref object value, Type type)
        {
            if(type.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                value = EditorGUI.ObjectField(position, label, (UnityEngine.Object)value, type, true);
            }
            else if(type.IsEnum)
            {
                value = EditorGUI.EnumPopup(position, label, (Enum)value);
            }
            else
            {
                Func<Rect, GUIContent, object, object> func;
                if(valueFieldActions.TryGetValue(type, out func))
                {
                    value = func(position, label, value);
                }
                else
                {
                    EditorGUI.HelpBox(position, "Cannot draw " + type.Name + " during play mode. Editing this value the inspector is disabled.", MessageType.Error);
                }
            }
        }

        /// <summary>
        /// Displays an expandable property field using EditorGUI.
        /// Used as a replacement for EditorGUILayout.PropertyField(property, label, true) as it didn't seem to work.
        /// </summary>
        /// <param name="property">The property to display.</param>
        /// <param name="label">The label to display.</param>
        public static void DisplayExpandablePropertyField(SerializedProperty property, GUIContent label = null)
        {
            property = property.Copy();
            EditorGUILayout.PropertyField(property, label);

            var previousIndentLevel = EditorGUI.indentLevel;

            if (property.isExpanded)
            {
                var childrenLeft = GetNumberOfPropertyChildren(property);

                property.NextVisible(true);
                while (childrenLeft > 0)
                {
                    EditorGUI.indentLevel = property.depth;
                    EditorGUILayout.PropertyField(property);
                    property.NextVisible(true);
                    childrenLeft--;
                }
            }

            EditorGUI.indentLevel = previousIndentLevel;
        }

        public static int GetNumberOfPropertyChildren(SerializedProperty property)
        {
            property = property.Copy();
            return property.CountInProperty() - 1;
        }

        /// <summary>
        /// Creates a text file within the application's datapath.
        /// </summary>
        /// <param name="content">The content of the file.</param>
        /// <param name="filename">The name of the file.</param>
        /// <returns>The file, loaded as a UnityEngine.TextAsset.</returns>
        public static TextAsset CreateTextFile(string content, string filename)
        {
            var path = Path.Combine(Application.dataPath, filename);

            if (File.Exists(path))
            {
                throw new IOException(string.Format("A file with the name {0} already exists in the asset folder root.", filename));
            }

            try
            {
                File.WriteAllText(path, content);
            }
            catch
            {
                throw new IOException("Could not write text file.");
            }

            AssetDatabase.Refresh();

            var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/" + filename);
            if (!textAsset)
            {
                throw new IOException("Could not write text file.");
            }

            return textAsset;
        }
        
        public static void DisplayAllPropertiesExcept(this SerializedProperty iterator, SerializedProperty property)
        {
            var enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                var showProperty = iterator.propertyPath != property.propertyPath;
                if (showProperty)
                {
                    EditorGUILayout.PropertyField(iterator);
                }
                enterChildren = showProperty;
            }
        }

        public static void DisplayAllPropertiesExcept(this SerializedProperty iterator, SerializedProperty propertyA, SerializedProperty propertyB)
        {
            var enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                var showProperty = iterator.propertyPath != propertyA.propertyPath &&
                                   iterator.propertyPath != propertyB.propertyPath;
                if (showProperty)
                {
                    EditorGUILayout.PropertyField(iterator);
                }
                enterChildren = showProperty;
            }
        }

        public static void DisplayAllPropertiesExcept(this SerializedProperty iterator, SerializedProperty propertyA, SerializedProperty propertyB, SerializedProperty propertyC)
        {
            var enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                var showProperty = iterator.propertyPath != propertyA.propertyPath &&
                                   iterator.propertyPath != propertyB.propertyPath &&
                                   iterator.propertyPath != propertyC.propertyPath;
                if (showProperty)
                {
                    EditorGUILayout.PropertyField(iterator);
                }
                enterChildren = showProperty;
            }
        }

        public static void DisplayAllPropertiesExcept(this SerializedObject serializedObject, bool showScriptField, SerializedProperty property)
        {
            var iterator = serializedObject.GetIterator();
            if (!showScriptField)
            {
                iterator.NextVisible(true);
            }
            iterator.DisplayAllPropertiesExcept(property);
        }

        public static void DisplayAllPropertiesExcept(this SerializedObject serializedObject, bool showScriptField, SerializedProperty propertyA, SerializedProperty propertyB)
        {
            var iterator = serializedObject.GetIterator();
            if (!showScriptField)
            {
                iterator.NextVisible(true);
            }
            iterator.DisplayAllPropertiesExcept(propertyA, propertyB);
        }

        public static void DisplayAllPropertiesExcept(this SerializedObject serializedObject, bool showScriptField, SerializedProperty propertyA, SerializedProperty propertyB, SerializedProperty propertyC)
        {
            var iterator = serializedObject.GetIterator();
            if (!showScriptField)
            {
                iterator.NextVisible(true);
            }
            iterator.DisplayAllPropertiesExcept(propertyA, propertyB, propertyC);
        }
    }
}
