using UnityEngine;
using TMPro;

public class SettingsPanelUI : MonoBehaviour
{
    private Grid grid;

    void Start()
    {
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();

        Close();
    }

    public void Open()
    {
        gameObject.SetActive(true); 
    }

    public void Close()
    {
        gameObject.SetActive(false); 
    }

    public void SetGridSize(TMP_InputField input)
    {
        if(input.gameObject.name == "XSize")
        {
            int value = int.Parse(input.text);

            if(value < 3) value = 3; 
            if(value > 15) value = 15;

            input.text = value.ToString();

            grid.SetRows(value);
        }
        else
        {
            int value = int.Parse(input.text);

            if(value < 3) value = 3;
            if(value > 15) value = 15; 

            input.text = value.ToString();
            
            grid.SetColumns(value);
        }

        Camera.main.GetComponent<CameraScript>().ResetCamera();
    }
}
