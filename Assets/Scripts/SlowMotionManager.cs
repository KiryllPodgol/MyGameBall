using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class SlowMotionManager : MonoBehaviour
{
    [SerializeField] private float slowMotionTimeScale = 0.2f;
    [SerializeField] private float vignetteTransitionDuration = 0.5f;
    [SerializeField] private Volume volume;

    private Vignette _vignette;
    private InputAsset _input;
    private Coroutine _vignetteCoroutine;
    private Coroutine _slowMotionCoroutine;

    public bool IsSlowMotionActive;

    private void Awake()
    {
        _input = new InputAsset();
        if (volume.profile.TryGet<Vignette>(out _vignette))
        {
            _vignette.intensity.value = 0f;
        }
        else
        {
            Debug.LogWarning("Виньетка не найдена в профиле постпроцессинга!");
        }
    }

    private void OnEnable()
    {
        _input.Gameplay.SlowMotion.started += _ => ToggleSlowMotion(true);
        _input.Gameplay.SlowMotion.canceled += _ => ToggleSlowMotion(false);
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Gameplay.SlowMotion.started -= _ => ToggleSlowMotion(true);
        _input.Gameplay.SlowMotion.canceled -= _ => ToggleSlowMotion(false);
        _input.Disable();

       
        if (_slowMotionCoroutine != null) StopCoroutine(_slowMotionCoroutine);
        if (_vignetteCoroutine != null) StopCoroutine(_vignetteCoroutine);

        Time.timeScale = 1f;
        if (_vignette != null) _vignette.intensity.value = 0f;
    }

    private void ToggleSlowMotion(bool isActive)
    {
        IsSlowMotionActive = isActive;
        if (_slowMotionCoroutine != null) StopCoroutine(_slowMotionCoroutine);
        _slowMotionCoroutine = StartCoroutine(SmoothSlowMotion(
            IsSlowMotionActive ? slowMotionTimeScale : 1f, 
            vignetteTransitionDuration
        ));
        if (_vignetteCoroutine != null) StopCoroutine(_vignetteCoroutine);
        if (_vignette != null)
        {
            _vignetteCoroutine = StartCoroutine(SmoothVignetteIntensity(
                IsSlowMotionActive ? 0.4f : 0f, 
                vignetteTransitionDuration
            ));
        }
    }
    private IEnumerator SmoothSlowMotion(float targetTimeScale, float duration)
    {
        float startScale = Time.timeScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(startScale, targetTimeScale, elapsed / duration);
            yield return null;
        }

        Time.timeScale = targetTimeScale;
    }

    private IEnumerator SmoothVignetteIntensity(float targetIntensity, float duration)
    {
        float startIntensity = _vignette.intensity.value;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            _vignette.intensity.value = Mathf.Lerp(startIntensity, targetIntensity, elapsed / duration);
            yield return null;
        }

        _vignette.intensity.value = targetIntensity;
    }
}
