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
            // Загрузка по имени сцены
            SceneManager.LoadScene(nextLevelName);
        }
        else if (nextLevelIndex >= 0)
        {
            // Загрузка по индексу
            SceneManager.LoadScene(nextLevelIndex);
        }
        else
        {
            Debug.LogWarning("Параметры портала не настроены! Укажите имя сцены или индекс.");
        }
    }
}