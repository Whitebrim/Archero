// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    /// <summary>
    /// Provides operations for serializing data to any target.
    /// </summary>
    public interface ISavestateWriter
    {
        void PreSave();

        void SetBool(string key, bool value);
        void SetInt(string key, int value);
        void SetFloat(string key, float value);
        void SetString(string key, string value);
        void SetComposed(string key, params object[] objs);
        void SetComposed(string key, string delimiter, params object[] objs);

        void PostSave();
    }
}
