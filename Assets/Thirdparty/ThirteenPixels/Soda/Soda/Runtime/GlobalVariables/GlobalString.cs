// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;

    /// <summary>
    /// A GlobalVariable representing a string.
    /// </summary>
    [CreateAssetMenu(menuName = "Soda/GlobalVariable/String", order = 40)]
    public class GlobalString : GlobalVariableBase<string>
    {
        public override void LoadValue(ISavestateReader reader, string key)
        {
            value = reader.GetString(key);
        }

        public override void SaveValue(ISavestateWriter writer, string key)
        {
            writer.SetString(key, value);
        }
    }
}
