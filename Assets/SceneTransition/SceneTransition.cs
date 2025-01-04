using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public TextMeshProUGUI LoadingProcentage;
    public Image LoadingProgress;
    private static SceneTransition _instance;
    private static bool ShouldPlayOpeningAnimation = false;
    private AsyncOperation loadingSceneOperation;
    private Animator componentAnimator;
    public static void SwitchToScene(int sceneIndex)
    {
        if (_instance == null)
        {
            Debug.LogError("SceneTransition instance is missing! Make sure it's added to the starting scene.");
            return;
        }

        _instance.componentAnimator.SetTrigger("SceneClosing");
        _instance.loadingSceneOperation = SceneManager.LoadSceneAsync(sceneIndex);
        _instance.loadingSceneOperation.allowSceneActivation = false;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        componentAnimator = GetComponent<Animator>();
        if (ShouldPlayOpeningAnimation)
        {
            componentAnimator.SetTrigger("SceneOpening");
        }
    }

    private void Update()
    {
        if (loadingSceneOperation != null)
        {
            LoadingProcentage.text = Mathf.RoundToInt(loadingSceneOperation.progress * 100) + "%";
            LoadingProgress.fillAmount = loadingSceneOperation.progress;
        }
    }

    public void OnAnimationOver()
    {
        ShouldPlayOpeningAnimation = true;
        if (loadingSceneOperation != null)
        {
            loadingSceneOperation.allowSceneActivation = true;
        }
    }
}