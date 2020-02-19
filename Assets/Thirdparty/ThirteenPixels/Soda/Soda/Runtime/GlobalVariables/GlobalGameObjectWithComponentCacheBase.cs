// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;

    /// <summary>
    /// A GlobalVariable representing a GameObject, including a set of cached components.
    /// This comes very close to what is often referred to as a "singleton" in Unity terminology, but without the disdvantages of actual static code.
    /// </summary>
    public abstract class GlobalGameObjectWithComponentCacheBase<T> : GlobalGameObject
    {
        /// <summary>
        /// A set of cached components for direct access.
        /// </summary>
        public T componentCache { private set; get; }

        /// <summary>
        /// The value this GlobalVariable represents.
        /// Throws an exception if you pass a GameObject that doesn't match the component specifications.
        /// </summary>
        public new GameObject value
        {
            get
            {
                return base.value;
            }
            set
            {
                if (!TrySetValue(value))
                {
                    throw new System.Exception("The GameObject you're trying to assign does not match the component specification.");
                }
            }
        }

        /// <summary>
        /// Tries to create a component cache from the given GameObject.
        /// </summary>
        /// <param name="componentCache">A single component reference or (preferably) a struct containing multiple component references.</param>
        /// <returns>True only if <paramref name="gameObject"/> contains all the required components for the cache.</returns>
        protected virtual bool TryCreateComponentCache(GameObject gameObject, out T componentCache)
        {
            return ComponentCache.TryCreateViaReflection(gameObject,
                                                         out componentCache,
                                                         () => new System.Exception("Trying to initialize a component cache with a cache type that is neither a component nor a struct. Please use a component type or a struct, or override " + GetType() + ".TryCreateComponentCache to allow this."));
        }

        /// <summary>
        /// Tries to set the value to the passed GameObject.
        /// It's the same as setting the value, but returns false instead of raising an exception if it fails.
        /// </summary>
        /// <param name="gameObject">The GameObject to reference.</param>
        /// <returns>True if the GameObject's components match the specification for this GlobalVariable.</returns>
        public override sealed bool TrySetValue(GameObject gameObject)
        {
            if (gameObject == value)
            {
                return true;
            }
            else if (gameObject == null)
            {
                componentCache = default;
                base.value = gameObject;
                return true;
            }
            else if (TryCreateComponentCache(gameObject, out T componentCache))
            {
                this.componentCache = componentCache;
                base.value = gameObject;
                return true;
            }

            return false;
        }
    }
}
