using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private AudioClip collectSound; 
    [SerializeField] private GameObject panelScore; 
    [SerializeField] private GameManager gameManager; 
    private bool _isCollected; 
    public static event System.Action OnCollected;

    private void OnTriggerEnter(Collider other)
    { 
        if (!_isCollected && other.GetComponent<Ball>() != null)
        {
            _isCollected = true;
            if (panelScore != null)
            {
                panelScore.SetActive(true);
            }
            
            if (OnCollected != null)
            {
                OnCollected.Invoke();
            }
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }
            
            Destroy(gameObject);
        }
    }
}