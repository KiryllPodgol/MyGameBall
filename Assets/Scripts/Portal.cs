using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] private string nextLevelName;
    [SerializeField] private int nextLevelIndex = -1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Ball>() != null)
        {
            LoadNextLevel();
        }
    }

    private void LoadNextLevel()
    {
        // Если задано имя сцены, используем его для перехода
        if (!string.IsNullOrEmpty(nextLevelName))
        {
            int sceneIndex = SceneManager.GetSceneByName(nextLevelName).buildIndex;
            SceneTransition.SwitchToScene(sceneIndex);
        }
        // Если задан индекс сцены, используем его напрямую
        else if (nextLevelIndex >= 0)
        {
            SceneTransition.SwitchToScene(nextLevelIndex);
        }
        else
        {
            Debug.LogWarning("Параметры портала не настроены! Укажите имя сцены или индекс.");
        }
    }
}