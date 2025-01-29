using System;

public static class GameEvents
{
  
    public static event Action OnCollectibleCollected;

    public static void CollectibleCollected()
    {
        OnCollectibleCollected?.Invoke();
    }
    
    public static event Action<float> OnVolumeChanged;

    public static void VolumeChanged(float newVolume)
    {
        OnVolumeChanged?.Invoke(newVolume);
    }
}