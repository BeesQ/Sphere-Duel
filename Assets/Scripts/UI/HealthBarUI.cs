using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour {
    #region References
    [Header("Host Health (Left)")]
    [Tooltip("Slider for the host player health bar")]
    [SerializeField] private Slider hostHealthSlider;

    [Tooltip("Fill Image of the host health slider (color changes green to red)")]
    [SerializeField] private Image hostFillImage;

    [Header("Client Health (Right)")]
    [Tooltip("Slider for the client player health bar")]
    [SerializeField] private Slider clientHealthSlider;

    [Tooltip("Fill Image of the client health slider (color changes green to red)")]
    [SerializeField] private Image clientFillImage;

    [Header("Score")]
    [Tooltip("Score text below host health bar (format: 0/3)")]
    [SerializeField] private TextMeshProUGUI hostScoreText;

    [Tooltip("Score text below client health bar (format: 0/3)")]
    [SerializeField] private TextMeshProUGUI clientScoreText;
    #endregion

    #region Unity Callbacks
    private void Awake() {
        ValidateReferences();
    }

    private void Start() {
        InitializeSliders();
        InitializeScores();
    }

    private void OnEnable() {
        GameEvents.OnPlayerHealthChanged += HandlePlayerHealthChanged;
        GameEvents.OnScoreChanged += HandleScoreChanged;
    }

    private void OnDisable() {
        GameEvents.OnPlayerHealthChanged -= HandlePlayerHealthChanged;
        GameEvents.OnScoreChanged -= HandleScoreChanged;
    }
    #endregion

    #region Health Updates
    private void HandlePlayerHealthChanged(ulong clientId, float currentHealth) {
        float maxHealth = GameManager.Instance.PlayerMaxHealth;
        float normalized = Mathf.Clamp01((currentHealth - 1f) / (maxHealth - 1f));

        if (clientId == 0) {
            hostHealthSlider.value = currentHealth;
            hostFillImage.color = Color.Lerp(Const.NegativeRedColor, Const.PositiveGreenColor, normalized);
        }
        else {
            clientHealthSlider.value = currentHealth;
            clientFillImage.color = Color.Lerp(Const.NegativeRedColor, Const.PositiveGreenColor, normalized);
        }
    }
    #endregion

    #region Score Updates
    private void HandleScoreChanged(ulong clientId, int newScore) {
        int scoreToWin = GameManager.Instance.ScoreToWin;
        string scoreText = $"{newScore}/{scoreToWin}";

        if (clientId == 0) {
            hostScoreText.text = scoreText;
        }
        else {
            clientScoreText.text = scoreText;
        }
    }
    #endregion

    #region Initialization
    private void InitializeSliders() {
        float maxHealth = GameManager.Instance.PlayerMaxHealth;

        hostHealthSlider.wholeNumbers = true;
        hostHealthSlider.minValue = 0;
        hostHealthSlider.maxValue = maxHealth;
        hostHealthSlider.value = maxHealth;
        hostFillImage.color = Color.green;

        clientHealthSlider.wholeNumbers = true;
        clientHealthSlider.minValue = 0;
        clientHealthSlider.maxValue = maxHealth;
        clientHealthSlider.value = maxHealth;
        clientFillImage.color = Color.green;
    }

    private void InitializeScores() {
        int scoreToWin = GameManager.Instance.ScoreToWin;
        hostScoreText.text = $"0/{scoreToWin}";
        clientScoreText.text = $"0/{scoreToWin}";
    }
    #endregion

    #region Validation
    private void ValidateReferences() {
        bool isValid = true;

        if (hostHealthSlider == null) { Debug.LogError($"[{nameof(HealthBarUI)}] Host Health Slider is not assigned!", this); isValid = false; }
        if (hostFillImage == null) { Debug.LogError($"[{nameof(HealthBarUI)}] Host Fill Image is not assigned!", this); isValid = false; }
        if (clientHealthSlider == null) { Debug.LogError($"[{nameof(HealthBarUI)}] Client Health Slider is not assigned!", this); isValid = false; }
        if (clientFillImage == null) { Debug.LogError($"[{nameof(HealthBarUI)}] Client Fill Image is not assigned!", this); isValid = false; }
        if (hostScoreText == null) { Debug.LogError($"[{nameof(HealthBarUI)}] Host Score Text is not assigned!", this); isValid = false; }
        if (clientScoreText == null) { Debug.LogError($"[{nameof(HealthBarUI)}] Client Score Text is not assigned!", this); isValid = false; }

        if (!isValid) enabled = false;
    }
    #endregion
}