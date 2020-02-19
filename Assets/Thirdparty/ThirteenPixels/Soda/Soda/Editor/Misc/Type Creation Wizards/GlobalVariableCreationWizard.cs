// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System.Text.RegularExpressions;

    /// <summary>
    /// This wizard creates a new GlobalVariable type for representing the type you enter.
    /// The class comes with a matching ScopedVariable type and Editor classes for both.
    /// </summary>
    public class GlobalVariableCreationWizard : TypeCreationWizardBase
    {
        [MenuItem("Window/Soda/Create/GlobalVariable Type")]
        private static void Open()
        {
            OpenWizard<GlobalVariableCreationWizard>("Soda GlobalVariable Type Creation Wizard");
        }

        protected override string templatePath { get { return "GlobalVariableTemplate.cs"; } }
        protected override string successMessage { get { return "The GlobalVariable class file has been created."; } }

        protected override void DisplayTextAndOptions()
        {
            GUILayout.Label("Please enter the type you want to generate a GlobalVariable type for.\n"
                + "Remember to be case-sensitive.");

            GUI.FocusControl("TextField");
        }

        protected override bool IsValidInput(string input)
        {
            return Regex.IsMatch(input, "\\w+");
        }

        protected override string GenerateFilename(string typeName)
        {
            return "Global" + typeName + ".cs";
        }

        protected override string ParseTemplate(string content, string input)
        {
            content = content.Replace("#TYPE#", input);
            return content;
        }
    }
}
