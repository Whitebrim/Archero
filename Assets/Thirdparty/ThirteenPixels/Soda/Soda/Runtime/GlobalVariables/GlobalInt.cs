// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;

    /// <summary>
    /// A GlobalVariable representing an integer.
    /// </summary>
    [CreateAssetMenu(menuName = "Soda/GlobalVariable/Int", order = 20)]
    public class GlobalInt : GlobalVariableBase<int>
    {
        /// <summary>
        /// For incrementing or decrementing the value via UnityEvent.
        /// </summary>
        public void Increment(int amount)
        {
            if(amount != 0)
            {
                value += amount;
            }
        }

        public override void LoadValue(ISavestateReader reader, string key)
        {
            value = reader.GetInt(key);
        }

        public override void SaveValue(ISavestateWriter writer, string key)
        {
            writer.SetInt(key, value);
        }
    }
}
