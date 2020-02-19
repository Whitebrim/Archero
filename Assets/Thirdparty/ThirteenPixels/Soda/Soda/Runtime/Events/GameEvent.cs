// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    /// <summary>
    /// A ScriptableObject representing a global event.
    /// </summary>
    [CreateAssetMenu(menuName = "Soda/GameEvent/Simple", order = 299)]
    public class GameEvent : GameEventBase
    {
        [SerializeField]
        private UnityEvent onRaiseGlobally = default;

        /// <summary>
        /// Call this method to raise the event, leading to all added responses being invoked.
        /// </summary>
        public void Raise()
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
                onRaiseGlobally.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            onRaise.Invoke();

            currentlyBeingRaised = false;
        }
    }
}
