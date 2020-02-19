using ThirteenPixels.Soda;
using UnityEngine;

[CreateAssetMenu(menuName = "Soda/GlobalVariable/GameObject/Global EnemyHandler")]
public class GlobalEnemyHandler : GlobalGameObjectWithComponentCacheBase<EnemyHandler>
{
    protected override bool TryCreateComponentCache(GameObject gameObject,
    out EnemyHandler componentCache)
    {
        componentCache = gameObject.GetComponent<EnemyHandler>();
        return componentCache != null;
    }
}