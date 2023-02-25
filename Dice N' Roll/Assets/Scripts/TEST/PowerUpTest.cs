using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PowerTypeTest
{
    Add, 
    Remove,
    Double,
    Split
}

public class PowerUpTest : MonoBehaviour
{
    
    [Header("Values")]
    private float x;
    private float y;
    [SerializeField] private float power;
    [SerializeField] private float frequency;
    [SerializeField] private float height;
    public int value;
    public PowerTypeTest powerType;
    
    [Header("Editor")]
    private GameObject powerUpEditorPrefab;
    private GameObject powerUpEditor;
    [HideInInspector] public bool _editorOpen;
    [SerializeField] private bool _inEditor;

    private GameObject pickupLabel;
    private Transform dots;
    private GameObject grid;

    void Update(){
        /*if(!grid.GetComponent<Grod>().inMenu){
            pickupLabel.transform.parent.gameObject.transform.eulerAngles += new Vector3(0,0,.1f); 
        } else {
            pickupLabel.transform.parent.gameObject.transform.eulerAngles += new Vector3(0,.1f,0);
        }

        transform.position = new Vector3(y , height + Mathf.Sin(Time.time * frequency) * power, x);
        transform.eulerAngles += new Vector3(0,.1f,0);

        if (_inEditor && powerUpEditor)
        {
            powerUpEditor.transform.eulerAngles -= new Vector3(0,.1f,0);
            powerUpEditor.transform.position = new Vector3(powerUpEditor.transform.position.x, transform.position. y - (height + Mathf.Sin(Time.time * frequency) * power) + height * 2, powerUpEditor.transform.position.z);
        }*/
        ChangeNumber();
    }

    void Start()
    {
        x = transform.position.z;
        y = transform.position.x;
        pickupLabel = transform.Find("Canvas").transform.Find("PickupValue").gameObject;
        dots = transform.Find("DotsCanvas");
        grid = transform.parent.parent.parent.Find("Grid").Find("TilePrefab").gameObject;
        
        if(_inEditor){
            powerUpEditorPrefab = transform.parent.parent.parent.Find("UI").Find("Editors").Find("PowerUpEditor").gameObject;
        }
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

    /// EDITOR \\\

    void OnMouseDown()
    {
        if(!_editorOpen){
            _editorOpen = true;
            GameObject editor = Instantiate(powerUpEditorPrefab, new Vector3(0 + y, 1.25f, 0 + x), Quaternion.Euler(-70,90,0), transform);
            editor.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            editor.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Slider>().value = value;
            editor.SetActive(true);
            powerUpEditor = editor;
        }
    }
}
