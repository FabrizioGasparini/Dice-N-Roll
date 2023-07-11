using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [HideInInspector] public  TabGroup tabGroup;
    [HideInInspector] public Image background;
    

    private void Start()
    {
        tabGroup = GetComponentInParent<TabGroup>();
        background = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }
}
