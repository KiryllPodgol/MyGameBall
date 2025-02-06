using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    private int _coins;
    [SerializeField] private string nextLevelName;
    [SerializeField] private int nextLevelIndex = -1;

    private bool isActivated = false; 
    private void OnTriggerEnter(Collider other)
    {
        if (!isActivated && other.GetComponent<Ball>() != null)
        {
            isActivated = true; 
            LoadNextLevel(); 
        }
    }

    private void LoadNextLevel()
    {
       
        if (GameStats.Instance != null)
        {
            GameStats.Instance.AddCoins(_coins);
            int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
            GameStats.Instance.EndLevel(currentLevelIndex); 
        }
        
        if (!string.IsNullOrEmpty(nextLevelName))
        {
            Debug.Log($"Переход на уровень: {nextLevelName}");
            int sceneIndex = SceneManager.GetSceneByName(nextLevelName).buildIndex;
            if (sceneIndex >= 0)
            {
                SceneTransition.SwitchSceneWithLoading(sceneIndex);
            }
            else
            {
                Debug.LogError($"Сцена с именем {nextLevelName} не найдена в Build Settings!");
            }
        }
        else if (nextLevelIndex >= 0)
        {
            Debug.Log($"Переход на уровень с индексом: {nextLevelIndex}");
            SceneTransition.SwitchSceneWithLoading(nextLevelIndex);
        }
        else
        {
            Debug.LogWarning("Параметры портала не настроены! Укажите имя сцены или индекс.");
        }
    }
}