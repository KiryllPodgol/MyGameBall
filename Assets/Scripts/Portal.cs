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
        if (!string.IsNullOrEmpty(nextLevelName))
        {
            Debug.Log($"Переход на уровень: {nextLevelName}");
            SceneTransition.SwitchToScene(SceneManager.GetSceneByName(nextLevelName).buildIndex);
        }
        else if (nextLevelIndex >= 0)
        {
            Debug.Log($"Переход на уровень с индексом: {nextLevelIndex}");
            SceneTransition.SwitchToScene(nextLevelIndex);
        }
        else
        {
            Debug.LogWarning("Параметры портала не настроены! Укажите имя сцены или индекс.");
        }
    }
}