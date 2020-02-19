// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.IO;

    /// <summary>
    /// This wizard creates a new ValueObject type for representing the type you enter.
    /// The class comes with a matching ValueReference type and Editor classes for both.
    /// </summary>
    public abstract class TypeCreationWizardBase : EditorWindow
    {
        protected abstract string templatePath { get; }
        protected abstract string successMessage { get; }

        private string enteredTypeName = "";

        protected static void OpenWizard<T>(string title) where T : EditorWindow
        {
            var window = GetWindow<T>(true, title, true);
            var size = new Vector2(550, 200);
            window.minSize = size;
            window.maxSize = size;

            window.Focus();
        }

        private void OnEnable()
        {
            enteredTypeName = "";
        }

        private void OnGUI()
        {
            DisplayTextAndOptions();

            GUILayout.BeginHorizontal();

            DisplayWithRelativeOffsetFromTop(() =>
            {
                GUI.SetNextControlName("TextField");
                enteredTypeName = GUILayout.TextField(enteredTypeName, GUILayout.Width(410), GUILayout.Height(20));
            });

            var validClassNameEntered = IsValidInput(enteredTypeName);
            var enterIsPressed = Event.current != null &&
                                 Event.current.type == EventType.KeyUp &&
                                 Event.current.keyCode == KeyCode.Return;

            if (validClassNameEntered && enterIsPressed)
            {
                QueueGeneratingValueObjectClass();
                GUI.enabled = false;
            }
            else
            {
                GUI.enabled = validClassNameEntered;
            }

            DisplayWithRelativeOffsetFromTop(() =>
            {
                if (GUILayout.Button("Generate", GUILayout.Width(120), GUILayout.Height(20)))
                {
                    QueueGeneratingValueObjectClass();
                }
            });

            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }

        protected abstract void DisplayTextAndOptions();

        private static void DisplayWithRelativeOffsetFromTop(Action displayElementsAction, float offset = 0)
        {
            GUILayout.BeginVertical();
            GUILayout.Space(offset);
            displayElementsAction();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        protected virtual bool IsValidInput(string input)
        {
            return true;
        }

        private void QueueGeneratingValueObjectClass()
        {
            var typeName = enteredTypeName;
            EditorApplication.delayCall += () => GenerateValueObjectClass(typeName);
        }

        private void GenerateValueObjectClass(string typeName)
        {
            var template = Resources.Load<TextAsset>(templatePath);
            if (template == null)
            {
                DisplayFailure("The wizard could not find the correct class template.");
                return;
            }

            var code = template.text;
            code = ParseTemplate(code, typeName);
            TextAsset createdAsset;
            try
            {
                createdAsset = SodaEditorHelpers.CreateTextFile(code, GenerateFilename(typeName));
            }
            catch (IOException e)
            {
                DisplayFailure(e.Message);
                return;
            }

            Close();

            EditorUtility.DisplayDialog("Success", successMessage, "OK");

            EditorApplication.projectChanged += () => Selection.activeObject = createdAsset;
        }

        protected abstract string GenerateFilename(string typeName);

        protected abstract string ParseTemplate(string content, string input);

        private void DisplayFailure(string reason)
        {
            EditorUtility.DisplayDialog("Could not create class", reason, "OK");
            Focus();
        }
    }
}
