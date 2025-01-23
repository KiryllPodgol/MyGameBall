[System.Serializable]
public class LevelStats
{
    public int deaths;
    public int restarts;
    public int coinsCollected;
    public float levelTime;
    public int score;

    public LevelStats()
    {
        deaths = 0;
        restarts = 0;
        coinsCollected = 0;
        levelTime = 0f;
        score = 0;
    }
}