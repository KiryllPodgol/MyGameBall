using UnityEngine;
using TMPro;

public class ResultsUI : MonoBehaviour
{
    public TextMeshProUGUI[] levelTexts; // Ссылки на строки "Уровень"
    public TextMeshProUGUI[] restartTexts; // Ссылки на строки "Рестартов"
    public TextMeshProUGUI[] timeTexts; // Ссылки на строки "Время"
    public TextMeshProUGUI[] deathsTexts;
    public TextMeshProUGUI[] coinsTexts; // Ссылки на строки "Собрано монет"
    

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
                restartTexts[i].text = GameStats.Instance.restarts[i].ToString();
                timeTexts[i].text = GameStats.Instance.levelTimes[i].ToString("F2") + " сек";
                coinsTexts[i].text = GameStats.Instance.coinsCollected[i].ToString();
                deathsTexts[i].text = GameStats.Instance.deaths[i].ToString();
                
            }
        }
    }
}