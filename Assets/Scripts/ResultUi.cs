using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ResultsUI : MonoBehaviour
{
    public LevelResult levelResultPrefab;
    public Transform resultsContainer;
    private List<LevelResult> levelResultInstances = new List<LevelResult>();

    private void Start()
    {
        UpdateResults();
    }

    public void UpdateResults()
    {
        if (GameStats.Instance != null && levelResultPrefab != null && resultsContainer != null)
        {
            // Удаляем старые префабы
            foreach (var instance in levelResultInstances)
            {
                Destroy(instance);
            }

            levelResultInstances.Clear();

            int levelsToShow = GameStats.Instance.levels.Length;
            float height = 100f;

            for (int i = 0; i < levelsToShow; i++)
            {
                var levelStats = GameStats.Instance.levels[i];
                LevelResult levelResult = Instantiate(levelResultPrefab, resultsContainer);
                levelResult.SetLevelResult(levelStats, i);
                levelResultInstances.Add(levelResult);

                RectTransform rectTransform = levelResult.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(0, -height * i);
            }
        }
        else
        {
            Debug.LogError("levelResultPrefab или resultsContainer не назначены в инспекторе.");
        }
    }
}