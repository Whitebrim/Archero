// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    [System.Serializable]
    public class ScopedInt : ScopedVariableBase<int, GlobalInt>
    {
        public ScopedInt(int value) : base(value)
        {
        }
    }
}
