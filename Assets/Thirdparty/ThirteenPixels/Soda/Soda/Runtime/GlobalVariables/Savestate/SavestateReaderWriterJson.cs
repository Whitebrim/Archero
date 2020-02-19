// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Linq;

    /// <summary>
    /// Example implementation of the Savestate-related interfaces, saving and loading values using Json encoding.
    /// Feel free to use this class, but be aware that it is not tested much.
    /// For production, a more sophisticated class, perhaps using a dedicated Json library, is recommended.
    /// </summary>
    public class SavestateReaderWriterJson : ISavestateWriter, ISavestateReader
    {
        private const string defaultDelimiter = "&&";
        private static readonly string[] delimiters = new string[] { defaultDelimiter } ;

        private readonly string path;
        private readonly Dictionary<string, string> entries = new Dictionary<string, string>();

        private readonly Regex jsonLine = new Regex(" *\"(.+)\" *: *\"(.+)\" *,?");


        public SavestateReaderWriterJson(string path)
        {
            this.path = path;
        }

        public void PreLoad()
        {
            entries.Clear();

            var lines = new List<string>(File.ReadAllLines(path));
            lines.RemoveAt(0);
            lines.RemoveAt(lines.Count - 1);
            foreach (var line in lines)
            {
                try
                {
                    var match = jsonLine.Match(line);
                    var key = match.Groups[1].Value;
                    var value = match.Groups[2].Value;

                    entries.Add(key, value);
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        public void PostLoad()
        {
            entries.Clear();
        }

        public bool GetBool(string key)
        {
            return entries[key] == "1";
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
            return float.Parse(entries[key]);
        }

        public int GetInt(string key)
        {
            return int.Parse(entries[key]);
        }

        public string GetString(string key)
        {
            return entries[key];
        }

        public bool HasKey(string key)
        {
            return entries.ContainsKey(key);
        }

        public void PreSave()
        {
            entries.Clear();
        }

        public void PostSave()
        {
            var entryCount = 0;

            using (StreamWriter file = new StreamWriter(path))
            {
                file.WriteLine("{");

                foreach (var entry in entries)
                {
                    var line = "  \"" + entry.Key + "\": \"" + entry.Value + "\"";
                    if (entryCount < entries.Count - 1)
                    {
                        line += ",";
                    }
                    entryCount++;

                    file.WriteLine(line);
                }

                file.WriteLine("}");
            }
        }

        private static string FormatStringForJson(string s)
        {
            return s.Replace("\"", "\\\"");
        }

        public void SetBool(string key, bool value)
        {
            entries.Add(key, value ? "1" : "0");
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
            SetString(key, value + "");
        }

        public void SetInt(string key, int value)
        {
            SetString(key, value + "");
        }

        public void SetString(string key, string value)
        {
            entries.Add(key, value);
        }
    }
}
