using ThirteenPixels.Soda;
using UnityEngine;

[CreateAssetMenu(menuName = "Soda/GlobalVariable/GameObject/Global Transform")]
public class GlobalTransform : GlobalGameObjectWithComponentCacheBase<Transform>
{
    protected override bool TryCreateComponentCache(GameObject gameObject,
    out Transform componentCache)
    {
        componentCache = gameObject.transform;
        return componentCache != null;
    }
}