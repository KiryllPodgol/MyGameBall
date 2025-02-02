using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject panelScore;
    [SerializeField] private TMP_Text scoreText;

    private void OnEnable()
    {
        Debug.Log("[UIController] Подписка на OnCoinsUpdated");
        GameEvents.OnCoinsUpdated += UpdatePanelScore;
    }

    private void OnDisable()
    {
        Debug.Log("[UIController] Отписка от OnCoinsUpdated");
        GameEvents.OnCoinsUpdated -= UpdatePanelScore;
    }

    private void UpdatePanelScore(int newCoinCount)
    {
        Debug.Log($"[UIController] UpdatePanelScore вызван с {newCoinCount}");

        if (panelScore != null)
        {
            panelScore.SetActive(true);
        }

        if (scoreText != null)
        {
            scoreText.text = $"Монеты: {newCoinCount}";
        }
    }
}