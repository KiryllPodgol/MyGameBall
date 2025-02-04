    using UnityEngine;
    using TMPro;
    using UnityEngine.SceneManagement;

    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameObject panelScore;
        [SerializeField] private TMP_Text scoreText;
        private void OnEnable()
        {
            // Debug.Log("[UIController] Подписка на OnCoinsUpdated");
         
            GameEvents.OnCollectibleCollected += AddCoins;
        }
        private void OnDisable()
        {
            // Debug.Log("[UIController] Отписка от OnCoinsUpdated");
            GameEvents.OnCollectibleCollected -= AddCoins;
        }
        private void AddCoins()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int coinscount = GameStats.Instance.AddCoins(currentSceneIndex,1);
            UpdatePanelScore(coinscount);
            
        }
        private void UpdatePanelScore(int newCoinCount)
        {
            // Debug.Log($"[UIController] UpdatePanelScore вызван с {newCoinCount}");

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