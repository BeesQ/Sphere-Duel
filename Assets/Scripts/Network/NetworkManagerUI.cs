using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour {
    #region UI References

    [Header("Buttons")]
    [Tooltip("Button to start hosting a game")]
    [SerializeField] private Button hostButton;

    [Tooltip("Button to join an existing game as client")]
    [SerializeField] private Button joinButton;

    [Header("Input")]
    [Tooltip("Input field for the host IP address")]
    [SerializeField] private TMP_InputField ipInputField;

    [Header("Status")]
    [Tooltip("Text displaying connection status")]
    [SerializeField] private TextMeshProUGUI statusText;

    #endregion

    #region Unity Callbacks

    private void Start() {
        hostButton.onClick.AddListener(HandleHostClicked);
        joinButton.onClick.AddListener(HandleJoinClicked);

        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;

        ipInputField.text = "127.0.0.1";
        SetStatus("Enter IP then","Host or Join.");
    }

    private void OnDestroy() {
        if (NetworkManager.Singleton == null) return;

        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
    }

    #endregion

    #region Button Handlers

    private void HandleHostClicked() {
        SetButtonsInteractable(false);
        SetStatus("Starting host...");

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData("0.0.0.0", Const.DefaultPort);

        NetworkManager.Singleton.StartHost();

        SetStatus($"Hosting on port {Const.DefaultPort}.", $"Waiting for player...");
    }

    private void HandleJoinClicked() {
        string ip = ipInputField.text.Trim();

        if (string.IsNullOrEmpty(ip)) {
            SetStatus("Please enter a valid IP address.");
            return;
        }

        SetButtonsInteractable(false);

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, Const.DefaultPort);

        NetworkManager.Singleton.StartClient();

        SetStatus("Connecting to:", $"{ip}:{Const.DefaultPort}...");
    }

    #endregion

    #region Network Callbacks

    private void HandleClientConnected(ulong clientId) {
        GameEvents.OnClientConnected?.Invoke(clientId);

        if (NetworkManager.Singleton.IsHost) {
            int connectedPlayers = NetworkManager.Singleton.ConnectedClientsIds.Count;
            SetStatus("Waiting for player...", $"Players: {connectedPlayers}/{Const.MaxPlayers}");

            if (connectedPlayers >= Const.MaxPlayers) {
                LoadGameScene();
            }
        }
        else {
            SetStatus("Connected!", "Waiting for host to start...");
        }
    }

    private void HandleClientDisconnected(ulong clientId) {
        GameEvents.OnClientDisconnected?.Invoke(clientId);

        if (!NetworkManager.Singleton.IsHost) return;

        int connectedPlayers = NetworkManager.Singleton.ConnectedClientsIds.Count;
        SetStatus("Player disconnected.", $"Players: {connectedPlayers}/{Const.MaxPlayers}");
    }

    #endregion

    #region Scene Management

    private void LoadGameScene() {
        NetworkManager.Singleton.SceneManager.LoadScene(Const.GameSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    #endregion

    #region UI Helpers

    private void SetStatus(string message) {
        statusText.text = message;
    }

    private void SetStatus(string line1, string line2) {
        statusText.text = $"{line1}\n{line2}";
    }

    private void SetButtonsInteractable(bool interactable) {
        hostButton.interactable = interactable;
        joinButton.interactable = interactable;
        ipInputField.interactable = interactable;
    }

    #endregion
}