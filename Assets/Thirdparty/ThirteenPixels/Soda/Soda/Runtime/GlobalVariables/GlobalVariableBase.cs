// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;
    using System;

    /// <summary>
    /// Base class for GlobalVariables.
    /// Each GlobalVariable represents a single global variable stored in an injectable ScriptableObject.
    /// </summary>
    public abstract class GlobalVariableBase<T> : GlobalVariableBase, ISerializationCallbackReceiver
    {
        [SerializeField]
        [DisplayInsteadInPlaymode("value", tooltip = "The value this object currently has.")]
        private T originalValue = default;
        private T _value;
        /// <summary>
        /// The value this GlobalVariable represents.
        /// </summary>
        public virtual T value
        {
            get
            {
                return _value;
            }
            set
            {
                if (Equals(_value, value)) return;

                _value = value;
                onChange.Invoke(value);
            }
        }

        #region Description
        #pragma warning disable 0414
        [TextArea]
        [SerializeField]
        private string description = "";
#pragma warning restore 0414
        #endregion
        
        private SodaEvent<T> _onChange;
        /// <summary>
        /// This event is invoked when the value changes.
        /// </summary>
        public SodaEvent<T> onChange
        {
            get
            {
                if (_onChange == null)
                {
                    _onChange = new SodaEvent<T>(() => value);
                }
                return _onChange;
            }
        }
        internal override sealed SodaEventBase GetOnChangeEvent()
        {
            return onChange;
        }

        private void Awake()
        {
            hideFlags = HideFlags.DontUnloadUnusedAsset;
        }
        
        public void OnAfterDeserialize()
        {
            _value = originalValue;
        }

        public void OnBeforeSerialize() { }

        /// <summary>
        /// For copying the value from another GlobalVariable via UnityEvent.
        /// The passed GlobalVariable has to be of the same type as this one.
        /// </summary>
        public void CopyValue(GlobalVariableBase other)
        {
            var otherGlobalVariable = other as GlobalVariableBase<T>;
            if (otherGlobalVariable != null)
            {
                value = otherGlobalVariable.value;
            }
            else
            {
                Debug.LogError("Can only copy the value from another ValueObject of the same type.");
            }
        }
    }

    /// <summary>
    /// Base class for GlobalVariables.
    /// This non-generic version allows other classes to use GlobalVariables regardless of their generic type, which used for the savestate system.
    /// </summary>
    public abstract class GlobalVariableBase : ScriptableObject
    {
        /// <summary>
        /// Loads the value for this GlobalVariable by using the provided ISavesateReader and key.
        /// Override this in a GlobalVariable class to allow it to do so.
        /// </summary>
        public virtual void LoadValue(ISavestateReader reader, string key)
        {
            throw new NotSupportedException("The GlobalVariable class \"" + GetType() + "\" does not support value loading.");
        }

        /// <summary>
        /// Saves the value of this GlobalVariable by using the provided ISavesateWriter and key.
        /// Override this in a GlobalVariable class to allow it to do so.
        /// </summary>
        public virtual void SaveValue(ISavestateWriter writer, string key)
        {
            throw new NotSupportedException("The GlobalVariable class \"" + GetType() + "\" does not support value saving.");
        }

#if UNITY_EDITOR
        public static implicit operator bool(GlobalVariableBase valueObject)
        {
            Debug.LogWarning("You are implicitly converting a GlobalVariable to a bool. Please use an explicit != null instead to avoid confusion with reading the value property.", valueObject);
            return valueObject != null;
        }
#endif
        
        internal abstract SodaEventBase GetOnChangeEvent();
    }
}
