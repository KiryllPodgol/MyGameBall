using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private GameObject panelScore;
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

            if (panelScore != null)
            {
                panelScore.SetActive(true);
            }
            
            if (collectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(collectSound);
            }
            
            GameEvents.CollectibleCollected();

            Destroy(gameObject);
        }
    }
}