using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public TextMeshProUGUI LoadingPercentage;
    public Image LoadingProgress;
    private static SceneTransition _instance;
    [SerializeField] private bool ShouldPlayOpeningAnimation = false;
    private AsyncOperation loadingSceneOperation; 
    private Animator animator;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(transform.root.gameObject); 

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Awake: Не удалось найти Animator! Проверьте наличие компонента Animator.");
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    public void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (ShouldPlayOpeningAnimation)
        {
            Debug.Log("OnLevelFinishedLoading: Проигрывание анимации открытия.");
            animator.SetTrigger("SceneOpening");
            ShouldPlayOpeningAnimation = false;
        }
        else
        {
            Debug.Log("OnLevelFinishedLoading did not play opening animation");
        }
    }

    public static void SwitchSceneWithLoading(int targetSceneIndex)
    {
        Debug.Log("Попытка переключить сцену с индексом: " + targetSceneIndex);

        if (_instance == null)
        {
            Debug.LogError("Instance SceneTransition не найден! Убедитесь, что компонент добавлен в начальную сцену.");
            return;
        }
        if (_instance.animator != null)
        {
            Debug.Log("Запуск анимации закрытия сцены.");
            _instance.animator.SetTrigger("SceneClosing");
        }
        _instance.StartCoroutine(_instance.LoadSceneWithLoading(targetSceneIndex));
    }

    private IEnumerator LoadSceneWithLoading(int targetSceneIndex)
    {
        ShouldPlayOpeningAnimation = false;
        Debug.Log("Начало загрузки сцены с индексом: " + targetSceneIndex);
        yield return new WaitForSeconds(1f);

        Debug.Log("Загрузка сцены подложки 'LoadingScene'.");
        SceneManager.LoadScene("LoadingScene");

        // Даем немного времени, чтобы сцена подложки успела отобразиться
        yield return new WaitForSeconds(0.5f);
        yield return null;
        Debug.Log("Запуск асинхронной загрузки целевой сцены.");

        loadingSceneOperation = SceneManager.LoadSceneAsync(targetSceneIndex);
        loadingSceneOperation.allowSceneActivation = false;
        while (!loadingSceneOperation.isDone)
        {
            float progress = Mathf.Clamp01(loadingSceneOperation.progress / 0.9f);
            if (LoadingPercentage != null)
            {
                LoadingPercentage.text = Mathf.RoundToInt(progress * 100) + "%";
                Debug.Log("Прогресс загрузки: " + Mathf.RoundToInt(progress * 100) + "%");
            }

            if (LoadingProgress != null)
                LoadingProgress.fillAmount = progress;
            if (loadingSceneOperation.progress >= 0.9f)
            {
                Debug.Log("Загрузка сцены завершена на 90%, ожидаем завершения анимации.");
                yield return new WaitForSeconds(1f);
                break;
            }


            yield return null;
        }
        
        Debug.Log("Загрузка целевой сцены завершена. Активируем сцену.");
        ShouldPlayOpeningAnimation = true;
        loadingSceneOperation.allowSceneActivation = true;
    }

    public void OnAnimationOver()
    {
        Debug.Log("Анимация завершена.");
        if (loadingSceneOperation != null)
        {
            Debug.Log("Разрешаем активацию целевой сцены.");
            loadingSceneOperation.allowSceneActivation = true; 
        }
    }
}
