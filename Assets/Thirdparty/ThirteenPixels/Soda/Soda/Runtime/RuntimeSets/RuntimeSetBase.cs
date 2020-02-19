// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A global set of GameObjects, curated at runtime.
    /// </summary>
    public abstract class RuntimeSetBase<T> : RuntimeSetBase
    {
        private readonly Dictionary<GameObject, T> dict = new Dictionary<GameObject, T>();
        private readonly List<T> list = new List<T>();

        public sealed override int elementCount
        {
            get { return dict.Count; }
        }

        /// <summary>
        /// Adds a GameObject to the RuntimeSet.
        /// Only succeeds when an "element" could be created by applying the RuntimeSet's rules.
        /// </summary>
        /// <param name="go">The GameObject to add to the RuntimeSet.</param>
        /// <returns>True if the GameObject could be added to the RuntimeSet. False if the GameObject was already added or no proper element could be created from it.</returns>
        public sealed override bool Add(GameObject go)
        {
            if (go == null) return false;

            T element;
            var success = false;
            if (TryCreateElement(go, out element))
            {
                success = AddToCollections(go, element);
                if (success)
                {
                    OnAddElement(element);
                    onElementCountChange.Invoke(elementCount);
                }
            }
            else
            {
                Debug.LogError("Could not create element for RuntimeSet.", go);
            }
            
            return success;
        }

        /// <summary>
        /// Creates an "element" for the RuntimeSet if possible.
        /// An element could be a component reference or a struct comtaining multiple references.
        /// </summary>
        /// <param name="gameObject">The GameObject to create the element from.</param>
        /// <param name="element">The element that was created.</param>
        /// <returns>Whether or not the element could be created. If, for example, the required component was missing on the given GameObject, this is false.</returns>
        protected virtual bool TryCreateElement(GameObject gameObject, out T element)
        {
            return ComponentCache.TryCreateViaReflection(gameObject,
                                                         out element,
                                                         () => new Exception("Trying to initialize a component cache with a cache type that is neither a component nor a struct. Please use a component type or a struct, or override " + GetType() + ".TryCreateElement to allow this."));
        }

        /// <summary>
        /// Called whenever an element is successfully added.
        /// Override this to initialize the element to be in the set.
        /// For example, you can make the RuntimeSet monitor changes in its element.
        /// </summary>
        /// <param name="element">The element that was added.</param>
        protected virtual void OnAddElement(T element)
        {

        }

        /// <summary>
        /// Removes a GameObject from the RuntimeSet.
        /// </summary>
        /// <param name="go">The GameObject to remove.</param>
        public sealed override void Remove(GameObject go)
        {
            T element;
            if (dict.TryGetValue(go, out element))
            {
                dict.Remove(go);
                list.Remove(element);
                OnRemoveElement(element);
                onElementCountChange.Invoke(elementCount);
            }
        }

        /// <summary>
        /// Called whenever an element is successfully removed.
        /// Override this to return the element to the state it was before being added to this RuntimeSet.
        /// For example, you can revoke any monitoring of the element done by the RuntimeSet.
        /// </summary>
        /// <param name="element">The element that was removed.</param>
        protected virtual void OnRemoveElement(T element)
        {

        }

        private bool AddToCollections(GameObject go, T element)
        {
            if (!dict.ContainsKey(go))
            {
                dict.Add(go, element);
                list.Add(element);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether a specific GameObject is part of this set.
        /// </summary>
        public sealed override bool Contains(GameObject go)
        {
            return dict.ContainsKey(go);
        }

        /// <summary>
        /// Invokes the given action on every element in this RuntimeSet.
        /// Adding or removing elements to or from this set is okay.
        /// Elements added during the iteration are not taken into account for it.
        /// </summary>
        public void ForEach(Action<T> action)
        {
            for (var i = list.Count - 1; i >= 0; i--)
            {
                action(list[i]);
            }
        }

        /// <summary>
        /// Returns a collection of all GameObjects in this RuntimeSet that updates if the RuntimeSet contents change.
        /// </summary>
        public sealed override IEnumerable<GameObject> GetAllGameObjects()
        {
            return dict.Keys;
        }

        /// <summary>
        /// Returns a collection of all elements in this RuntimeSet that updates if the RuntimeSet contents change.
        /// </summary>
        public IEnumerable<T> GetAllElements()
        {
            return dict.Values;
        }
    }

    public abstract class RuntimeSetBase : ScriptableObject
    {
        #region Description
#pragma warning disable 0414
        [TextArea]
        [SerializeField]
        private string description = "";
#pragma warning restore 0414
        #endregion
        
        public abstract int elementCount { get; }
        
        private SodaEvent<int> _onElementCountChange;
        /// <summary>
        /// This event is invoked when an item is added or removed.
        /// </summary>
        public SodaEvent<int> onElementCountChange
        {
            get
            {
                if (_onElementCountChange == null)
                {
                    _onElementCountChange = new SodaEvent<int>(() => elementCount);
                }
                return _onElementCountChange;
            }
        }

#if UNITY_EDITOR
        internal SodaEventBase GetOnElementCountChangeEvent()
        {
            return onElementCountChange;
        }
#endif

        /// <summary>
        /// Adds a GameObject to the RuntimeSet.
        /// Only succeeds when an "element" could be created by applying the RuntimeSet's rules.
        /// </summary>
        /// <param name="go">The GameObject to add to the RuntimeSet.</param>
        /// <returns>True if the GameObject could be added to the RuntimeSet. False if the GameObject was already added or no proper element could be created from it.</returns>
        public abstract bool Add(GameObject go);

        /// <summary>
        /// Removes a GameObject from the RuntimeSet.
        /// </summary>
        /// <param name="go">The GameObject to remove.</param>
        public abstract void Remove(GameObject go);

        /// <summary>
        /// Checks whether a specific GameObject is part of this set.
        /// </summary>
        public abstract bool Contains(GameObject go);

        /// <summary>
        /// Returns a collection of all GameObjects in this RuntimeSet.
        /// Used for the inspector window to display a list of all items.
        /// </summary>
        public abstract IEnumerable<GameObject> GetAllGameObjects();
    }
}
