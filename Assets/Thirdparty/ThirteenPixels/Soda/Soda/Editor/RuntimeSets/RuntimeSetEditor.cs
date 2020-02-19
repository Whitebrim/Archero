// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;

    [CustomEditor(typeof(RuntimeSetBase), editorForChildClasses: true)]
    public class RuntimeSetEditor : Editor
    {
        private IEnumerable<GameObject> gameObjectsInRuntimeSet;
        private int gameObjectCountInRuntimeSet;
        private bool initialized = false;

        // Using a lazy initialization here because some Unity versions like to instantiate two instances of Editor classes.
        // This way, only the instance that is actually being drawn will subscribe to the runtimeSet's event and can be filtered out in the list.
        private void LazyOnEnable()
        {
            if (!initialized)
            {
                var runtimeSetTarget = (RuntimeSetBase)target;
                runtimeSetTarget.onElementCountChange.AddResponseAndInvoke(UpdateListOfGameObjectsToDisplay);
                initialized = true;
            }
        }

        private void OnDisable()
        {
            if (initialized)
            {
                var runtimeSetTarget = (RuntimeSetBase)target;
                runtimeSetTarget.onElementCountChange.RemoveResponse(UpdateListOfGameObjectsToDisplay);
                initialized = false;
            }
        }

        private void UpdateListOfGameObjectsToDisplay(int elementCount)
        {
            gameObjectCountInRuntimeSet = elementCount;
            var runtimeSetTarget = (RuntimeSetBase)target;
            gameObjectsInRuntimeSet = runtimeSetTarget.GetAllGameObjects();
            EditorApplication.delayCall += Repaint;
        }

        public override void OnInspectorGUI()
        {
            LazyOnEnable();
            
            SodaEditorHelpers.DisplayInspectorSubtitle(target.GetType().Name);

            var descriptionProperty = serializedObject.FindProperty("description");

            serializedObject.Update();
            serializedObject.DisplayAllPropertiesExcept(false, descriptionProperty);
            EditorGUILayout.PropertyField(descriptionProperty);
            serializedObject.ApplyModifiedProperties();

            if(Application.isPlaying)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Monitoring Objects");
                SodaEventDrawer.DisplayListeners(((RuntimeSetBase)target).GetOnElementCountChangeEvent(), this);

                EditorGUILayout.Space();
                DisplayElements();
            }
        }

        private void DisplayElements()
        {
            EditorGUILayout.LabelField("Elements in this RuntimeSet");

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(20);

                if (gameObjectCountInRuntimeSet == 0)
                {
                    GUILayout.Label("None");
                }
                else
                {
                    GUILayout.BeginVertical(GUILayout.Width(Screen.width - 100));

                    GameObject elementToDestroy = null;
                    foreach (var gameObject in gameObjectsInRuntimeSet)
                    {
                        GUILayout.BeginHorizontal();
                        if(GUILayout.Button("Select", GUILayout.Width(100)))
                        {
                            Selection.activeGameObject = gameObject;
                        }
                        GUILayout.Label(gameObject.name);
                        if(GUILayout.Button("Destroy", GUILayout.Width(100)))
                        {
                            elementToDestroy = gameObject;
                        }
                        GUILayout.EndHorizontal();
                    }

                    if(elementToDestroy)
                    {
                        Destroy(elementToDestroy);
                    }

                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndHorizontal();
        }
    }
    

}
