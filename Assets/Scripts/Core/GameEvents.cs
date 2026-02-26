using System;

public static class GameEvents {
    #region Player

    public static Action<ulong> OnPlayerSpawned;
    public static Action<ulong> OnPlayerDied;
    public static Action<ulong, float> OnPlayerHealthChanged;

    #endregion

    #region Game

    public static Action OnGameStarted;
    public static Action OnGameEnded;
    public static Action OnRoundStarted;
    public static Action OnRoundEnded;

    #endregion

    #region Network

    public static Action<ulong> OnClientConnected;
    public static Action<ulong> OnClientDisconnected;

    #endregion
}