// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;
    using ThirteenPixels.Soda;

    /// <summary>
    /// Represents a collection of GlobalVariable objects.
    /// By using Save() and Load(), the values of the GlobalVariable objects are saved or loaded by using the provided
    /// ISavestateWriter or ISavestateReader, respectively.
    /// </summary>
    [CreateAssetMenu(menuName = "Soda/Savestate")]
    public class Savestate : ScriptableObject
    {
        [System.Serializable]
        public struct Entry
        {
            public string key;
            public GlobalVariableBase value;
        }

        [SerializeField]
        private Entry[] entries = default;
        private bool loading;

        /// <summary>
        /// Loads the values for all GlobalVariables in the list using the provided keys.
        /// The defaultReader registered in the SavestateSettings class is used for deserialization.
        /// </summary>
        public void Load()
        {
            if (SavestateSettings.defaultReader != null)
            {
                Load(SavestateSettings.defaultReader);
            }
            else
            {
                throw new System.NullReferenceException("No defaultReader was set in SavestateSettings.");
            }
        }

        /// <summary>
        /// Loads the values for all GlobalVariables in the list using the provided keys.
        /// </summary>
        /// <param name="reader">The ISavestateReader used for deserialization.</param>
        public void Load(ISavestateReader reader)
        {
            loading = true;

            try
            {
                reader.PreLoad();
                foreach (var entry in entries)
                {
                    if (entry.key != "")
                    {
                        if (reader.HasKey(entry.key))
                        {
                            try
                            {
                                entry.value.LoadValue(reader, entry.key);
                            }
                            catch (System.NotSupportedException)
                            {
                                Debug.LogError("Trying to load value for a GlobalVariable that does not support it.");
                            }
                            catch
                            {
                                Debug.LogError("Unspecified error while trying to load value for a GlobalVariable.");
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("Trying to load Savestate with empty key.");
                    }
                }
                reader.PostLoad();
            }
            finally
            {
                loading = false;
            }
        }

        /// <summary>
        /// Saves the values of all GlobalVariables in the list, mapped to the provided keys.
        /// The defaultWriter registered in the SavestateSettings class is used for serialization.
        /// </summary>
        public void Save()
        {
            if (loading) return;

            if (SavestateSettings.defaultWriter != null)
            {
                Save(SavestateSettings.defaultWriter);
            }
            else
            {
                throw new System.NullReferenceException("No defaultWriter was set in SavestateSettings.");
            }
        }

        /// <summary>
        /// Saves the values of all GlobalVariables in the list, mapped to the provided keys.
        /// </summary>
        /// <param name="writer">The ISaveSateWriter used for serialization.</param>
        public void Save(ISavestateWriter writer)
        {
            writer.PreSave();
            foreach(var entry in entries)
            {
                if (entry.key != "")
                {
                    try
                    {
                        entry.value.SaveValue(writer, entry.key);
                    }
                    catch (System.NotSupportedException)
                    {
                        Debug.LogError("Trying to save value for a GlobalVariable that does not support it.");
                    }
                    catch
                    {
                        Debug.LogError("Unspecified error while trying to save value for a GlobalVariable.");
                    }
                }
                else
                {
                    Debug.LogError("Trying to save Savestate with empty key.");
                }
            }
            writer.PostSave();
        }
    }
}
