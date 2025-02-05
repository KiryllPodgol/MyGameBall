using System;
using System.IO;
using System.Text;
using UnityEngine;

public class GameStats : MonoBehaviour
{
    public static GameStats Instance;
    private const int DefaultNumberOfLevels = 3;
    public GameStatsData data;
    private float levelStartTime;
    private bool[] firstEntry;
    private const string EncryptionKey = "apsfjkawjjw123dfk";

    private string SaveFilePath => Path.Combine(Application.persistentDataPath, "game_stats.json");

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadStatsFromFile();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void ResetStats()
    {
        data = new GameStatsData(DefaultNumberOfLevels);
        firstEntry = new bool[data.numberOfLevels];

        for (int i = 0; i < firstEntry.Length; i++)
        {
            firstEntry[i] = true;
        }
    }

    public void StartLevel(int sceneIndex)
    {
        int levelIndex = ConvertIndex(sceneIndex);
        levelStartTime = Time.time;

        if (firstEntry[levelIndex])
        {
            data.levels[levelIndex].deaths = 0;
            data.levels[levelIndex].coinsCollected = 0;
            firstEntry[levelIndex] = false;
        }
    }

    public void EndLevel(int sceneIndex)
    {
        int levelIndex = ConvertIndex(sceneIndex);
        data.levels[levelIndex].levelTime = Time.time - levelStartTime;
        data.levels[levelIndex].score = CalculateLevelScore(levelIndex);
        SaveStatsToFile();
    }

    public void AddDeath(int sceneIndex)
    {
        int levelIndex = ConvertIndex(sceneIndex);
        data.levels[levelIndex].deaths++;
    }

    public int AddCoins(int sceneIndex, int coins)
    {
        int levelIndex = ConvertIndex(sceneIndex);
        data.levels[levelIndex].coinsCollected += coins;
        return data.levels[levelIndex].coinsCollected;
    }

    public void AddRestart(int sceneIndex)
    {
        int levelIndex = ConvertIndex(sceneIndex);
        data.levels[levelIndex].restarts++;
        data.levels[levelIndex].coinsCollected = 0;
    }

    private int ConvertIndex(int sceneIndex) => sceneIndex - 1;

    private int CalculateLevelScore(int levelIndex)
    {
        int baseScore = 100;
        int K_coins = 20;
        int K_time = 2;
        int K_restarts = 50;
        int K_deaths = 100;
        int maxPenalty = 300;
        int[] bonuses = { 100, 200, 300 };

        var stats = data.levels[levelIndex];
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

    private void SaveStatsToFile()
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            string encryptedJson = XOREncryptDecrypt(json, EncryptionKey);
            File.WriteAllText(SaveFilePath, encryptedJson);
            Debug.Log($"[GameStats] Статистика сохранена в {SaveFilePath}");
        }
        catch (IOException e)
        {
            Debug.LogError($"[GameStats] Ошибка сохранения: {e.Message}");
        }
    }

    public void LoadStats()
    {
        LoadStatsFromFile();
    }

    private void LoadStatsFromFile()
    {
        if (File.Exists(SaveFilePath))
        {
            try
            {
                string encryptedJson = File.ReadAllText(SaveFilePath);
                Debug.Log("[GameStats] Зашифрованные данные из файла: " + encryptedJson);

                string json = XOREncryptDecrypt(encryptedJson, EncryptionKey);
                Debug.Log("[GameStats] JSON после расшифровки: " + json);

                data = JsonUtility.FromJson<GameStatsData>(json);
                Debug.Log($"[GameStats] Статистика загружена из {SaveFilePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[GameStats] Ошибка загрузки: {e.Message}");
                File.Delete(SaveFilePath);
                Debug.Log("[GameStats] Старый файл удален.");

                ResetStats();
            }
        }
        else
        {
            Debug.Log("[GameStats] Файл статистики не найден. Создана новая статистика.");
            ResetStats();
        }
    }

    private string XOREncryptDecrypt(string input, string key)
    {
        StringBuilder output = new StringBuilder(input.Length);
        for (int i = 0; i < input.Length; i++)
        {
            output.Append((char)(input[i] ^ key[i % key.Length]));
        }

        return output.ToString();
    }
}