// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;

    /// <summary>
    /// A GlobalVariable representing a GameObject.
    /// </summary>
    [CreateAssetMenu(menuName = "Soda/GlobalVariable/GameObject", order = 80)]
    public class GlobalGameObject : GlobalVariableBase<GameObject>
    {
        /// <summary>
        /// Tries to set the value to the passed GameObject.
        /// It's the same as setting the value, but returns false instead of raising an exception if it fails.
        /// This method is meant to be overriden by GlobalGameObjectWithComponentCacheBase.
        /// </summary>
        /// <param name="gameObject">The GameObject to reference.</param>
        /// <returns>Always returns true.</returns>
        public virtual bool TrySetValue(GameObject gameObject)
        {
            value = gameObject;
            return true;
        }
    }
}
