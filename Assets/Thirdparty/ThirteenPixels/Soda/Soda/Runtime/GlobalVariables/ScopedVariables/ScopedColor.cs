// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;

    [System.Serializable]
    public class ScopedColor : ScopedVariableBase<Color, GlobalColor>
    {
        public ScopedColor(Color value) : base(value)
        {
        }
    }
}
