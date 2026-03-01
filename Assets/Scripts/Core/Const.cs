using UnityEngine;

public static class Const {
    #region Player
    public const float PlayerMoveSpeed = 5f;
    public const float PlayerMaxHealth = 5f;
    public const float DefaultPlayerRadius = 0.5f;
    #endregion

    #region Projectile
    public const float ProjectileSpeed = 10f;
    public const float ProjectileDamage = 1f;
    public const float ProjectileLifetime = 3f;
    public const float FireCooldown = 1f;
    #endregion

    #region Arena
    public const float ArenaMaxHalfWidth = 8.89f;
    #endregion

    #region Spawn
    public const float SpawnOffsetX = 6f;
    #endregion

    #region Game
    public const int ScoreToWin = 3;
    public const float RespawnDelay = 2f;
    #endregion

    #region Network
    public const ushort DefaultPort = 7777;
    public const int MaxPlayers = 2;
    #endregion

    #region Scenes
    public const string MenuSceneName = "MenuScene";
    public const string GameSceneName = "GameScene";
    #endregion

    #region UI Colors
    public static readonly Color WinColor = new Color(
        0.2f,
        0.8f,
        0.2f,
        1f
    );

    public static readonly Color LoseColor = new Color(
        0.8f,
        0.2f,
        0.2f,
        1f
    );
    #endregion
}