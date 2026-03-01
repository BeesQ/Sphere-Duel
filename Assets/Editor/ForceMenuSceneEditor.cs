using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class ForceMenuSceneEditor {
    #region State
    private static bool isRedirecting;
    #endregion

    #region Initialization
    static ForceMenuSceneEditor() {
        EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
    }
    #endregion

    #region Play Mode Handling
    private static void HandlePlayModeStateChanged(PlayModeStateChange state) {
        if (state != PlayModeStateChange.ExitingEditMode) return;

        if (isRedirecting) {
            isRedirecting = false;
            return;
        }

        string currentScene = EditorSceneManager.GetActiveScene().name;
        if (currentScene == Const.MenuSceneName) return;

        EditorApplication.isPlaying = false;

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        EditorSceneManager.OpenScene($"Assets/Scenes/{Const.MenuSceneName}.unity");
        isRedirecting = true;

        EditorApplication.delayCall += () => {
            EditorApplication.isPlaying = true;
        };
    }
    #endregion
}