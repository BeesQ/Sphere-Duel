using Unity.Netcode;
using UnityEngine;

public class ScoreManager : NetworkBehaviour {
    #region Singleton
    public static ScoreManager Instance { get; private set; }
    #endregion

    #region Network Variables
    private NetworkVariable<int> hostScore = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private NetworkVariable<int> clientScore = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    #endregion

    #region Properties
    public bool IsMatchOver { get; private set; }
    #endregion

    #region Unity Callbacks
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy() {
        if (Instance == this) Instance = null;
    }

    private void OnEnable() {
        GameEvents.OnPlayerDied += HandlePlayerDied;
    }

    private void OnDisable() {
        GameEvents.OnPlayerDied -= HandlePlayerDied;
    }
    #endregion

    #region Network Callbacks
    public override void OnNetworkSpawn() {
        hostScore.OnValueChanged += HandleHostScoreChanged;
        clientScore.OnValueChanged += HandleClientScoreChanged;

        GameEvents.OnScoreChanged?.Invoke(0, hostScore.Value);
        GameEvents.OnScoreChanged?.Invoke(1, clientScore.Value);
    }

    public override void OnNetworkDespawn() {
        hostScore.OnValueChanged -= HandleHostScoreChanged;
        clientScore.OnValueChanged -= HandleClientScoreChanged;
    }
    #endregion

    #region Score Logic
    private void HandlePlayerDied(ulong diedClientId) {
        if (!IsServer) return;
        if (IsMatchOver) return;

        if (diedClientId == 0) {
            clientScore.Value++;
        }
        else {
            hostScore.Value++;
        }

        int scoreToWin = GameManager.Instance.ScoreToWin;

        if (hostScore.Value >= scoreToWin) {
            HandleMatchWonClientRpc(0);
        }
        else if (clientScore.Value >= scoreToWin) {
            HandleMatchWonClientRpc(1);
        }
    }
    #endregion

    #region Score Events
    private void HandleHostScoreChanged(int previousValue, int newValue) {
        GameEvents.OnScoreChanged?.Invoke(0, newValue);
    }

    private void HandleClientScoreChanged(int previousValue, int newValue) {
        GameEvents.OnScoreChanged?.Invoke(1, newValue);
    }
    #endregion

    #region Match Flow
    [ClientRpc]
    private void HandleMatchWonClientRpc(ulong winnerClientId) {
        IsMatchOver = true;
        GameEvents.OnMatchWon?.Invoke(winnerClientId);
    }

    public void RequestPlayAgain() {
        if (IsServer) {
            ResetMatch();
        }
        else {
            RequestPlayAgainRpc();
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestPlayAgainRpc() {
        ResetMatch();
    }

    private void ResetMatch() {
        if (!IsMatchOver) return;

        hostScore.Value = 0;
        clientScore.Value = 0;

        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList) {
            PlayerHealth health = client.PlayerObject.GetComponent<PlayerHealth>();
            if (health != null) {
                health.ResetForNewMatch();
            }
        }

        HandleMatchResetClientRpc();
    }

    [ClientRpc]
    private void HandleMatchResetClientRpc() {
        IsMatchOver = false;
        GameEvents.OnMatchReset?.Invoke();
    }
    #endregion
}