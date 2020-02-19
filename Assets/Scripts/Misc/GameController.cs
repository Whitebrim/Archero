using UnityEngine;
using UnityEngine.SceneManagement;
using ThirteenPixels.Soda;

public enum GameState
{
    INIT,
    STARTED
}
public class GameController : MonoBehaviour
{
    [SerializeField] GlobalGameState gameState;
    [SerializeField] private GlobalEnemySpawner enemySpawner;
    private void Start()
    {
        enemySpawner.componentCache.SpawnEnemies();
        gameState.value = GameState.STARTED;
    }

    /// <summary>
    /// Restarts the game
    /// </summary>
    public void Restart()
    {
        gameState.value = GameState.INIT;
        SceneManager.LoadScene(0);
    }
}
