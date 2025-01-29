using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private Slider volumeSlider;

    private const string VolumePrefKey = "MusicVolume";

    private void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        float savedVolume = PlayerPrefs.GetFloat(VolumePrefKey, 0.5f);
        if (musicSource != null)
        {
            musicSource.volume = savedVolume;
        }
        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(ChangeVolume); 
        }
    }

    private void OnDestroy()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(ChangeVolume);
        }
    }

    public void ChangeVolume(float volume)
    {
        if (musicSource != null)
        {
            GameEvents.VolumeChanged(volume);
            PlayerPrefs.SetFloat(VolumePrefKey, volume); 
            PlayerPrefs.Save();
        }
    }
}