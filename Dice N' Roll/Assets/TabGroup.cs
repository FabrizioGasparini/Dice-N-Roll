using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TabGroup : MonoBehaviour
{
    [Header("Tabs")]
    [Tooltip("Auto Updates when Game Starts")] 
    public List<TabButtonScript> tabButtons;
    public List<GameObject> pagesToSwap;
    
    [Header("Tabs Settings")]
    public Color32 tabIdle;
    public Color32 tabHover;
    public Color32 tabSelect;
    
    private TabButtonScript selectedTab;
    

    public void Subscribe(TabButtonScript tab)
    {
        tabButtons ??= new List<TabButtonScript>();

        tabButtons.Add(tab);
        
        if(selectedTab == null) OnTabSelected(tab);
    }

    public void OnTabEnter(TabButtonScript tab)
    {
        ResetTabs();
        if (selectedTab != null && tab == selectedTab) return;
        tab.background.color = tabHover;
    }
    
    public void OnTabExit(TabButtonScript tab)
    {
        ResetTabs();
    }
    
    public void OnTabSelected(TabButtonScript tab)
    {
        selectedTab = tab;
        ResetTabs();
        tab.background.color = tabSelect;

        int index = tab.transform.GetSiblingIndex();

        for (int i = 0; i < pagesToSwap.Count; i++)
        {
            pagesToSwap[i].SetActive(i == index);
        }
    }

    private void ResetTabs()
    {
        foreach (TabButtonScript tab in tabButtons)
        {
            if (selectedTab != null && tab == selectedTab) continue;
            tab.background.color = tabIdle;
        }
    }
}
