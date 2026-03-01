using UnityEngine;

public static class SpawnHelper {
    #region Public Methods
    public static Vector3 GetSpawnPosition(ulong clientId) {
        float xPosition = clientId == 0 ? -Const.SpawnOffsetX : Const.SpawnOffsetX;
        return new Vector3(xPosition, 0f, 0f);
    }

    public static Quaternion GetSpawnRotation(ulong clientId) {
        return clientId == 0 ? Quaternion.identity : Quaternion.Euler(0f, 0f, 180f);
    }
    #endregion
}