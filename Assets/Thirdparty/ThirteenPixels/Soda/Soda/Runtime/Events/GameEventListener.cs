// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// A MonoBehaviour listening to a specific GameEvent.
    /// Whenever the GameEvent is raised, the response UnityEvent is invoked.
    /// </summary>
    [AddComponentMenu("Soda/GameEvent Listener")]
    public class GameEventListener : MonoBehaviour
    {
        [Tooltip("The event to react upon.")]
        [SerializeField]
        private GameEventBase _gameEvent;
        public GameEventBase gameEvent
        {
            get
            {
                return _gameEvent;
            }
            set
            {
                if(_gameEvent == value) return;

                if(enabled && _gameEvent)
                {
                    _gameEvent.onRaise.RemoveResponse(OnEventRaised);
                }

                _gameEvent = value;

                if(enabled && _gameEvent)
                {
                    _gameEvent.onRaise.AddResponse(OnEventRaised);
                }
            }
        }

        [Space]
        [Tooltip("Response to invoke when the event is raised.")]
        public UnityEvent response;


        private void OnEnable()
        {
            if(gameEvent)
            {
                gameEvent.onRaise.AddResponse(OnEventRaised);
            }
        }

        private void OnDisable()
        {
            if(gameEvent)
            {
                gameEvent.onRaise.RemoveResponse(OnEventRaised);
            }
        }

        internal void OnEventRaised()
        {
            response.Invoke();
        }
    }
}
