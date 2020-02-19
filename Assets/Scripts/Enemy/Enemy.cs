using UnityEngine;
using ThirteenPixels.Soda;
public class Enemy : Entity
{
    [SerializeField] protected GlobalTransform player;
    [SerializeField] protected GlobalEnemyHandler enemyHandler;
    [SerializeField] protected GameEvent onPlayerDeath;
    [SerializeField] protected float movingTime;  // Время движения противника
    [SerializeField] protected float waitingTime; // Время простоя противника
    [SerializeField] protected float randomTime;  // Рандомное время от 0 до значения будет добавлятся к времени движения
    [SerializeField] protected float touchDamageMultiplier = 1; // Модификатор базового урона при соприкосновении
    [SerializeField] protected int coinsToDrop = 100; // Сколько монет будет падать за убийство
    protected Player touchingPlayer; // Нужен для оптеделения, остался ли игрок в области поражения после кулдауна атаки от прикосновения

    protected void OnEnable()
    {
        onPlayerDeath.onRaise.AddResponse(ResetTouchingPlayer);
    }
    protected void OnDisable()
    {
        onPlayerDeath.onRaise.RemoveResponse(ResetTouchingPlayer);
    }

    private void ResetTouchingPlayer()
    {
        touchingPlayer = null;
    }

    protected override void Death(Entity killer)
    {
        Player player = killer as Player;
        if(player!=null)
            player.AddCoins(coinsToDrop);
        enemyHandler.componentCache.RemoveEnemy(this);
        Destroy(gameObject);
    }

    protected void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            touchingPlayer = player;
            player.TakeDamage(new DamageReport(damage * touchDamageMultiplier, this));
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.tag == Tags.playerTag)
            touchingPlayer = null;
    }
}
