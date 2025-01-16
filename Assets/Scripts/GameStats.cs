using UnityEngine;

public class GameStats : MonoBehaviour
{
    public static GameStats Instance;

    public int[] deaths; // Смерти на уровнях
    public int[] restarts; // Рестарты на уровнях
    public int[] coinsCollected; // Собранные монеты на уровнях
    public float[] levelTimes; // Время прохождения уровней
    private float levelStartTime; // Время начала уровня

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Сохранение объекта между сценами
            InitializeStats();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeStats()
    {
        deaths = new int[3];
        restarts = new int[3];
        coinsCollected = new int[3];
        levelTimes = new float[3];
    }

    public void StartLevel(int levelIndex)
    {
        levelStartTime = Time.time; // Запоминаем время начала уровня
    }

    public void EndLevel(int levelIndex)
    {
        levelTimes[levelIndex] = Time.time - levelStartTime; // Сохраняем время уровня
    }

    public void AddDeath(int levelIndex)
    {
        deaths[levelIndex]++;
    }

    public void AddRestart(int levelIndex)
    {
        restarts[levelIndex]++;
    }

    public void AddCoins(int levelIndex, int coins)
    {
        coinsCollected[levelIndex] += coins;
    }
}