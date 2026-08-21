using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("The build index of the main gameplay scene.")]
    [SerializeField] private int gameSceneIndex = 0; // Set to your main game scene index

    // Hook this to the Play / Start Game Button's OnClick()
    public void PlayGame()
    {
        Time.timeScale = 1f; // Ensure game is unpaused
        SceneManager.LoadScene(gameSceneIndex);
    }

    // Hook this to the Quit Button's OnClick()
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}