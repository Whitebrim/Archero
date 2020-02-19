// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;

    [System.Serializable]
    public class ScopedVector3 : ScopedVariableBase<Vector3, GlobalVector3>
    {
        public ScopedVector3(Vector3 value) : base(value)
        {
        }
    }
}
