using UnityEditor;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [Header("Coin Settings")]
    [SerializeField] private GameObject coinPrefab; 
    [SerializeField] private Transform[] spawnPoints;  
    [SerializeField] private int numberOfCoins = 5;

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
                Instantiate(coinPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
            }
        }
        else
        {
            Debug.LogError("Coin Prefab or Spawn Points not assigned!");
        }
    }
}