using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class MenuPageUI : MonoBehaviour
{
    public string PageName;
    public bool isFadeInverted;
    public Color BackgroundColor;

    private CanvasGroup MenuPage;
    
    private bool isFadingIn;
    private bool isFadingOut;

    private Vector2 defaultPosition = new Vector2(0, -87.5f);
    private Vector2 fadeRightPos = new Vector2(1900f, -87.5f);
    private Vector2 fadeLeftPos = new Vector2(-1900f, -87.5f);

    private RectTransform rect;

    private bool currentfadeInverted; //This variable can be changed if another page has inverted fading (it's not the same as isFadeInverted)

    private Image backgroundToFade;
    private Color currentBackgroundColor;
    private bool isFadingBackground;
    
    private void Start()
    {
        MenuPage = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        Fading();
    }

    public bool IsFading()
    {
        return isFadingIn || isFadingOut;
    }
    
    public void FadeIn(bool inverted)
    {
        if(MenuPage == null) MenuPage = GetComponent<CanvasGroup>();
        if(rect == null) rect = GetComponent<RectTransform>();

        MenuPage.alpha = 0;
        
        currentfadeInverted = inverted;
        
        isFadingOut = false;
        isFadingIn = true;

        gameObject.SetActive(true);
        rect.localPosition = currentfadeInverted ? fadeLeftPos : fadeRightPos;
    }
    
    public void FadeOut(bool inverted)
    {
        MenuPage.alpha = 1;

        currentfadeInverted = inverted;
        
        isFadingIn = false;
        isFadingOut = true;
    }

    private void Fading()
    {
        if (isFadingIn)
        {
            if (MenuPage.alpha < 1)
            {
                MenuPage.alpha += Time.deltaTime * MenuPagesManagerUI.FadeDuration;
                rect.localPosition = Vector2.Lerp(rect.localPosition, defaultPosition, MenuPage.alpha);
                if (MenuPage.alpha >= 1)
                {
                    isFadingIn = false;
                }
            }
            else isFadingIn = false;
        }
        
        if (isFadingOut)
        {
            if (MenuPage.alpha >= 0)
            {
                MenuPage.alpha -= Time.deltaTime * MenuPagesManagerUI.FadeDuration;
                rect.localPosition = Vector2.Lerp(rect.localPosition , currentfadeInverted ? fadeRightPos : fadeLeftPos, 1 - MenuPage.alpha);
                if (MenuPage.alpha == 0)
                {
                    gameObject.SetActive(false);
                    isFadingOut = false;
                }
            }
            else isFadingOut = false;
        }

        if (isFadingBackground)
        {
            float t = Time.deltaTime * MenuPagesManagerUI.FadeDuration;
            if (t < 1)
            {
                backgroundToFade.color = Color.Lerp(currentBackgroundColor, BackgroundColor, t);
                if (t >= 1)
                {
                    isFadingBackground = false;
                }
            }
            else isFadingIn = false;
        }
    }

    public void FadeBackground(Image background, Color color) //ONLY CALL IF THIS PAGE IS FADING IN
    {
        backgroundToFade = background;
        currentBackgroundColor = color;

        isFadingBackground = true;
    }
}
