using UnityEngine;

public class Rock : Shell
{
    protected override void OnEnemyCollision(Entity entity) { }

    protected override void OnObstacleCollision(Transform obstacle)
    {
        shooter.DeactivateShell(gameObject);
    }

    protected override void OnPlayerCollision(Entity entity)
    {
        entity.TakeDamage(damageReport);
        shooter.DeactivateShell(gameObject);
    }
}
