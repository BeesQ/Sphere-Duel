using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisconnectHandler : MonoBehaviour {
    #region Singleton
    public static DisconnectHandler Instance { get; private set; }
    #endregion

    #region State
    private bool isReturningToMenu;
    #endregion

    #region Unity Callbacks
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start() {
        if (NetworkManager.Singleton != null) {
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
        }
    }

    private void OnDestroy() {
        if (Instance == this) Instance = null;

        if (NetworkManager.Singleton != null) {
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
        }
    }
    #endregion

    #region Network Callbacks
    private void HandleClientDisconnected(ulong clientId) {
        if (isReturningToMenu) return;

        ReturnToMenu();
    }
    #endregion

    #region Public Methods
    public void ReturnToMenu() {
        if (isReturningToMenu) return;
        isReturningToMenu = true;

        if (NetworkManager.Singleton != null) {
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
            NetworkManager.Singleton.Shutdown();
        }

        SceneManager.LoadScene(Const.MenuSceneName);
    }
    #endregion
}