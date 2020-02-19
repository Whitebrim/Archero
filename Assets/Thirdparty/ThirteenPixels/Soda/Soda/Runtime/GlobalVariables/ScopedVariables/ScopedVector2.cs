// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;

    [System.Serializable]
    public class ScopedVector2 : ScopedVariableBase<Vector2, GlobalVector2>
    {
        public ScopedVector2(Vector2 value) : base(value)
        {
        }
    }
}
