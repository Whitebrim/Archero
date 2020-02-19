// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    [System.Serializable]
    public class ScopedString : ScopedVariableBase<string, GlobalString>
    {
        public ScopedString(string value) : base(value)
        {
        }
    }
}
