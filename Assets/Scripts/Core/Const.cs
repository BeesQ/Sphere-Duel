public static class Const {
    #region Player

    public const float PlayerMoveSpeed = 5f;
    public const float PlayerMaxHealth = 100f;

    #endregion

    #region Projectile

    public const float ProjectileSpeed = 10f;
    public const float ProjectileDamage = 20f;
    public const float ProjectileLifetime = 3f;

    #endregion

    #region Arena

    public const float ArenaHalfWidth = 8f;
    public const float ArenaHalfHeight = 4.5f;

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
}