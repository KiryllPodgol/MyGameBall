using UnityEngine;

public static class SceneLoader
{
    public static void LoadScene(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
        {
            SceneTransition.SwitchSceneWithLoading(sceneIndex);
        }
        else
        {
            Debug.LogError("Scene index out of range!");
        }
    }
}