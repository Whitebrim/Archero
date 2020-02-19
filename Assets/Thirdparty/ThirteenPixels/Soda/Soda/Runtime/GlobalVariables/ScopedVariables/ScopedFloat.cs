// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    [System.Serializable]
    public class ScopedFloat : ScopedVariableBase<float, GlobalFloat>
    {
        public ScopedFloat(float value) : base(value)
        {
        }
    }
}
