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
                restartTexts[i].text = $" {GameStats.Instance.restarts[i]}";
                timeTexts[i].text = $" {GameStats.Instance.levelTimes[i]:F2} сек";
                coinsTexts[i].text = $" {GameStats.Instance.coinsCollected[i]}";
                deathsTexts[i].text = $"{GameStats.Instance.deaths[i]}";
                scoreTexts[i].text = $" {GameStats.Instance.levelScores[i]}";
            }
        }
    }
}