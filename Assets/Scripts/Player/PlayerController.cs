using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour {
    #region References
    [Header("References")]
    [Tooltip("Player Rigidbody2D component")]
    [SerializeField] private Rigidbody2D rb;

    [Tooltip("NetworkTransform component for syncing position/rotation")]
    [SerializeField] private NetworkTransform networkTransform;

    private Camera mainCamera;
    #endregion

    #region State
    private Vector2 moveInput;
    #endregion

    #region Unity Callbacks
    private void Awake() {
        ValidateReferences();
    }

    private void Update() {
        if (!IsOwner) return;

        ReadInput();
        HandleRotation();
    }

    private void FixedUpdate() {
        if (!IsOwner) return;

        HandleMovement();
    }
    #endregion

    #region Network Callbacks
    public override void OnNetworkSpawn() {
        if (!IsOwner) return;

        mainCamera = Camera.main;
        SetSpawnPosition();
    }
    #endregion

    #region Input
    private void ReadInput() {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        Vector2 input = Vector2.zero;
        if (keyboard.wKey.isPressed) input.y += 1f;
        if (keyboard.sKey.isPressed) input.y -= 1f;
        if (keyboard.aKey.isPressed) input.x -= 1f;
        if (keyboard.dKey.isPressed) input.x += 1f;

        moveInput = input.normalized;
    }
    #endregion

    #region Movement
    private void HandleMovement() {
        Vector2 newPosition = rb.position + moveInput * GameManager.Instance.PlayerMoveSpeed * Time.fixedDeltaTime;

        newPosition.x = Mathf.Clamp(newPosition.x, -Const.ArenaHalfWidth, Const.ArenaHalfWidth);
        newPosition.y = Mathf.Clamp(newPosition.y, -Const.ArenaHalfHeight, Const.ArenaHalfHeight);

        rb.MovePosition(newPosition);
    }
    #endregion

    #region Rotation
    private void HandleRotation() {
        if (mainCamera == null || Mouse.current == null) return;

        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);

        Vector2 direction = ((Vector2)mouseWorldPos - rb.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    #endregion

    #region Spawn
    private void SetSpawnPosition() {
        float xPosition = OwnerClientId == 0 ? -Const.SpawnOffsetX : Const.SpawnOffsetX;
        Vector3 spawnPosition = new Vector3(xPosition, 0f, 0f);

        networkTransform.Teleport(spawnPosition, transform.rotation, transform.localScale);
    }
    #endregion

    #region Validation
    private void ValidateReferences() {
        if (rb == null) Debug.LogError($"[{nameof(PlayerController)}] Rigidbody2D is not assigned!", this);
        if (networkTransform == null) Debug.LogError($"[{nameof(PlayerController)}] NetworkTransform is not assigned!", this);
    }
    #endregion
}