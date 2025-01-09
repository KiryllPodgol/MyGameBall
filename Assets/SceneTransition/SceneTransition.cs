using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public TextMeshProUGUI LoadingPercentage; // Текст процента загрузки
    public Image LoadingProgress;            // Прогресс-бар
    private static SceneTransition _instance;
    private static bool ShouldPlayOpeningAnimation = false; // Чтобы анимация проигрывалась при открытии
    private AsyncOperation loadingSceneOperation; 
    private Animator animator;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject); // Уничтожить дублирующий объект
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject); // Сохраняем объект при смене сцен
        animator = GetComponent<Animator>();

        // Если анимация открытия должна проиграться
        if (ShouldPlayOpeningAnimation)
        {
            animator.SetTrigger("SceneOpening");
            ShouldPlayOpeningAnimation = false; // Сбрасываем флаг
        }
    }

    // Метод для переключения сцены через подложку
    public static void SwitchSceneWithLoading(int targetSceneIndex)
    {
        if (_instance == null)
        {
            Debug.LogError("SceneTransition instance is missing! Make sure it's added to the starting scene.");
            return;
        }

        // Запускаем анимацию закрытия сцены
        _instance.animator.SetTrigger("SceneClosing");

        // Передаем индекс целевой сцены для загрузки
        _instance.StartCoroutine(_instance.LoadSceneWithLoading(targetSceneIndex));
    }

    private IEnumerator LoadSceneWithLoading(int targetSceneIndex)
    {
        // Ждем завершения анимации закрытия
        yield return new WaitForSeconds(1f);

        // Загружаем сцену-подложку (экран загрузки)
        SceneManager.LoadScene("LoadingScene");

        // Ждем одного кадра, чтобы сцена подложки успела загрузиться
        yield return null;

        // Начинаем асинхронную загрузку целевой сцены
        loadingSceneOperation = SceneManager.LoadSceneAsync(targetSceneIndex);
        loadingSceneOperation.allowSceneActivation = false;

        // Обновляем прогресс на экране загрузки
        while (!loadingSceneOperation.isDone)
        {
            float progress = Mathf.Clamp01(loadingSceneOperation.progress / 0.9f);
            if (LoadingPercentage != null)
                LoadingPercentage.text = Mathf.RoundToInt(progress * 100) + "%";
            if (LoadingProgress != null)
                LoadingProgress.fillAmount = progress;

          
            if (loadingSceneOperation.progress >= 0.9f)
            {
                break;
            }

            yield return null;
        }
    }

    // Метод вызывается анимацией после завершения
    public void OnAnimationOver()
    {
        ShouldPlayOpeningAnimation = true; // Устанавливаем анимацию открытия для следующей сцены
        if (loadingSceneOperation != null)
        {
            loadingSceneOperation.allowSceneActivation = true; // Активируем сцену
        }
    }
}