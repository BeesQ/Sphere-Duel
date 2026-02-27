using UnityEngine;

public class GameManager : MonoBehaviour {
    #region Singleton
    public static GameManager Instance { get; private set; }
    #endregion

    #region Config
    [Header("Player")]
    [Tooltip("Player movement speed (default from Const)")]
    [SerializeField] private float playerMoveSpeed = Const.PlayerMoveSpeed;

    [Tooltip("Player max health (default from Const)")]
    [SerializeField] private float playerMaxHealth = Const.PlayerMaxHealth;

    [Header("Projectile")]
    [Tooltip("Projectile travel speed (default from Const)")]
    [SerializeField] private float projectileSpeed = Const.ProjectileSpeed;

    [Tooltip("Damage dealt per projectile hit (default from Const)")]
    [SerializeField] private float projectileDamage = Const.ProjectileDamage;

    [Header("Game")]
    [Tooltip("Score needed to win the match")]
    [SerializeField] private int scoreToWin = Const.ScoreToWin;

    [Tooltip("Delay before player respawns after death")]
    [SerializeField] private float respawnDelay = Const.RespawnDelay;
    #endregion

    #region Properties
    public float PlayerMoveSpeed => playerMoveSpeed;
    public float PlayerMaxHealth => playerMaxHealth;
    public float ProjectileSpeed => projectileSpeed;
    public float ProjectileDamage => projectileDamage;
    public int ScoreToWin => scoreToWin;
    public float RespawnDelay => respawnDelay;
    #endregion

    #region Unity Callbacks
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion
}