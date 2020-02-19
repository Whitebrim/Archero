// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    /// <summary>
    /// Provides operations for deserializing data from any source.
    /// </summary>
    public interface ISavestateReader
    {
        void PreLoad();

        bool HasKey(string key);
        bool GetBool(string key);
        int GetInt(string key);
        float GetFloat(string key);
        string GetString(string key);
        T[] GetComposed<T>(string key);
        T[] GetComposed<T>(string key, string delimiter);

        void PostLoad();
    }
}
