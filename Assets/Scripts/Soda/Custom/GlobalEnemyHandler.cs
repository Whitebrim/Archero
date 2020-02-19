using ThirteenPixels.Soda;
using UnityEngine;

[CreateAssetMenu(menuName = "Soda/GlobalVariable/GameObject/Global EnemySpawner")]
public class GlobalEnemySpawner : GlobalGameObjectWithComponentCacheBase<EnemySpawner>
{
    protected override bool TryCreateComponentCache(GameObject gameObject,
    out EnemySpawner componentCache)
    {
        componentCache = gameObject.GetComponent<EnemySpawner>();
        return componentCache != null;
    }
}