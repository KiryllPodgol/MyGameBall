using System;

public static class GameEvents
{
  
    public static event Action OnCollectibleCollected;

    public static void CollectibleCollected()
    {
        OnCollectibleCollected?.Invoke();
    }
    public static event Action<int> OnCoinsUpdated;

    public static void RaiseCoinsUpdated(int newCoinCount)
    {
        OnCoinsUpdated?.Invoke(newCoinCount);
    }
    public static event Action<float> OnSensitivityChanged;

    public static void ChangeSensitivity(float value)
    {
        OnSensitivityChanged?.Invoke(value);
    }

    
    public static event Action<float> OnVolumeChanged;

    public static void VolumeChanged(float newVolume)
    {
        OnVolumeChanged?.Invoke(newVolume);
    }
}