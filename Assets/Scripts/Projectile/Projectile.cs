using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour {
    #region References
    [Header("References")]
    [Tooltip("Rigidbody2D for physics-based movement")]
    [SerializeField] private Rigidbody2D rb;
    #endregion

    #region State
    private ulong shooterClientId;
    private float lifetimeTimer;
    #endregion

    #region Unity Callbacks
    private void Awake() {
        ValidateReferences();
    }

    private void Update() {
        if (!IsServer) return;

        lifetimeTimer -= Time.deltaTime;

        if (lifetimeTimer <= 0f) {
            NetworkObject.Despawn();
        }
    }

    private void FixedUpdate() {
        if (!IsServer) return;

        Vector2 newPosition = rb.position + (Vector2)transform.right * GameManager.Instance.ProjectileSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!IsServer) return;

        PlayerHealth health = collision.GetComponentInParent<PlayerHealth>();
        if (health == null) return;
        if (health.OwnerClientId == shooterClientId) return;

        health.TakeDamage(GameManager.Instance.ProjectileDamage);
        NetworkObject.Despawn();
    }
    #endregion

    #region Public Methods
    public void Initialize(ulong shooterClientId) {
        this.shooterClientId = shooterClientId;
    }
    #endregion

    #region Network Callbacks
    public override void OnNetworkSpawn() {
        if (IsServer) {
            lifetimeTimer = GameManager.Instance.ProjectileLifetime;
        }
    }
    #endregion

    #region Validation
    private void ValidateReferences() {
        bool isValid = true;

        if (rb == null) { Debug.LogError($"[{nameof(Projectile)}] Rigidbody2D is not assigned!", this); isValid = false; }

        if (!isValid) enabled = false;
    }
    #endregion
}