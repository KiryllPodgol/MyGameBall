using UnityEngine;

public class GameStats : MonoBehaviour
{
    public static GameStats Instance;

    public int numberOfLevels = 3; 
    public LevelStats[] levels; 
    private float levelStartTime;

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
        levels = new LevelStats[numberOfLevels];
        for (int i = 0; i < numberOfLevels; i++)
        {
            levels[i] = new LevelStats();
        }
    }

    public void StartLevel(int sceneIndex)
    {
        int levelIndex = ConvertIndex(sceneIndex);
        levelStartTime = Time.time;
    }

    public void EndLevel(int sceneIndex)
    {
        int levelIndex = ConvertIndex(sceneIndex);
        levels[levelIndex].levelTime = Time.time - levelStartTime;
        levels[levelIndex].score = CalculateLevelScore(levelIndex);
    }

    public void AddDeath(int sceneIndex)
    {
        int levelIndex = ConvertIndex(sceneIndex);
        levels[levelIndex].deaths++;
    }

    public void AddRestart(int sceneIndex)
    {
        int levelIndex = ConvertIndex(sceneIndex);
        levels[levelIndex].restarts++;
    }

    private int ConvertIndex(int sceneIndex)
    {
        return sceneIndex - 1;
    }

    public void AddCoins(int levelIndex, int coins)
    {
        levels[ConvertIndex(levelIndex)].coinsCollected += coins;
    }

    private int CalculateLevelScore(int levelIndex)
    {
        // Константы
        int baseScore = 100;        
        int K_coins = 10;           
        int K_time = 2;             
        int K_restarts = 50;        
        int K_deaths = 100;         
        int maxPenalty = 300;       
        int[] bonuses = { 100, 200, 300 }; 

        // Данные уровня
        var stats = levels[levelIndex];
        
        float remainingTime = Mathf.Max(0, 120f - stats.levelTime);
        
        // Расчёт штрафов (с ограничением)
        int penalty = (stats.restarts * K_restarts) + (stats.deaths * K_deaths);
        penalty = Mathf.Min(penalty, maxPenalty);

        int score = baseScore +
                    (stats.coinsCollected * K_coins) +
                    (int)(remainingTime * K_time) -
                    penalty;

        // Добавление бонуса за уровень без смертей и рестартов
        if (stats.restarts == 0 && stats.deaths == 0)
        {
            score += bonuses[Mathf.Min(levelIndex, bonuses.Length - 1)];
        }

        return Mathf.Max(score, 0);
    }
}
