using System;
using UnityEngine;

[Serializable]
public class GameStatsData
{
    public int numberOfLevels = 3;
    public LevelStats[] levels;

    public GameStatsData(int numberOfLevels)
    {
        this.numberOfLevels = numberOfLevels;
        levels = new LevelStats[numberOfLevels];

        for (int i = 0; i < numberOfLevels; i++)
        {
            levels[i] = new LevelStats();
        }
    }
}