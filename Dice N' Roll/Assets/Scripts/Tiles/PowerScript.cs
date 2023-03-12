using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PowerType
{
    Add, 
    Remove,
    Double,
    Split
}

public class PowerScript : MonoBehaviour
{
    private float x;
    private float y;
    [HideInInspector] public int value;

    private GameObject pickupLabel;
    private Transform dots;

    void Update()
    {
        ChangeNumber();
    }

    void Start()
    {
        x = transform.position.z;
        y = transform.position.x;
        pickupLabel = transform.Find("Canvas").transform.Find("PickupValue").gameObject;
        dots = transform.Find("DotsCanvas");
    }
    
    public void ChangeNumber(){
        List<GameObject> dots00 = new List<GameObject>();
        List<GameObject> dots10 = new List<GameObject>();
        List<GameObject> dots15 = new List<GameObject>();
        List<GameObject> dots20 = new List<GameObject>();
        List<GameObject> dots01 = new List<GameObject>();
        List<GameObject> dots11 = new List<GameObject>();
        List<GameObject> dots21 = new List<GameObject>();
        List<GameObject> dotsError = new List<GameObject>();


        foreach (Transform dot in dots)
        {
            if(dot.name == "Dot00"){
                dots00.Add(dot.gameObject);
            } else if(dot.name == "Dot10"){
                dots10.Add(dot.gameObject);
            } else if(dot.name == "Dot15"){
                dots15.Add(dot.gameObject);
            } else if(dot.name == "Dot20"){
                dots20.Add(dot.gameObject);
            } else if(dot.name == "Dot01"){
                dots01.Add(dot.gameObject);
            } else if(dot.name == "Dot11"){
                dots11.Add(dot.gameObject);
            } else if(dot.name == "Dot21"){
                dots21.Add(dot.gameObject);
            } else {
                dotsError.Add(dot.gameObject);
            }
        }

        SetActiveDots(dots00, false);
        SetActiveDots(dots10, false);
        SetActiveDots(dots15, false);
        SetActiveDots(dots20, false);
        SetActiveDots(dots01, false);
        SetActiveDots(dots11, false);
        SetActiveDots(dots21, false);
        SetActiveDots(dotsError, false);

        switch(value){
            case 0:
                SetActiveDots(dotsError, true);
                break;
            case 1:
                SetActiveDots(dots15, true);
                break;
            case 2:
                SetActiveDots(dots00, true);
                SetActiveDots(dots21, true);
                break;
            case 3:
                SetActiveDots(dots00, true);
                SetActiveDots(dots15, true);
                SetActiveDots(dots21, true);
                break;
            case 4:
                SetActiveDots(dots00, true);
                SetActiveDots(dots01, true);
                SetActiveDots(dots20, true);
                SetActiveDots(dots21, true);
                break;
            case 5:
                SetActiveDots(dots00, true);
                SetActiveDots(dots01, true);
                SetActiveDots(dots15, true);
                SetActiveDots(dots20, true);
                SetActiveDots(dots21, true);
                break;
            case 6:
                SetActiveDots(dots00, true);
                SetActiveDots(dots01, true);
                SetActiveDots(dots10, true);
                SetActiveDots(dots11, true);
                SetActiveDots(dots20, true);
                SetActiveDots(dots21, true);
                break;
            default:
                break;
        }
    }

    private void SetActiveDots(List<GameObject> dots, bool value){
        foreach (GameObject dot in dots)
        {
            dot.SetActive(value);
        }
    }

    public void SetValue(string value)
    {
        GetComponentInChildren<TMPro.TextMeshProUGUI>().text = value;
    }
}
