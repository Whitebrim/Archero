// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;

    /// <summary>
    /// A GlobalVariable representing a Vector3.
    /// </summary>
    [CreateAssetMenu(menuName = "Soda/GlobalVariable/Vector3", order = 60)]
    public class GlobalVector3 : GlobalVariableBase<Vector3>
    {
        public override void LoadValue(ISavestateReader reader, string key)
        {
            var arr = reader.GetComposed<float>(key);
            value = new Vector3(arr[0], arr[1], arr[2]);
        }

        public override void SaveValue(ISavestateWriter writer, string key)
        {
            writer.SetComposed(key, value.x, value.y, value.z);
        }
    }
}
