using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectorUIScript : MonoBehaviour
{
    [SerializeField] MenuLevelUIScript menuLevelPrefab;
    [SerializeField] Transform levelsTransform;
    [SerializeField] bool findCoopLevels;
    [SerializeField] bool isOnline;
    
    private LevelData[] levels = new LevelData[20];
    private COOPLevelData[] coopLevels = new COOPLevelData[20];

    void Start()
    {
        if(!findCoopLevels)
        {
            levels = Resources.LoadAll<LevelData>("Levels");

            foreach (LevelData level in levels)
            {
                if(level.LevelSprite != null)
                {
                    var newLevel = Instantiate(menuLevelPrefab, levelsTransform);
                    newLevel.SetLevelInfo(level.LevelSprite, level.name);
                    newLevel.GetComponent<Button>().onClick.AddListener(() => GameObject.FindObjectOfType<MainMenu>().PlayLevel(level.name));
                }
            }
        }
        else
        {
            if(!isOnline)
            {
                coopLevels = Resources.LoadAll<COOPLevelData>("Levels/CO-OP/Local");

                foreach (COOPLevelData level in coopLevels)
                {
                    if(level.LevelSprite != null)
                    {                        
                        var newLevel = Instantiate(menuLevelPrefab, levelsTransform);
                        newLevel.SetLevelInfo(level.LevelSprite, level.name);

                        newLevel.GetComponent<Button>().onClick.AddListener(() => GameObject.FindObjectOfType<LobbyManager>().PlayLevel(level.name));
                    }
                }
            }
            else
            {
                coopLevels = Resources.LoadAll<COOPLevelData>("Levels/CO-OP/Online");

                foreach (COOPLevelData level in coopLevels)
                {
                    if(level.LevelSprite != null)
                    {                        
                        var newLevel = Instantiate(menuLevelPrefab, levelsTransform);
                        newLevel.SetLevelInfo(level.LevelSprite, level.name);

                        newLevel.GetComponent<Button>().onClick.AddListener(() => GameObject.FindObjectOfType<MainMenu>().PlayLevel(level.name));
                    }
                }
            }

        }
    }
}
