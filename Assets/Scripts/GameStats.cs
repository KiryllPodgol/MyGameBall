using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStats : MonoBehaviour
{
    private static GameStats instance;

    public static GameStats Instance
    {
        get
        {
            if (instance == null)
            {
            var go = new GameObject();  
            instance = go.AddComponent<GameStats>();
            }
            return instance;
        }
    }

private const int DefaultNumberOfLevels = 3;
    public GameStatsData data;
    private float levelStartTime;
    private bool[] firstEntry;
    public event Action<int, int>OnNumberOfCoinsChanged;
    private string SaveFilePath => Path.Combine(Application.persistentDataPath, "game_stats.json");
    private string BackupFilePath => SaveFilePath + ".bak";

    public int CurrentLevel
    {
        get { return SceneManager.GetActiveScene().buildIndex; }
    }

    private void Awake()
    {
        ResetStats();
        if (instance == null)
        {
            instance = this;
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

    public int AddCoins(int coins)
    {
        int sceneIndex = CurrentLevel;
        int levelIndex = ConvertIndex(sceneIndex);
        data.levels[levelIndex].coinsCollected += coins;
        int resultCoins = data.levels[levelIndex].coinsCollected;
        OnNumberOfCoinsChanged?.Invoke(sceneIndex, resultCoins);
        return resultCoins;
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
            string encryptedJson = EncryptAES(json, GenerateDynamicKey());

            // бэкап
            if (File.Exists(SaveFilePath))
            {
                File.Copy(SaveFilePath, BackupFilePath, true);
            }

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

    private bool IsBase64String(string input)
    {
        if (input == null)
            return false;

        Span<byte> buffer = new Span<byte>(new byte[input.Length * 3 / 4]);
        return Convert.TryFromBase64String(input, buffer, out int bytesParsed);
    }

    private void LoadStatsFromFile()
    {
        if (File.Exists(SaveFilePath))
        {
            try
            {
                string encryptedJson = File.ReadAllText(SaveFilePath);
                if (string.IsNullOrWhiteSpace(encryptedJson))
                {
                    throw new Exception("Файл статистики пуст.");
                }

                if (IsBase64String(encryptedJson))
                {
                    // Попытка расшифровки
                    string json = DecryptAES(encryptedJson, GenerateDynamicKey());

                    // Проверяем на корректность JSON
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        throw new Exception("Расшифрованный JSON пуст.");
                    }

                    data = JsonUtility.FromJson<GameStatsData>(json);
                    Debug.Log($"[GameStats] Статистика загружена из {SaveFilePath}");
                }
                else
                {
                    throw new Exception("Ошибка формата Base64: Некорректный формат данных.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[GameStats] Ошибка загрузки: {e.Message}");

                // Если ошибка, восстанавливаем из бэкапа
                if (File.Exists(BackupFilePath))
                {
                    File.Copy(BackupFilePath, SaveFilePath, true);
                    Debug.Log("[GameStats] Восстановление из бэкапа...");
                    LoadStatsFromFile();
                }
                else
                {
                    Debug.LogError("[GameStats] Бэкап не найден. Сброс статистики.");
                    ResetStats();
                }
            }
        }
        else
        {
            Debug.Log("[GameStats] Файл статистики не найден. Создана новая статистика.");
            ResetStats();
        }
    }


    private string GenerateDynamicKey()
    {
        string deviceId = SystemInfo.deviceUniqueIdentifier;
        string appKey = "SpecificKey";
        return Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(deviceId + appKey)))
            .Substring(0, 32);
    }

    private string EncryptAES(string plainText, string key)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] ivBytes = new byte[16];
        Array.Copy(keyBytes, ivBytes, Math.Min(keyBytes.Length, ivBytes.Length));

        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.IV = ivBytes;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                return Convert.ToBase64String(encryptedBytes);
            }
        }
    }

    private string DecryptAES(string encryptedText, string key)
    {
        if (string.IsNullOrWhiteSpace(encryptedText))
        {
            throw new ArgumentException("Входной текст для дешифровки пуст или некорректен.");
        }

        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] ivBytes = new byte[16];
        Array.Copy(keyBytes, ivBytes, Math.Min(keyBytes.Length, ivBytes.Length));

        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.IV = ivBytes;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            try
            {
                using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                    byte[] plainBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    return Encoding.UTF8.GetString(plainBytes);
                }
            }
            catch (FormatException e)
            {
                throw new FormatException($"Ошибка формата Base64: {e.Message}");
            }
            catch (CryptographicException e)
            {
                throw new CryptographicException($"Ошибка расшифровки: {e.Message}");
            }
        }
    }
}