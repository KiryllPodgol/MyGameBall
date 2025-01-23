using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ResultsUI : MonoBehaviour
{
    public GameObject levelResultPrefab;
    public Transform resultsContainer;

    private List<GameObject> levelResultInstances = new List<GameObject>();

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
                GameObject instance = Instantiate(levelResultPrefab, resultsContainer);
                LevelResult levelResult = instance.GetComponent<LevelResult>();
                levelResult.SetLevelResult(levelStats, i);
                levelResultInstances.Add(instance);

                RectTransform rectTransform = instance.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(0, -height * i);
            }
        }
        else
        {
            Debug.LogError("levelResultPrefab или resultsContainer не назначены в инспекторе.");
        }
    }
}