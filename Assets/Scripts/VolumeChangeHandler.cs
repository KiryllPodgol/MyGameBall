using UnityEngine;
[ RequireComponent (typeof( AudioSource))]
public class VolumeChangeHandler : MonoBehaviour
{ 
    private AudioSource audioSource;
    private void Awake()
    {
            audioSource = GetComponent<AudioSource>();
            GameEvents.OnVolumeChanged += UpdateVolume;
        
    }

    private void OnDestroy()
    {
        if (audioSource != null)
        {
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