// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;
    using System.Linq;

    /// <summary>
    /// Example implementation of the Savestate-related interfaces, saving and loading values using Unity's PlayerPrefs.
    /// </summary>
    public class SavestateReaderWriterPlayerPrefs : ISavestateWriter, ISavestateReader
    {
        private const string defaultDelimiter = "&&";
        private static readonly string[] delimiters = new string[] { defaultDelimiter } ;

        public bool GetBool(string key)
        {
            return PlayerPrefs.GetInt(key, 0) != 0;
        }

        public T[] GetComposed<T>(string key)
        {
            return GetComposed<T>(key, defaultDelimiter);
        }

        public T[] GetComposed<T>(string key, string delimiter)
        {
            var s = GetString(key);
            delimiters[0] = delimiter;
            var values = s.Split(delimiters, System.StringSplitOptions.None);
            return values.Cast<T>().ToArray();
        }

        public float GetFloat(string key)
        {
            return PlayerPrefs.GetFloat(key);
        }

        public int GetInt(string key)
        {
            return PlayerPrefs.GetInt(key);
        }

        public string GetString(string key)
        {
            return PlayerPrefs.GetString(key);
        }

        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public void PostLoad() { }

        public void PostSave()
        {
            PlayerPrefs.Save();
        }

        public void PreLoad() { }

        public void PreSave() { }

        public void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }

        public void SetComposed(string key, params object[] objs)
        {
            SetComposed(key, defaultDelimiter, objs);
        }

        public void SetComposed(string key, string delimiter, params object[] objs)
        {
            for (var i = 0; i < objs.Length; i++)
            {
                objs[i] = objs[i].ToString();
            }
            SetString(key, string.Join(delimiter, (string[])objs));
        }

        public void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        public void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }
    }
}
