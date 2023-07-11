using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class TileFrameUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Settings")] 
    [SerializeField] private bool canScale = true;

    private Vector2 defaultScale = Vector2.one;
    private Vector2 hoverScale = Vector2.one * 1.05f;
    private Vector2 clickScale = Vector2.one * 0.975F;
    
    private float openHeight = 625f;
    private float closedHeight = 135f;
    private float scaleDuration = 10f; 

    [Header("References")] 
    [SerializeField] private TileFrameUI oppositeTileFrame;
    [SerializeField] private bool isMainTileFrame;
    [Space(10)] 
    [SerializeField] private GameObject MainFrame;
    [SerializeField] private GameObject InfoFrame;
    
    private bool isHovering;
    private bool hasClicked;
    private bool isUpdatingHeight;
    private bool isUpdatingInfo;
    private RectTransform rect;

    private Coroutine updateCoroutine;

    private void Start()
    {
        rect = transform.GetComponent<RectTransform>();
    }

    private void Update()
    {
        UpdateFrameScale();
    }

    private void UpdateFrameScale()
    {
        Vector2 scale = defaultScale;
        if(isHovering) scale = hoverScale;
        if(hasClicked) scale = clickScale;
        
        transform.localScale = Vector2.Lerp(transform.localScale, scale, Time.deltaTime * scaleDuration);
    }

    public void UpdateRectHeight(float height, float pivot) // TileFrame needs to update it's height
    {
        SetPivot(new Vector2(0.5f, pivot)); // Set the height according to 'isMainTileFrame' 

        if(updateCoroutine != null) StopCoroutine(updateCoroutine); //If the TileFrame's height is already changing, stop
        updateCoroutine = StartCoroutine(UpdateRectHeightCoroutine(height));//Start changing the TileFrame's height
    }

    private IEnumerator UpdateRectHeightCoroutine(float height)//TileFrame needs to update it's height
    {
        if(oppositeTileFrame.isUpdatingInfo) yield return new WaitUntil(() => !oppositeTileFrame.isUpdatingInfo);
        
        isUpdatingHeight = true;
        
        while (Mathf.Abs(height - rect.sizeDelta.y) > 1f)
        {
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, Mathf.Lerp(rect.sizeDelta.y, height, Time.deltaTime * scaleDuration));
            yield return new WaitForSeconds(0.001f);
        }

        isUpdatingHeight = false;
        yield return this;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseOver(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseOver(false);
    }
    
    private void OnMouseOver(bool isMouseOver)
    {
        isHovering = isMouseOver;
        if (canScale && !isMainTileFrame && !oppositeTileFrame.isUpdatingInfo && !isUpdatingInfo) 
        {
            oppositeTileFrame.UpdateRectHeight(isMouseOver ? closedHeight : openHeight, 1f);
            UpdateRectHeight(isMouseOver ? openHeight : closedHeight, 0f);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        hasClicked = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        hasClicked = false;
    }

    public void ShowInfoFrame(bool show)
    {
        if(isHovering && !oppositeTileFrame.isUpdatingInfo) StartCoroutine(ShowInfoFrameCoroutine(show));
    }

    private IEnumerator ShowInfoFrameCoroutine(bool show)
    {
        UpdateRectHeight(closedHeight,isMainTileFrame ? 0f : 1f);
        isUpdatingInfo = true; 

        yield return new WaitUntil(() => !isUpdatingHeight);

        //Set the frame's visibility
        InfoFrame.SetActive(show);
        MainFrame.SetActive(!show);
        
        yield return new WaitForSeconds(0.5f);

        isUpdatingInfo = false;
        UpdateRectHeight(openHeight, isMainTileFrame ? 0f : 1f);

        if (!isHovering && !isMainTileFrame)
        {
            isUpdatingInfo = true;

            yield return new WaitUntil(() => !isUpdatingHeight);
            
            UpdateRectHeight(closedHeight, 0f);
            oppositeTileFrame.UpdateRectHeight(openHeight, 1f);

            isUpdatingInfo = false;
        }
    }

    public void SetPivot(Vector2 pivot)
    {
        Vector2 size = rect.rect.size;
        Vector2 deltaPivot = rect.pivot - pivot;
        Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
        rect.pivot = pivot;
        rect.localPosition -= deltaPosition;
    }

}
