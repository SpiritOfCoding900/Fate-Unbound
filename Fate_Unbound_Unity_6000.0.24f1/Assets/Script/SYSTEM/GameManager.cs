using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SimpleSingleton<GameManager>
{
    [Header("Player Data: ")]
    [SerializeField] private Player player01;     // prefab reference in Inspector
    public Player CurrentPlayer { private set; get; }

    [System.Serializable]
    public struct PlayerClassStats
    {
        public string className;
        public float MaxHP, MaxMP, ATK, DEF, dodgeRate, moveSpeed;
        public string description;
        public int ID;
    }

    private bool hasChosenClass = false;
    private PlayerClassStats chosenStats;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainScene")
            UIManager.Instance.Open(GameUIID.Logo);
    }

    // Called from your UI
    public void SetChosenClass(PlayerClassStats stats)
    {
        chosenStats = stats;
        hasChosenClass = true;

        // If a player already exists, update them immediately.
        if (CurrentPlayer != null)
            ApplyChosenClassTo(CurrentPlayer);
    }

    public void spawnPlayerOnce(Vector3 PlayersNewSpawnPosition)
    {
        var obj = Instantiate(player01, PlayersNewSpawnPosition, Quaternion.identity);
        if (obj.TryGetComponent(out Player player))
        {
            CurrentPlayer = player;

            // Apply class after spawn (this is the important part)
            if (hasChosenClass)
                ApplyChosenClassTo(CurrentPlayer);
        }
    }

    private void ApplyChosenClassTo(Player player)
    {
        player.className = chosenStats.className;
        player.MaxHP = chosenStats.MaxHP;
        player.MaxMP = chosenStats.MaxMP;

        player.CurrentHP = player.MaxHP;
        player.CurrentMP = player.MaxMP;

        player.ATK = chosenStats.ATK;
        player.DEF = chosenStats.DEF;
        player.dodgeRate = chosenStats.dodgeRate;
        player.moveSpeed = chosenStats.moveSpeed;
        player.description = chosenStats.description;
        player.ID = chosenStats.ID;
    }
}