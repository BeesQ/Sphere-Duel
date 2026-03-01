using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour {
    #region References
    [Header("References")]
    [Tooltip("CircleCollider2D on the Sprite child (disabled on death)")]
    [SerializeField] private CircleCollider2D hitCollider;

    [Tooltip("NetworkTransform for teleporting on respawn")]
    [SerializeField] private NetworkTransform networkTransform;

    private SpriteRenderer[] allRenderers;
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

    #region Unity Callbacks
    private void Awake() {
        allRenderers = GetComponentsInChildren<SpriteRenderer>();
        ValidateReferences();
    }
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

            if (ScoreManager.Instance == null || !ScoreManager.Instance.IsMatchOver) {
                StartCoroutine(RespawnCoroutine());
            }
        }
    }

    public void ResetForNewMatch() {
        if (!IsServer) return;

        currentHealth.Value = GameManager.Instance.PlayerMaxHealth;

        Vector3 spawnPosition = SpawnHelper.GetSpawnPosition(OwnerClientId);
        Quaternion spawnRotation = SpawnHelper.GetSpawnRotation(OwnerClientId);

        HandleRespawnClientRpc(spawnPosition, spawnRotation);
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
        SetVisuals(false);
        GameEvents.OnPlayerDied?.Invoke(OwnerClientId);
    }

    private IEnumerator RespawnCoroutine() {
        yield return new WaitForSeconds(GameManager.Instance.RespawnDelay);

        currentHealth.Value = GameManager.Instance.PlayerMaxHealth;

        Vector3 spawnPosition = SpawnHelper.GetSpawnPosition(OwnerClientId);
        Quaternion spawnRotation = SpawnHelper.GetSpawnRotation(OwnerClientId);

        HandleRespawnClientRpc(spawnPosition, spawnRotation);
    }

    [ClientRpc]
    private void HandleRespawnClientRpc(Vector3 spawnPosition, Quaternion spawnRotation) {
        IsAlive = true;
        SetVisuals(true);

        if (IsOwner) {
            networkTransform.Teleport(spawnPosition, spawnRotation, transform.localScale);
        }

        GameEvents.OnPlayerRespawned?.Invoke(OwnerClientId);
    }

    private void SetVisuals(bool visible) {
        foreach (SpriteRenderer renderer in allRenderers) {
            renderer.enabled = visible;
        }

        hitCollider.enabled = visible;
    }
    #endregion

    #region Validation
    private void ValidateReferences() {
        bool isValid = true;

        if (hitCollider == null) { Debug.LogError($"[{nameof(PlayerHealth)}] CircleCollider2D is not assigned!", this); isValid = false; }
        if (networkTransform == null) { Debug.LogError($"[{nameof(PlayerHealth)}] NetworkTransform is not assigned!", this); isValid = false; }
        if (allRenderers.Length == 0) { Debug.LogError($"[{nameof(PlayerHealth)}] No SpriteRenderers found in children!", this); isValid = false; }

        if (!isValid) enabled = false;
    }
    #endregion
}