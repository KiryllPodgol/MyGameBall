using UnityEngine;
using TMPro;

public class LevelResult : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI restartText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI deathsText;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI scoreText;

    public void SetLevelResult(LevelStats stats, int levelIndex)
    {
        levelText.text = $"Уровень {levelIndex + 1}";
        restartText.text = $"{stats.restarts}";
        timeText.text = $"{stats.levelTime:F2} сек";
        deathsText.text = $"{stats.deaths}";
        coinsText.text = $"{stats.coinsCollected}";
        scoreText.text = $"{stats.score}";
    }
}