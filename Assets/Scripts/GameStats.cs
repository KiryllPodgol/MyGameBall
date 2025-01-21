using UnityEngine;

public class GameStats : MonoBehaviour
{
    public static GameStats Instance;

    public int[] deaths; 
    public int[] restarts; 
    public int[] coinsCollected;
    public float[] levelTimes; 
    private float levelStartTime;
    public int[] levelScores; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
        levelScores = new int[3];
    }

    public void StartLevel(int sceneIndex)
    {
        int levelIndex = ConvertIndex(sceneIndex);
        levelStartTime = Time.time;
    }

    public void EndLevel(int sceneIndex)
    {
        int levelIndex = ConvertIndex(sceneIndex);
        levelTimes[levelIndex] = Time.time - levelStartTime;
        levelScores[levelIndex] = CalculateLevelScore(levelIndex);
    }

    public void AddDeath(int sceneIndex)
    {
        int levelIndex = ConvertIndex(sceneIndex);
        deaths[levelIndex]++;
    }

    public void AddRestart(int sceneIndex)
    {
        int levelIndex = ConvertIndex(sceneIndex);
        restarts[levelIndex]++;
    }
    private int ConvertIndex(int sceneIndex)
    {
        return sceneIndex - 1;
    }

    public void AddCoins(int levelIndex, int coins)
    {
        coinsCollected[ConvertIndex(levelIndex)] += coins;
    }


    private int CalculateLevelScore(int levelIndex)
    {
        // Константы
        int baseScore = 100;        // Базовые очки за уровень
        int K_coins = 10;           // Множитель для монет
        int K_time = 2;             // Множитель для оставшегося времени
        int K_restarts = 50;        // Штраф за рестарты
        int K_deaths = 100;         // Штраф за смерти
        int maxPenalty = 300;       // Максимальный штраф
        int[] bonuses = { 100, 200, 300 }; // Фиксированные бонусы

        // Данные уровня
        int coins = coinsCollected[levelIndex];
        float remainingTime = Mathf.Max(0, 120f - levelTimes[levelIndex]);
        int restartsCount = restarts[levelIndex];
        int deathsCount = deaths[levelIndex];

        // Расчёт штрафов (с ограничением)
        int penalty = (restartsCount * K_restarts) + (deathsCount * K_deaths);
        penalty = Mathf.Min(penalty, maxPenalty);

      
        int score = baseScore + 
                    (coins * K_coins) + 
                    (int)(remainingTime * K_time) - 
                    penalty;

        // Добавление бонуса за уровень без смертей и рестартов
        if (restartsCount == 0 && deathsCount == 0)
        {
            score += bonuses[levelIndex];
        }

        return score > 0 ? score : 0;
    }
}
