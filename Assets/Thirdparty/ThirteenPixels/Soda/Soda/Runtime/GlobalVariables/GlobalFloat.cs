// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;

    /// <summary>
    /// A GlobalVariable representing a float.
    /// </summary>
    [CreateAssetMenu(menuName = "Soda/GlobalVariable/Float", order = 30)]
    public class GlobalFloat : GlobalVariableBase<float>
    {
        /// <summary>
        /// For incrementing or decrementing the value via UnityEvent.
        /// </summary>
        public void Increment(float amount)
        {
            if(amount != 0)
            {
                value += amount;
            }
        }

        public override void LoadValue(ISavestateReader reader, string key)
        {
            value = reader.GetFloat(key);
        }

        public override void SaveValue(ISavestateWriter writer, string key)
        {
            writer.SetFloat(key, value);
        }
    }
}
