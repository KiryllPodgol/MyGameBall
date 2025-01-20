using UnityEngine;
using TMPro;

public class ResultsUI : MonoBehaviour
{
    public TextMeshProUGUI[] levelTexts;
    public TextMeshProUGUI[] restartTexts;
    public TextMeshProUGUI[] timeTexts;
    public TextMeshProUGUI[] deathsTexts; 
    public TextMeshProUGUI[] coinsTexts;
    public TextMeshProUGUI[] scoreTexts; 

    private void Start()
    {
        UpdateResults();
    }

    public void UpdateResults()
    {
        if (GameStats.Instance != null)
        {
            for (int i = 0; i < 3; i++)
            {
                levelTexts[i].text = $"Уровень {i + 1}";
                restartTexts[i].text = $"Рестартов: {GameStats.Instance.restarts[i]}";
                timeTexts[i].text = $"Время: {GameStats.Instance.levelTimes[i]:F2} сек";
                coinsTexts[i].text = $"Монет: {GameStats.Instance.coinsCollected[i]}";
                deathsTexts[i].text = $"Смертей: {GameStats.Instance.deaths[i]}";
                scoreTexts[i].text = $"Очки: {GameStats.Instance.levelScores[i]}";
            }
        }
    }
}