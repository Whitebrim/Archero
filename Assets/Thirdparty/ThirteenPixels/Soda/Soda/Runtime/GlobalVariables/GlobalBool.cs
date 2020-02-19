// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;

    /// <summary>
    /// A GlobalVariable representing a boolean.
    /// </summary>
    [CreateAssetMenu(menuName = "Soda/GlobalVariable/Bool", order = 10)]
    public class GlobalBool : GlobalVariableBase<bool>
    {
        public override void LoadValue(ISavestateReader reader, string key)
        {
            value = reader.GetBool(key);
        }

        public override void SaveValue(ISavestateWriter writer, string key)
        {
            writer.SetBool(key, value);
        }
    }
}
