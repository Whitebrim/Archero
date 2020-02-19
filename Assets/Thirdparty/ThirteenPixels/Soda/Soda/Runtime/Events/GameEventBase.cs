// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    /// <summary>
    /// A ScriptableObject representing a global event.
    /// Base class for GameEvents with and without parameter.
    /// </summary>
    public abstract class GameEventBase : ScriptableObject
    {
        #region Description
#pragma warning disable 0414
        [TextArea]
        [SerializeField]
        private string description = "";
#pragma warning restore 0414
        #endregion

#if UNITY_EDITOR
        [NonSerialized]
        public bool debug;
#endif
        /// <summary>
        /// This event is invoked when the GameEvent is raised.
        /// For parameterized GameEvents, use this event to register responses that ignore the parameter.
        /// </summary>
        public readonly SodaEvent onRaise = new SodaEvent();
        // For preventing cyclic/recursive invocation
        protected bool currentlyBeingRaised = false;

        internal virtual Type GetParameterType()
        {
            return null;
        }
    }

    /// <summary>
    /// A ScriptableObject representing a global event with a parameter.
    /// </summary>
    public abstract class GameEventBase<T> : GameEventBase
    {
        public readonly SodaEvent<T> onRaiseWithParameter = new SodaEvent<T>();
        protected abstract UnityEvent<T> onRaiseGlobally { get; }

        #region Parameter Description
#pragma warning disable 0414
        [TextArea]
        [SerializeField]
        private string parameterDescription = "";
#pragma warning restore 0414
        #endregion

        /// <summary>
        /// Call this method to raise the event, leading to all added responses being invoked.
        /// </summary>
        public void Raise(T parameter)
        {
#if UNITY_EDITOR
            if (debug)
            {
                Debug.Log("GameEvent \"" + name + "\" was raised.\n", this);
            }
#endif
            if (currentlyBeingRaised)
            {
                Debug.LogWarning("Event is already being raised, preventing recursive raise.", this);
                return;
            }

            currentlyBeingRaised = true;

            try
            {
                onRaiseGlobally.Invoke(parameter);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            onRaiseWithParameter.Invoke(parameter);
            onRaise.Invoke();

            currentlyBeingRaised = false;
        }

        internal override Type GetParameterType()
        {
            return typeof(T);
        }
    }
}
