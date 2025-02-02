using UnityEngine;

public class GameStats : MonoBehaviour
{
    public static GameStats Instance;

    public int numberOfLevels = 3; 
    public LevelStats[] levels; 
    private float levelStartTime;
    private bool[] firstEntry;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ResetStats();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetStats()
    {
        levels = new LevelStats[numberOfLevels];
        firstEntry = new bool[numberOfLevels]; // Инициализация массива флагов

        for (int i = 0; i < numberOfLevels; i++)
        {
            levels[i] = new LevelStats(); // Обнуляем показатели уровня
            firstEntry[i] = true; // Устанавливаем флаг первого входа в true
        }
    }

    public void LoadStats()
    {
        levels = new LevelStats[numberOfLevels];
        for (int i = 0; i < numberOfLevels; i++)
        {
            levels[i] = new LevelStats();
            LoadLevelStats(i);
        }
    }


    public void StartLevel(int sceneIndex)
    {
        int levelIndex = ConvertIndex(sceneIndex);
        levelStartTime = Time.time;

        
        if (firstEntry[levelIndex])
        {
            levels[levelIndex].deaths = 0;
            levels[levelIndex].coinsCollected = 0;
            firstEntry[levelIndex] = false; 
        }
    }

    public void EndLevel(int sceneIndex)
    {
        int levelIndex = ConvertIndex(sceneIndex);
        levels[levelIndex].levelTime = Time.time - levelStartTime;
        levels[levelIndex].score = CalculateLevelScore(levelIndex);
        UpdateLevelBest(levelIndex);
    }

    public void AddDeath(int sceneIndex)
    {
        int levelIndex = ConvertIndex(sceneIndex);
        levels[levelIndex].deaths++;
    }

    public void AddRestart(int sceneIndex)
    {
        int levelIndex = ConvertIndex(sceneIndex);
        
        // Увеличиваем количество рестартов
        levels[levelIndex].restarts++;
        
    }

    public void AddCoins(int sceneIndex, int coins)
    {
        int levelIndex = ConvertIndex(sceneIndex);
        levels[levelIndex].coinsCollected += coins;
        Debug.Log($"[GameStats] Добавлено {coins} монет, всего: {levels[levelIndex].coinsCollected}");

        GameEvents.RaiseCoinsUpdated(levels[levelIndex].coinsCollected);
        
    }

    private int ConvertIndex(int sceneIndex)
    {
        return sceneIndex - 1; // Предполагается, что индексы сцен начинаются с 1
    }

    private int CalculateLevelScore(int levelIndex)
    {
        // Константы
        int baseScore = 100;
        int K_coins = 20;
        int K_time = 2;
        int K_restarts = 50;
        int K_deaths = 100;
        int maxPenalty = 300;
        int[] bonuses = { 100, 200, 300 };

        var stats = levels[levelIndex];
        float remainingTime = Mathf.Max(0, 120f - stats.levelTime);

        int penalty = Mathf.Min(
            (stats.restarts * K_restarts) + (stats.deaths * K_deaths),
            maxPenalty
        );

        int score = baseScore +
                    (stats.coinsCollected * K_coins) +
                    (int)(remainingTime * K_time) -
                    penalty;


        if (stats.restarts == 0 && stats.deaths == 0)
        {
            score += bonuses[Mathf.Min(levelIndex, bonuses.Length - 1)];
        }

        return Mathf.Max(score, 0);
    }
    private void UpdateLevelBest(int levelIndex)
    {
        // Log the method entry with the level index

        int savedScore = PlayerPrefs.GetInt($"Level_{levelIndex}_Score", 0);

        var currentStats = levels[levelIndex];

        // Log the comparison of current score and saved score
        Debug.Log($"New score =" + currentStats.score + ", previous score =" + savedScore);

        if (currentStats.score > savedScore)
        {
            // Log when the score is higher and the stats will be saved
            Debug.Log($"Save new Stats");
            SaveLevelStats(levelIndex);
        }
        else
        {
            // Log when no new high score is found
            Debug.Log($"Leving prev stats");
        }

        // Log method exit
        Debug.Log($"Exiting UpdateLevelBest for Level {levelIndex}");
    }


    private void SaveLevelStats(int levelIndex)
    {
        PlayerPrefs.SetInt($"Level_{levelIndex}_Deaths", levels[levelIndex].deaths);
        PlayerPrefs.SetInt($"Level_{levelIndex}_Restarts", levels[levelIndex].restarts);
        PlayerPrefs.SetInt($"Level_{levelIndex}_Coins", levels[levelIndex].coinsCollected);
        PlayerPrefs.SetFloat($"Level_{levelIndex}_Time", levels[levelIndex].levelTime);
        PlayerPrefs.SetInt($"Level_{levelIndex}_Score", levels[levelIndex].score);
        PlayerPrefs.Save(); // Сохраняем изменения в PlayerPrefs
    }

    private void LoadLevelStats(int levelIndex)
    {
        levels[levelIndex].deaths = PlayerPrefs.GetInt($"Level_{levelIndex}_Deaths", 0);
        levels[levelIndex].restarts = PlayerPrefs.GetInt($"Level_{levelIndex}_Restarts", 0);
        levels[levelIndex].coinsCollected = PlayerPrefs.GetInt($"Level_{levelIndex}_Coins", 0);
        levels[levelIndex].levelTime = PlayerPrefs.GetFloat($"Level_{levelIndex}_Time", 0f);
        levels[levelIndex].score = PlayerPrefs.GetInt($"Level_{levelIndex}_Score", 0);
    }
}
