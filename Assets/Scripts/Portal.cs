
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class Portal : MonoBehaviour
    {
        [SerializeField] private string nextLevelName; 
        [SerializeField] private int nextLevelIndex = -1; 

        private void OnTriggerEnter(Collider other)
        {
            // Проверяем, если объект, вошедший в триггер, является "Ball"
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
                SceneTransition.SwitchSceneWithLoading(nextLevelIndex); // Загрузка через сцену-подложку
            }
            else
            {
                Debug.LogWarning("Параметры портала не настроены! Укажите имя сцены или индекс.");
            }
        }
    }