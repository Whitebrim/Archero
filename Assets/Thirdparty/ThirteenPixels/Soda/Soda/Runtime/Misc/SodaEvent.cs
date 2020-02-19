// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An event with a paremeter.
    /// In addition event responses, SodaEvents store the object that add the response for debugging purposes.
    /// </summary>
    /// <typeparam name="T">The parameter type.</typeparam>
    public class SodaEvent<T> : SodaEventBase<Action<T>>
    {
        private readonly Func<T> callbackParameter;
        private T invokeAgainParameter;

        public SodaEvent(Func<T> callbackParameter = null, Action onChangeResponseCollection = null) : base(onChangeResponseCollection)
        {
            this.callbackParameter = callbackParameter;
        }

        /// <summary>
        /// Add a response to be invoked with the event. The callback is immediately invoked with the current value.
        /// </summary>
        /// <param name="response">The response to invoke.</param>
        /// <returns>True if the response could be added, false if it was already added before.</returns>
        public bool AddResponseAndInvoke(Action<T> response)
        {
            if (callbackParameter == null)
            {
                throw new Exception("Cannot invoke this SodaEvent while adding a response.");
            }

            var success = AddResponse(response);
            if (success)
            {
                response(callbackParameter());
            }
            return success;
        }

        [Obsolete("You don't need to pass a listener object anymore.")]
        public bool AddResponseAndInvoke(Action<T> response, UnityEngine.Object listener)
        {
            return AddResponseAndInvoke(response);
        }

        internal void Invoke(T parameter)
        {
            if (currentlyBeingInvoked)
            {
                invokeAgain = true;
                invokeAgainParameter = parameter;
                return;
            }
            
            RunInvocation(action => action(parameter));

            if (invokeAgain)
            {
                invokeAgain = false;
                var p = invokeAgainParameter;
                invokeAgainParameter = default;

                Invoke(p);
            }
        }

        internal override int GetListeners(object[] listeners)
        {
            return WriteToArray(responses.Select(action => action.Target), listeners);
        }
    }

    /// <summary>
    /// An event without a paremeter.
    /// In addition event responses, SodaEvents store the object that add the response for debugging purposes.
    /// </summary>
    public class SodaEvent : SodaEventBase<Action>
    {
        internal void Invoke()
        {
            if (currentlyBeingInvoked)
            {
                invokeAgain = true;
                return;
            }

            RunInvocation(action => action());

            if (invokeAgain)
            {
                invokeAgain = false;
                Invoke();
            }
        }

        internal override int GetListeners(object[] listeners)
        {
            return WriteToArray(responses.Select(action => action.Target), listeners);
        }
    }

    public abstract class SodaEventBase<T> : SodaEventBase
    {
        protected readonly List<T> responses = new List<T>();
        protected readonly HashSet<T> responseSet = new HashSet<T>();
        protected readonly List<T> responsesToRemove = new List<T>();

        /// <summary>
        /// The number of registered responses.
        /// </summary>
        public int responseCount
        {
            get { return responses.Count; }
        }

        public SodaEventBase(Action onChangeResponseCollection = null) : base(onChangeResponseCollection)
        {
        }

        /// <summary>
        /// Add a response to be invoked with the event.
        /// </summary>
        /// <param name="response">The response to invoke.</param>
        /// <returns>True if the response could be added, false if it was already added before.</returns>
        public bool AddResponse(T response)
        {
            if (!responseSet.Contains(response))
            {
                responses.Add(response);
                responseSet.Add(response);
                onChangeResponseCollection?.Invoke();
                return true;
            }
            return false;
        }

        [Obsolete("You don't need to pass a listener object anymore.")]
        public bool AddResponse(T response, UnityEngine.Object listener)
        {
            return AddResponse(response);
        }

        protected void RunInvocation(Action<T> invocation)
        {
            currentlyBeingInvoked = true;

            responsesToRemove.Clear();

            for (var i = responses.Count - 1; i >= 0; i--)
            {
                try
                {
                    invocation(responses[i]);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            foreach (var response in responsesToRemove)
            {
                ActuallyRemoveResponse(response);
            }
            responsesToRemove.Clear();

            currentlyBeingInvoked = false;
        }

        /// <summary>
        /// Removes a response so it's no longer invoked.
        /// </summary>
        /// <param name="response">The response to remove.</param>
        public void RemoveResponse(T response)
        {
            if (currentlyBeingInvoked)
            {
                responsesToRemove.Add(response);
            }
            else
            {
                ActuallyRemoveResponse(response);
            }
        }

        protected bool ActuallyRemoveResponse(T response)
        {
            return responseSet.Remove(response) && responses.Remove(response);
        }
    }

    public abstract class SodaEventBase
    {
        protected readonly Action onChangeResponseCollection;
        // For preventing cyclic/recursive invocation
        protected bool currentlyBeingInvoked = false;
        protected bool invokeAgain = false;

        public SodaEventBase(Action onChangeResponseCollection = null)
        {
            this.onChangeResponseCollection = onChangeResponseCollection;
        }
        
        internal abstract int GetListeners(object[] listeners);

        protected static int WriteToArray<T>(IEnumerable<T> input, T[] output)
        {
            var index = 0;
            using (var values = input.GetEnumerator())
            {
                while (index < output.Length && values.MoveNext())
                {
                    output[index] = values.Current;
                    index++;
                }
            }
            return index;
        }
    }
}