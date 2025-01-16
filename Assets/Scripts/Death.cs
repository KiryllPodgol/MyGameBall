using UnityEngine;

public class Death : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Ball>(out Ball ballComponent))
        {
            if (gameManager != null)
            {
                gameManager.Death(); 
            }
            else
            {
                Debug.LogWarning("GameManager не назначен в инспекторе!");
            }
        }
    }
}