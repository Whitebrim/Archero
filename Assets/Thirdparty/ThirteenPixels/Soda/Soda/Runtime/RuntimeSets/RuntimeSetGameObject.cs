// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A global set of GameObjects, curated at runtime.
    /// Stores only the GameObject itself for some limited, but optimized usage cases - mainly to check whether a GameObject is in it or to destroy all elements at once.
    /// </summary>
    [CreateAssetMenu(menuName = "Soda/RuntimeSet/GameObject", order = 10)]
    public class RuntimeSetGameObject : RuntimeSetBase
    {
        private readonly List<GameObject> list = new List<GameObject>();
        private readonly HashSet<GameObject> set = new HashSet<GameObject>();

        public override int elementCount
        {
            get { return set.Count; }
        }

        public override bool Add(GameObject go)
        {
            if (set.Add(go))
            {
                list.Add(go);
                onElementCountChange.Invoke(elementCount);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Contains(GameObject go)
        {
            return set.Contains(go);
        }

        public override IEnumerable<GameObject> GetAllGameObjects()
        {
            return list;
        }

        public override void Remove(GameObject go)
        {
            if (set.Remove(go))
            {
                list.Remove(go);
                onElementCountChange.Invoke(elementCount);
            }
        }

        /// <summary>
        /// Invokes the given action on every element in this RuntimeSet.
        /// Adding or removing elements to or from this set is okay.
        /// Elements added during the iteration are not taken into account for it.
        /// </summary>
        public void ForEach(Action<GameObject> action)
        {
            for (var i = list.Count - 1; i >= 0; i--)
            {
                action(list[i]);
            }
        }
    }
}
