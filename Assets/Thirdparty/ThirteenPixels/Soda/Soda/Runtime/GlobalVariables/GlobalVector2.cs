// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;

    /// <summary>
    /// A GlobalVariable representing a Vector2.
    /// </summary>
    [CreateAssetMenu(menuName = "Soda/GlobalVariable/Vector2", order = 50)]
    public class GlobalVector2 : GlobalVariableBase<Vector2>
    {
        public override void LoadValue(ISavestateReader reader, string key)
        {
            var arr = reader.GetComposed<float>(key);
            value = new Vector2(arr[0], arr[1]);
        }

        public override void SaveValue(ISavestateWriter writer, string key)
        {
            writer.SetComposed(key, value.x, value.y);
        }
    }
}
