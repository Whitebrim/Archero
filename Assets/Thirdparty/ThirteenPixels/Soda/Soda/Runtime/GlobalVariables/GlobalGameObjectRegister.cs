// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;

    /// <summary>
    /// Add this component to a GameObject and assign a GlobalGameObject value to assign the GameObject to it
    /// as long as the component is active and enabled and the GameObject's component setup matches any requirements imposed by the referenced GlobalGameObject.
    /// </summary>
    public class GlobalGameObjectRegister : MonoBehaviour
    {
        [SerializeField]
        private GlobalGameObject globalGameObject = default;

        private void OnEnable()
        {
            if (globalGameObject == null)
            {
                Debug.LogError("GlobalGameObjectRegister doesn't have a GlobalGameObject assigned.", gameObject);
                enabled = false;
                return;
            }

            var couldSetValue = globalGameObject.TrySetValue(gameObject);
            
            if (!couldSetValue)
            {
                Debug.LogError("GlobalGameObjectRegister couldn't register the GameObject due to its components not matching the requirements.", gameObject);
                enabled = false;
                return;
            }

            globalGameObject.onChange.AddResponse(CheckValue);
        }

        private void OnDisable()
        {
            if (globalGameObject == null) return;

            globalGameObject.onChange.RemoveResponse(CheckValue);
            if(globalGameObject.value == gameObject)
            {
                globalGameObject.value = null;
            }
        }

        private void CheckValue(GameObject newGO)
        {
            if(newGO != gameObject)
            {
                enabled = false;
            }
        }
    }
}
