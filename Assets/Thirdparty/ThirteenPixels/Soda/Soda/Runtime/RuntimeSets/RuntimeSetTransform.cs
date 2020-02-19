// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;

    /// <summary>
    /// A global set of GameObjects, curated at runtime.
    /// Stores the transform components of the GameObjects added to it.
    /// </summary>
    [CreateAssetMenu(menuName = "Soda/RuntimeSet/Transform", order = 20)]
    public class RuntimeSetTransform : RuntimeSetBase<Transform>
    {
        protected override bool TryCreateElement(GameObject go, out Transform element)
        {
            element = go.transform;
            return true;
        }
    }
}
