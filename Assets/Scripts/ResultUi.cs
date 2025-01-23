using UnityEngine;
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
        if (GameStats.Instance != null)
        {
            // Удаляем старые префабы
            foreach (var instance in levelResultInstances)
            {
                Destroy(instance);
            }
            levelResultInstances.Clear();

            int levelsToShow = GameStats.Instance.levels.Length;

            for (int i = 0; i < levelsToShow; i++)
            {
                var levelStats = GameStats.Instance.levels[i];
                GameObject instance = Instantiate(levelResultPrefab, resultsContainer);
                LevelResult levelResult = instance.GetComponent<LevelResult>();
                levelResult.SetLevelResult(levelStats, i);
                levelResultInstances.Add(instance);
            }
        }
    }
}