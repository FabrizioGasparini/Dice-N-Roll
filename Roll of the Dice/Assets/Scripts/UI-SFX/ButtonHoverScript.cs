using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ButtonHoverScript :  MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 startScale = new Vector3(0.9f, 0.9f, 1);
    private Vector3 endScale = new Vector3(1.1f, 1.1f, 1);
    private float speed = 5;
    private bool mouse_over;
 
    void Start()
    {
        transform.localScale = startScale;
    }

    void Update()
    {
        if(mouse_over){
            transform.localScale = Vector3.Lerp(transform.localScale, endScale, Time.deltaTime * speed);
        } else {
            transform.localScale = Vector3.Lerp(transform.localScale, startScale, Time.deltaTime * speed);
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        mouse_over = true;
    }

    
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        mouse_over = false;
    }

    public void ResetButton(){
        transform.localScale = startScale;
        mouse_over = false;
    }
}
