// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using System;
    using UnityEngine;
    using UnityEngine.Serialization;

    /// <summary>
    /// Base class for ScopedVariable types.
    /// A ScopedVariable is a type that allows to create a field that either contains a local value
    /// or a GlobalVariable that represents a value of the same type.
    /// </summary>
    /// <typeparam name="T">The type of the value being represented.</typeparam>
    /// <typeparam name="GVT">The GlobalVariable type that can be referenced to represent the value.</typeparam>
    public abstract class ScopedVariableBase<T, GVT> : ScopedVariableBase where GVT : GlobalVariableBase<T>
    {
        [FormerlySerializedAs("valueObject")]
        [FormerlySerializedAs("_valueObject")]
        [SerializeField]
        private GVT globalVariable = default;
        [FormerlySerializedAs("_localValue")]
        [SerializeField]
        private T localValue = default;
        [FormerlySerializedAs("_useLocal")]
        [SerializeField]
        private bool useLocal = false;

        public T value
        {
            get { return useLocal ? localValue : (globalVariable != null ? globalVariable.value : default(T)); }
            set
            {
                if (useLocal)
                {
                    if (!localValue.Equals(value))
                    {
                        localValue = value;
                        InvokeOnChangeEvent(value);
                    }
                }
                else if (globalVariable != null)
                {
                    globalVariable.value = value;
                }
                else
                {
                    throw new NullReferenceException("Trying to set a value to a GlobalVariable object, but there is none referenced.");
                }
            }
        }

        public ScopedVariableBase()
        {
        }

        public ScopedVariableBase(T value)
        {
            localValue = value;
            useLocal = true;
        }

        #region OnChangeValue event
        private SodaEvent<T> _onChangeValue;
        /// <summary>
        /// This event is invoked when the value changes, be it the local value, the value of the referenced GlobalVariable
        /// or when switching between local value and GlobalVariable.
        /// </summary>
        public SodaEvent<T> onChangeValue
        {
            get
            {
                if (_onChangeValue == null)
                {
                    _onChangeValue = new SodaEvent<T>(() => value, UpdateGlobalVariableMonitoring);
                }
                return _onChangeValue;
            }
        }

        internal override void UpdateGlobalVariableMonitoring()
        {
            if (globalVariable != null)
            {
                if (hasOnChangeResponses && !useLocal)
                {
                    globalVariable.onChange.AddResponse(InvokeOnChangeEvent);
                }
                else
                {
                    globalVariable.onChange.RemoveResponse(InvokeOnChangeEvent);
                }
            }
        }

        private void InvokeOnChangeEvent(T value)
        {
            onChangeValue.Invoke(value);
        }

        internal override void InvokeOnChangeEvent()
        {
            InvokeOnChangeEvent(value);
        }

        private bool hasOnChangeResponses
        {
            get { return onChangeValue.responseCount > 0; }
        }
        #endregion

        /// <summary>
        /// Assign a GlobalVariable to be referenced by this ScopedVariable.
        /// </summary>
        public void AssignGlobalVariable(GVT globalVariable)
        {
            this.globalVariable = globalVariable;
            useLocal = false;

            UpdateGlobalVariableMonitoring();
            InvokeOnChangeEvent();
        }

        /// <summary>
        /// Assign a local value to this ScopedVariable.
        /// Any assigned GlobalVariable will be ignored, and the value property will represent this local value.
        /// </summary>
        public void AssignLocalValue(T value)
        {
            localValue = value;
            useLocal = true;

            UpdateGlobalVariableMonitoring();

            globalVariable = null;

            InvokeOnChangeEvent();
        }
    }

    /// <summary>
    /// Base class for ScopedVariable types.
    /// A ScopedVariable is a type that allows to create a field that either contains a local value
    /// or a GlobalVariable that represents a value of the same type.
    /// </summary>
    public abstract class ScopedVariableBase
    {
        internal abstract void InvokeOnChangeEvent();
        internal abstract void UpdateGlobalVariableMonitoring();
    }
}
