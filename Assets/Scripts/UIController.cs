using UnityEngine;
using TMPro;
public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject panelScore;
    [SerializeField] private TMP_Text scoreText; 
    private int coinCount = 0;

    private void OnEnable()
    {
        GameEvents.OnCollectibleCollected += UpdatePanelScore;
    }

    private void OnDisable()
    {
        GameEvents.OnCollectibleCollected -= UpdatePanelScore;
    }

    private void UpdatePanelScore()
    {
        if (panelScore != null)
        {
            panelScore.SetActive(true);
        }

        coinCount++;
        if (scoreText != null)
        {
            scoreText.text = $"Монеты: {coinCount}";
        }
    }
}