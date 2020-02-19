// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    [System.Serializable]
    public class ScopedBool : ScopedVariableBase<bool, GlobalBool>
    {
        public ScopedBool(bool value) : base(value)
        {
        }
    }
}
