    using UnityEngine;
    using TMPro;
    using UnityEngine.SceneManagement;

    public class UIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;
        private void OnEnable()
        {
            // Debug.Log("[UIController] Подписка на OnCoinsUpdated");
            GameStats.Instance.OnNumberOfCoinsChanged += UpdatePanelScore;
            
        }
        private void OnDisable()
        {
            // Debug.Log("[UIController] Отписка от OnCoinsUpdated");
            GameStats.Instance.OnNumberOfCoinsChanged -= UpdatePanelScore;
        }
        private void UpdatePanelScore(int level ,int newCoinCount)
        {
            // Debug.Log($"[UIController] UpdatePanelScore вызван с {newCoinCount}");
            
            if (scoreText != null)
            {
                scoreText.text = $"Монеты: {newCoinCount}";
            }
        }
    }