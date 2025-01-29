using UnityEngine;
[ RequireComponent (typeof( AudioSource))]
public class VolumeChangeHandler : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        if (audioSource != null)
        {
            audioSource = GetComponent<AudioSource>();
            GameEvents.OnVolumeChanged += UpdateVolume;
        }
    }

    private void OnDestroy()
    {
        if (audioSource != null)
        {
            // Отписываемся от события
            GameEvents.OnVolumeChanged -= UpdateVolume;
        }
    }

    private void UpdateVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
}