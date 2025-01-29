using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private GameObject panelScore;
    private bool _isCollected;

    private void OnTriggerEnter(Collider other)
    {
        if (!_isCollected && other.GetComponent<Ball>() != null)
        {
            _isCollected = true;

            if (panelScore != null)
            {
                panelScore.SetActive(true);
            }

            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }
            
            GameEvents.CollectibleCollected();

            Destroy(gameObject);
        }
    }
}