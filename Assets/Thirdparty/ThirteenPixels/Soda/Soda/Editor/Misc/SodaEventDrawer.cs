// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using UnityEngine;
    using UnityEditor;

    /// <summary>
    /// Draws listeners registered to a SodaEvent.
    /// </summary>
    internal static class SodaEventDrawer
    {
        private const int maxListenerDisplayCount = 50;
        private static readonly object[] listenerBuffer = new object[maxListenerDisplayCount];

        /// <summary>
        /// Displays the listeners of the provided SodaEvent.
        /// </summary>
        /// <param name="sodaEvent">The SodaEvent to display the listeners of.</param>
        /// <param name="except">An optional object that will not be displayed. Intended for the editor instance that draws the listeners.</param>
        public static void DisplayListeners(SodaEventBase sodaEvent, Object except = null)
        {
            try
            {
                var count = sodaEvent.GetListeners(listenerBuffer);
                DisplayListeners(listenerBuffer, count, except);
            }
            catch
            {
                EditorGUILayout.HelpBox("An error occurred while drawing the listeners.", MessageType.Error);
                return;
            }
        }

        private static void DisplayListeners(object[] listeners, int count, object except = null)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(20);

                var displayed = false;
                
                GUILayout.BeginVertical(GUILayout.Width(Screen.width - 100));
                {
                    for (var i = 0; i < count; i++)
                    {
                        var listener = listeners[i];
                        if (listener != except)
                        {
                            GUILayout.BeginHorizontal();
                            if (listener is Object)
                            {
                                var listenerObject = (Object)listener;
                                if (GUILayout.Button("Select", GUILayout.Width(100)))
                                {
                                    Selection.activeObject = listenerObject;
                                }
                                GUILayout.Label(listenerObject.name + " (" + listenerObject.GetType().Name + ")");
                            }
                            else
                            {
                                GUILayout.Space(100);
                                GUILayout.Label(listener.GetType().Name);
                            }
                            GUILayout.EndHorizontal();
                            displayed = true;
                        }
                    }
                }
                if (count == listeners.Length)
                {
                    EditorGUILayout.HelpBox("There might be more listeners after this.", MessageType.Info);
                }
                GUILayout.EndVertical();

                if (!displayed)
                {
                    GUILayout.Label("None");
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}
