// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System.Text.RegularExpressions;

    /// <summary>
    /// This wizard creates a new RuntimeSet type for representing a set containing objects of the the type you enter.
    /// </summary>
    public class RuntimeSetCreationWizard : TypeCreationWizardBase
    {
        [MenuItem("Window/Soda/Create/RuntimeSet Type")]
        private static void Open()
        {
            OpenWizard<RuntimeSetCreationWizard>("Soda RuntimeSet Creation Wizard");
        }

        private bool singleComponent = true;
        private string className = "";

        protected override string templatePath { get { return singleComponent ? "RuntimeSetTemplateSingle.cs" : "RuntimeSetTemplateMultiple.cs"; } }
        protected override string successMessage { get { return "The RuntimeSet class file has been created."; } }

        protected override void DisplayTextAndOptions()
        {
            GUILayout.Label("Please select whether your new RuntimeSet should store a single component\n"
                + "or multiple components for each GameObject.");

            GUILayout.BeginHorizontal();
            singleComponent = GUILayout.Toggle(singleComponent, "Single component");
            singleComponent = !GUILayout.Toggle(!singleComponent, "Multiple components");
            GUILayout.EndHorizontal();

            if (singleComponent)
            {
                GUILayout.Label("Please enter the name of the component\n"
                    + "that the RuntimeSet is supposed to reference for each GameObject.\n"
                    + "Remember to be case-sensitive.");

                className = "";
            }
            else
            {
                GUILayout.Label("Next, please complete the class name for your new RuntimeSet class.\n"
                    + "Example: MeshRendererAndFilter");

                className = EditorGUILayout.TextField("Class Name: RuntimeSet", className);

                GUILayout.Label("Now, please enter all the component types that are to be references by your new RuntimeSet.\n"
                    + "Remember to be case-sensitive.\n"
                    + "Example: MeshRenderer MeshFilter");
            }
        }

        protected override bool IsValidInput(string input)
        {
            if (singleComponent)
            {
                return Regex.IsMatch(input, @"\w+");
            }
            else
            {
                return Regex.IsMatch(input, @"(\w+)( \w+)+") &&
                       Regex.IsMatch(className, @"\w+");
            }
        }

        protected override string GenerateFilename(string typeName)
        {
            return "RuntimeSet" + (singleComponent ? typeName : className) + ".cs";
        }

        protected override string ParseTemplate(string content, string input)
        {
            if (singleComponent)
            {
                content = ParseTemplateForSingleComponent(content, input);
            }
            else
            {
                var inputParts = input.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

                if (inputParts.Length <= 1)
                {
                    throw new System.Exception("This should not happen.");
                }
                else
                {
                    content = ParseTemplateForMultiComponent(content, className, inputParts);
                }
            }
            
            return content;
        }

        private string ParseTemplateForSingleComponent(string content, string typeName)
        {
            content = ReplaceDependingOnMode(content, true);
            content = content.Replace("#TYPE#", typeName);
            return content;
        }

        private string ParseTemplateForMultiComponent(string content, string className, string[] typeNames)
        {
            content = ReplaceDependingOnMode(content, false);
            content = content.Replace("#TYPE#", className);

            string[] fieldNames = GenerateFieldNamesForTypes(typeNames);

            var allFieldNamesConcatenatedWithAnd = "";
            for (var i = 0; i < fieldNames.Length; i++)
            {
                var firstItem = i == 0;
                if (!firstItem)
                {
                    allFieldNamesConcatenatedWithAnd += " && ";
                }
                allFieldNamesConcatenatedWithAnd += fieldNames[i];
            }

            content = Regex.Replace(content, @"#ALL_CNAMES#", allFieldNamesConcatenatedWithAnd);

            // Replace all #PER_COMPONENT lines with a block of one line per passed class name.
            // In each line, replace occurrences of #CTYPE# with the current type
            // and all occurrences of #CNAME# with a variable name generated from that type.
            content = Regex.Replace(content, @"( *?#PER_COMPONENT .+?)\r?\n", match =>
            {
                var templateLine = match.Value;
                templateLine = templateLine.Replace("#PER_COMPONENT ", "");

                var result = "";
                for (var i = 0; i < typeNames.Length; i++)
                {
                    var line = templateLine;

                    line = line.Replace("#CTYPE#", typeNames[i]);
                    line = line.Replace("#CNAME#", fieldNames[i]);

                    result += line;
                }
                return result;
            });

            return content;
        }

        private static string ReplaceDependingOnMode(string content, bool singleComponent)
        {
            content = Regex.Replace(content, @"#SINGLE_COMPONENT\?(.*?)#:#(.*?)\?#", match =>
            {
                return match.Groups[singleComponent ? 1 : 2].Value.Trim(' ');
            },
            RegexOptions.Singleline);
            return content;
        }

        private static string[] GenerateFieldNamesForTypes(string[] typeNames)
        {
            var fieldNames = new string[typeNames.Length];
            for (var i = 0; i < typeNames.Length; i++)
            {
                var typeName = typeNames[i];
                var fieldName = typeName.Substring(0, 1).ToLower() + typeName.Substring(1);
                // Just in case someone thinks class names should be lowercase...
                if (fieldName == typeName)
                {
                    fieldName = "_" + fieldName;
                }
                fieldNames[i] = fieldName;
            }

            return fieldNames;
        }
    }
}
