using Unity.Netcode;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinScreenUI : MonoBehaviour {
    #region References
    [Header("References")]
    [Tooltip("Root panel of the win screen (toggled on/off)")]
    [SerializeField] private GameObject winScreenPanel;

    [Tooltip("Image component on the panel (color changes on win/lose)")]
    [SerializeField] private Image panelImage;

    [Tooltip("Main result text (You Won! / You Lost!)")]
    [SerializeField] private TextMeshProUGUI resultText;

    [Tooltip("Button to restart the match")]
    [SerializeField] private Button playAgainButton;

    [Tooltip("Button to return to main menu")]
    [SerializeField] private Button mainMenuButton;
    #endregion

    #region Unity Callbacks
    private void Awake() {
        ValidateReferences();
    }

    private void Start() {
        winScreenPanel.SetActive(false);
        playAgainButton.onClick.AddListener(HandlePlayAgainClicked);
        mainMenuButton.onClick.AddListener(HandleMainMenuClicked);
    }

    private void OnEnable() {
        GameEvents.OnMatchWon += HandleMatchWon;
        GameEvents.OnMatchReset += HandleMatchReset;
    }

    private void OnDisable() {
        GameEvents.OnMatchWon -= HandleMatchWon;
        GameEvents.OnMatchReset -= HandleMatchReset;
    }
    #endregion

    #region Event Handlers
    private void HandleMatchWon(ulong winnerClientId) {
        winScreenPanel.SetActive(true);

        bool isLocalWinner = NetworkManager.Singleton.LocalClientId == winnerClientId;

        resultText.text = isLocalWinner ? "You Won!" : "You Lost!";
        panelImage.color = isLocalWinner ? Const.PositiveGreenColor : Const.NegativeRedColor;
    }

    private void HandleMatchReset() {
        winScreenPanel.SetActive(false);
    }
    #endregion

    #region Button Handlers
    private void HandlePlayAgainClicked() {
        if (ScoreManager.Instance != null) {
            ScoreManager.Instance.RequestPlayAgain();
        }
    }

    private void HandleMainMenuClicked() {
        if (DisconnectHandler.Instance != null) {
            DisconnectHandler.Instance.ReturnToMenu();
        }
    }
    #endregion

    #region Validation
    private void ValidateReferences() {
        bool isValid = true;

        if (winScreenPanel == null) { Debug.LogError($"[{nameof(WinScreenUI)}] Win Screen Panel is not assigned!", this); isValid = false; }
        if (panelImage == null) { Debug.LogError($"[{nameof(WinScreenUI)}] Panel Image is not assigned!", this); isValid = false; }
        if (resultText == null) { Debug.LogError($"[{nameof(WinScreenUI)}] Result Text is not assigned!", this); isValid = false; }
        if (playAgainButton == null) { Debug.LogError($"[{nameof(WinScreenUI)}] Play Again Button is not assigned!", this); isValid = false; }
        if (mainMenuButton == null) { Debug.LogError($"[{nameof(WinScreenUI)}] Main Menu Button is not assigned!", this); isValid = false; }

        if (!isValid) enabled = false;
    }
    #endregion
}