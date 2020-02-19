// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using UnityEngine;
    using UnityEditor;
    
    [CustomEditor(typeof(GameEventBase), editorForChildClasses: true)]
    public class GameEventEditor : Editor
    {
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
            SodaEditorHelpers.DisplayInspectorSubtitle("GameEvent");

            var gameEventBaseTarget = (GameEventBase)target;

            var descriptionProperty = serializedObject.FindProperty("description");
            var parameterDescriptionProperty = serializedObject.FindProperty("parameterDescription");
            var onRaiseGloballyProperty = serializedObject.FindProperty("onRaiseGlobally");
            if (onRaiseGloballyProperty == null)
            {
                onRaiseGloballyProperty = serializedObject.FindProperty("_onRaiseGlobally");
            }

            serializedObject.Update();
            if (parameterDescriptionProperty != null)
            {
                serializedObject.DisplayAllPropertiesExcept(false, descriptionProperty, parameterDescriptionProperty, onRaiseGloballyProperty);
            }
            else
            {
                serializedObject.DisplayAllPropertiesExcept(false, descriptionProperty, onRaiseGloballyProperty);
            }

            EditorGUILayout.PropertyField(descriptionProperty);
            if (parameterDescriptionProperty != null)
            {
                EditorGUILayout.PropertyField(parameterDescriptionProperty,
                                              new GUIContent("Parameter Description (" + gameEventBaseTarget.GetParameterType().Name + ")"));
            }
            GUILayout.Space(16);
            
            GUI.enabled = !Application.isPlaying;
            EditorGUILayout.PropertyField(onRaiseGloballyProperty);
            GUI.enabled = true;

            DisplayDebugCheckbox();
            serializedObject.ApplyModifiedProperties();

            GUI.enabled = Application.isPlaying;
            
            var gameEventTarget = target as GameEvent;
            if (!gameEventTarget)
            {
                EditorGUILayout.HelpBox("Cannot raise a parameterized GameEvent via inspector.", MessageType.Info);
                GUI.enabled = false;
            }

            if (GUILayout.Button("Raise"))
            {
                gameEventTarget.Raise();
            }
            GUI.enabled = true;

            if (Application.isPlaying)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Responding Objects", EditorStyles.boldLabel);
                if (targets.Length == 1)
                {
                    var onRaiseWithParameter = GetOnRaiseWithParameterEvent(gameEventBaseTarget);
                    if (onRaiseWithParameter != null)
                    {
                        SodaEventDrawer.DisplayListeners(onRaiseWithParameter);
                        EditorGUILayout.LabelField("Responding Objects (ignoring parameter)", EditorStyles.boldLabel);
                    }
                    SodaEventDrawer.DisplayListeners(gameEventBaseTarget.onRaise);
                }
                else
                {
                    EditorGUILayout.HelpBox("Cannot display when multiple GameEvents are selected.", MessageType.Warning);
                }
            }
        }

        private static SodaEventBase GetOnRaiseWithParameterEvent(GameEventBase gameEventBaseTarget)
        {
            var flags = System.Reflection.BindingFlags.GetField |
                        System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.Public;
            var field = gameEventBaseTarget.GetType().GetField("onRaiseWithParameter", flags);
            if (field != null)
            {
                return (SodaEventBase)field.GetValue(gameEventBaseTarget);
            }
            return null;
        }

        private void DisplayDebugCheckbox()
        {
            var gameEventTarget = (GameEventBase)target;

            EditorGUILayout.HelpBox("The Debug setting is reset when entering play mode.", MessageType.Info);
            GUILayout.BeginHorizontal();
            gameEventTarget.debug = EditorGUILayout.Toggle(gameEventTarget.debug, GUILayout.Width(16));
            GUILayout.Label("Debug (Raises are logged into the console)");
            GUILayout.EndHorizontal();
        }
    }
}
