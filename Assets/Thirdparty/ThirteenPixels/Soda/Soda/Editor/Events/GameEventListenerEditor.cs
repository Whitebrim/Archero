// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(GameEventListener))]
    public class GameEventListenerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Don't draw the default inspector in order to hide the "Script" property line
            serializedObject.Update();
            
            var gameEventProperty = serializedObject.FindProperty("_gameEvent");

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(gameEventProperty, new GUIContent("Game Event"));
            if(EditorGUI.EndChangeCheck() && Application.isPlaying)
            {
                var listener = (GameEventListener)target;
                listener.gameEvent = (GameEvent)gameEventProperty.objectReferenceValue;
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("response"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
