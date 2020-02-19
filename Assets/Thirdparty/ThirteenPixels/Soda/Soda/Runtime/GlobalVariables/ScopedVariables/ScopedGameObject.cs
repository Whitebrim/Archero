// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;

    [System.Serializable]
    public class ScopedGameObject : ScopedVariableBase<GameObject, GlobalGameObject>
    {
        public ScopedGameObject(GameObject value) : base(value)
        {
        }
    }
}
