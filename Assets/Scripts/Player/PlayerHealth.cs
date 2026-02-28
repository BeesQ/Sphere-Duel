using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour {
    #region References
    [Header("References")]
    [Tooltip("SpriteRenderer on the Sprite child (hidden on death)")]
    [SerializeField] private SpriteRenderer visualSpriteRenderer;

    [Tooltip("NetworkTransform for teleporting on respawn")]
    [SerializeField] private NetworkTransform networkTransform;
    #endregion

    #region Network Variables
    private NetworkVariable<float> currentHealth = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    #endregion

    #region Properties
    public bool IsAlive { get; private set; } = true;
    #endregion

    #region Network Callbacks
    public override void OnNetworkSpawn() {
        currentHealth.OnValueChanged += HandleHealthChanged;

        if (IsServer) {
            currentHealth.Value = GameManager.Instance.PlayerMaxHealth;
        }

        GameEvents.OnPlayerSpawned?.Invoke(OwnerClientId);
    }

    public override void OnNetworkDespawn() {
        currentHealth.OnValueChanged -= HandleHealthChanged;
    }
    #endregion

    #region Public Methods
    public void TakeDamage(float damage) {
        if (!IsServer) return;
        if (!IsAlive) return;

        currentHealth.Value = Mathf.Max(0f, currentHealth.Value - damage);

        if (currentHealth.Value <= 0f) {
            HandleDeathClientRpc();
            StartCoroutine(RespawnCoroutine());
        }
    }
    #endregion

    #region Health
    private void HandleHealthChanged(float previousValue, float newValue) {
        GameEvents.OnPlayerHealthChanged?.Invoke(OwnerClientId, newValue);
    }
    #endregion

    #region Death And Respawn
    [ClientRpc]
    private void HandleDeathClientRpc() {
        IsAlive = false;
        visualSpriteRenderer.enabled = false;
        gameObject.SetActive(false);
        GameEvents.OnPlayerDied?.Invoke(OwnerClientId);
    }

    private IEnumerator RespawnCoroutine() {
        yield return new WaitForSeconds(GameManager.Instance.RespawnDelay);

        currentHealth.Value = GameManager.Instance.PlayerMaxHealth;

        float xPosition = OwnerClientId == 0 ? -Const.SpawnOffsetX : Const.SpawnOffsetX;
        networkTransform.Teleport(new Vector3(xPosition, 0f, 0f), Quaternion.identity, transform.localScale);

        HandleRespawnClientRpc();
    }

    [ClientRpc]
    private void HandleRespawnClientRpc() {
        IsAlive = true;
        visualSpriteRenderer.enabled = true;
        gameObject.SetActive(true);
        GameEvents.OnPlayerRespawned?.Invoke(OwnerClientId);
    }
    #endregion

    #region Validation
    private void Awake() {
        ValidateReferences();
    }

    private void ValidateReferences() {
        bool isValid = true;

        if (visualSpriteRenderer == null) { Debug.LogError($"[{nameof(PlayerHealth)}] SpriteRenderer is not assigned!", this); isValid = false; }
        if (networkTransform == null) { Debug.LogError($"[{nameof(PlayerHealth)}] NetworkTransform is not assigned!", this); isValid = false; }

        if (!isValid) enabled = false;
    }
    #endregion
}