// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;

    /// <summary>
    /// A GlobalVariable representing a Color.
    /// </summary>
    [CreateAssetMenu(menuName = "Soda/GlobalVariable/Color", order = 70)]
    public class GlobalColor : GlobalVariableBase<Color>
    {
        public override void LoadValue(ISavestateReader reader, string key)
        {
            var arr = reader.GetComposed<float>(key);
            value = new Color(arr[0], arr[1], arr[2], arr[3]);
        }

        public override void SaveValue(ISavestateWriter writer, string key)
        {
            writer.SetComposed(key, value.r, value.g, value.b, value.a);
        }
    }
}
