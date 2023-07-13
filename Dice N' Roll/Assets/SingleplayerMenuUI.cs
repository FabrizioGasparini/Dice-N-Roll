using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleplayerMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelFrameUI levelPrefab;
    [SerializeField] private List<LevelData> availableLevels;
    [SerializeField] private Transform levelsParent;

    private void Start()
    {
        GenerateLevels();
    }

    private void GenerateLevels()
    {
        foreach (LevelData level in availableLevels)
        {
            LevelFrameUI newLevel = Instantiate(levelPrefab, levelsParent, true);
            newLevel.name = level.name;
            
            newLevel.UpdateLevelFrame(level.LevelSprite, level.name, false);
        }
    }
}