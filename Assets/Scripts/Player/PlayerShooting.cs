using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : NetworkBehaviour {
    #region References
    [Header("References")]
    [Tooltip("Transform where projectiles spawn (Shoot Point child)")]
    [SerializeField] private Transform shootPoint;

    [Tooltip("Projectile prefab with NetworkObject")]
    [SerializeField] private GameObject projectilePrefab;

    [Tooltip("PlayerHealth component for alive state check")]
    [SerializeField] private PlayerHealth playerHealth;
    #endregion

    #region State
    private float fireCooldownTimer;
    #endregion

    #region Unity Callbacks
    private void Awake() {
        ValidateReferences();
    }

    private void Update() {
        if (!IsOwner) return;

        fireCooldownTimer -= Time.deltaTime;

        if (!playerHealth.IsAlive) return;

        if (Mouse.current != null && Mouse.current.leftButton.isPressed && fireCooldownTimer <= 0f) {
            fireCooldownTimer = GameManager.Instance.FireCooldown;
            ShootServerRpc(shootPoint.position, transform.rotation);
        }
    }
    #endregion

    #region Network
    [ServerRpc]
    private void ShootServerRpc(Vector3 spawnPosition, Quaternion spawnRotation) {
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, spawnRotation);
        projectile.GetComponent<Projectile>().Initialize(OwnerClientId);
        projectile.GetComponent<NetworkObject>().Spawn();
    }
    #endregion

    #region Validation
    private void ValidateReferences() {
        bool isValid = true;

        if (shootPoint == null) { Debug.LogError($"[{nameof(PlayerShooting)}] Shoot Point is not assigned!", this); isValid = false; }
        if (projectilePrefab == null) { Debug.LogError($"[{nameof(PlayerShooting)}] Projectile Prefab is not assigned!", this); isValid = false; }
        if (playerHealth == null) { Debug.LogError($"[{nameof(PlayerShooting)}] PlayerHealth is not assigned!", this); isValid = false; }

        if (!isValid) enabled = false;
    }
    #endregion
}