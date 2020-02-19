// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;

    /// <summary>
    /// A MonoBehaviour used to add a GameObject to a RuntimeSet.
    /// The element is only part of the RuntimeSet while it's active.
    /// </summary>
    [AddComponentMenu("Soda/RuntimeSet Element")]
    public class RuntimeSetElement : MonoBehaviour
    {
        [Tooltip("The RuntimeSet to add this GameObject to.")]
        [SerializeField]
        [DisplayInsteadInPlaymode("runtimeSet")]
        private RuntimeSetBase _runtimeSet;
        public RuntimeSetBase runtimeSet
        {
            get
            {
                return _runtimeSet;
            }
            set
            {
                if(value == _runtimeSet) return;

                if(_runtimeSet && enabled)
                {
                    _runtimeSet.Remove(gameObject);
                }

                _runtimeSet = value;

                if(_runtimeSet && enabled)
                {
                    var couldAdd = _runtimeSet.Add(gameObject);
                    if (!couldAdd)
                    {
                        enabled = false;
                    }
                }
            }
        }

        private void OnEnable()
        {
            if(runtimeSet)
            {
                var couldAdd = runtimeSet.Add(gameObject);
                if (!couldAdd)
                {
                    enabled = false;
                }
            }
            else
            {
                Debug.LogError("This RuntimeSetElement does not have a RuntimeSet assigned.", this);
                enabled = false;
            }
        }

        private void OnDisable()
        {
            if(runtimeSet)
            {
                runtimeSet.Remove(gameObject);
            }
        }
    }
}
