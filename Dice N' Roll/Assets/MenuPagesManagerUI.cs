using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPagesManagerUI : MonoBehaviour
{
    [SerializeField] private List<MenuPageUI> menuPages = new List<MenuPageUI>();
    [SerializeField] private Image Background;
    
    private MenuPageUI currentMenuPage;
    private MenuPageUI menuPageToOpen;
    public static float FadeDuration = 1f;
    
    private void Start()
    {
        foreach (MenuPageUI page in menuPages) page.gameObject.SetActive(false);

        currentMenuPage = GetPageByName("Main");
        currentMenuPage.gameObject.SetActive(true);
    }

    public void OpenPage(string pageName)
    {
        menuPageToOpen = GetPageByName(pageName);

        if (menuPageToOpen != null)
        {
            bool inverted = currentMenuPage.isFadeInverted;
            
            if (currentMenuPage != null)
            {
                currentMenuPage.FadeOut(inverted);
            }
            menuPageToOpen.FadeIn(inverted);
            menuPageToOpen.FadeBackground(Background, currentMenuPage.BackgroundColor);

            currentMenuPage = menuPageToOpen;
        }
    }

    private MenuPageUI GetPageByName(string name)
    {
        foreach (MenuPageUI page in menuPages)
        {
            if(page.PageName == name) return page;
        }

        return null;
    }
}
