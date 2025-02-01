using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private AudioClip collectSound;
    private AudioSource audioSource;
    private bool _isCollected;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isCollected && other.GetComponent<Ball>() != null)
        {
            _isCollected = true;
            GameEvents.CollectibleCollected();
            if (collectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(collectSound);
            }

            GetComponent<Renderer>().enabled = false;
            Destroy(gameObject, collectSound.length);
        }
    }
}

