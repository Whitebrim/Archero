// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;

    // This class serves as a (very usable) example for extending a GlobalVariable class to add some additional semantics.
    /// <summary>
    /// A GlobalInt variant that has a maximum value, clamping the represented int value.
    /// </summary>
    [CreateAssetMenu(menuName = "Soda/GlobalVariable/Int with Maximum", order = 200)]
    public class GlobalIntWithMaximum : GlobalInt, ISerializationCallbackReceiver
    {
        [DisplayInsteadInPlaymode("maximum")]
        [SerializeField]
        private int originalMaximum = 100;
        private int _maximum;
        /// <summary>
        /// The maximum value this GlobalVariable's int value can have.
        /// Changing this might change that value.
        /// </summary>
        public int maximum
        {
            get { return _maximum; }
            set
            {
                _maximum = value;

                if (this.value > _maximum)
                {
                    this.value = _maximum;
                }
            }
        }

        /// <summary>
        /// The value this GlobalVariable represents.
        /// </summary>
        public override int value
        {
            get { return base.value; }
            set
            {
                if (value > _maximum)
                {
                    value = _maximum;
                }
                base.value = value;
            }
        }

        new public void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            maximum = originalMaximum;
        }
    }
}
