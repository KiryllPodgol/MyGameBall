using UnityEngine;
public class CoinSpawner : MonoBehaviour
{
    [Header("Coin Settings")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int numberOfCoins = 5;
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 100f;

    private void Start()
    {
        SpawnCoins();
    }

    private void SpawnCoins()
    {
        if (coinPrefab != null && spawnPoints.Length > 0)
        {
            int coinsToSpawn = Mathf.Min(numberOfCoins, spawnPoints.Length);

            for (int i = 0; i < coinsToSpawn; i++)
            {
              
                GameObject spawnedCoin = Instantiate(coinPrefab, spawnPoints[i].position, spawnPoints[i].rotation);

          
                spawnedCoin.AddComponent<RollingCoin>().rotationSpeed = rotationSpeed;
            }
        }
        else
        {
            Debug.LogError("Coin Prefab or Spawn Points not assigned!");
        }
    }
    
    private class RollingCoin : MonoBehaviour
    {
        public float rotationSpeed;

        private void Update()
        {
            transform.Rotate(Vector3.forward * (rotationSpeed * Time.deltaTime), Space.Self);
        }
    }
}