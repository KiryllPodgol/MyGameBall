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
            int levelsToShow = Mathf.Min(levelTexts.Length, GameStats.Instance.levelScores.Length);

            for (int i = 0; i < levelsToShow; i++)
            {
                levelTexts[i].text = $"Уровень {i + 1}";
                restartTexts[i].text = $" {GameStats.Instance.restarts[i]}";
                timeTexts[i].text = $" {GameStats.Instance.levelTimes[i]:F2} сек";
                coinsTexts[i].text = $" {GameStats.Instance.coinsCollected[i]}";
                deathsTexts[i].text = $"{GameStats.Instance.deaths[i]}";
                scoreTexts[i].text = $" {GameStats.Instance.levelScores[i]}";
            }

            for (int i = levelsToShow; i < levelTexts.Length; i++)
            {
                levelTexts[i].gameObject.SetActive(false);
                restartTexts[i].gameObject.SetActive(false);
                timeTexts[i].gameObject.SetActive(false);
                coinsTexts[i].gameObject.SetActive(false);
                deathsTexts[i].gameObject.SetActive(false);
                scoreTexts[i].gameObject.SetActive(false);
            }
        }
    }
}