using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileSettingsPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMPro.TextMeshProUGUI titleText;
    [SerializeField] private Slider gridRowsSlider;
    [SerializeField] private Slider gridColumnsSlider;
    
    private TMPro.TMP_Dropdown powerDropdown;

    [Header("SettingsPanels")]
    [SerializeField] private GameObject diceSettings;
    [SerializeField] private GameObject powerSettings;
    [SerializeField] private GameObject teleportSettings;
    [SerializeField] private GameObject gridSettings;

    private string currentSettings;

    private Grid grid;
    private EditorPlacementScript editor;

    void Awake()
    {
        SetVisible(false);

        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();

        editor = GameObject.FindGameObjectWithTag("Editor").GetComponent<EditorPlacementScript>();

        powerDropdown = powerSettings.GetComponentInChildren<TMPro.TMP_Dropdown>();


        foreach (var powerType in System.Enum.GetNames(typeof(PowerTypeTest)))
        {
            TMPro.TMP_Dropdown.OptionData newOption = new TMPro.TMP_Dropdown.OptionData();
            newOption.text = powerType;
            powerDropdown.options.Add(newOption);
        }

        gridRowsSlider.value = Resources.Load<LevelData>("Levels/CurrentEditorLevel").GridRows;
        gridColumnsSlider.value = Resources.Load<LevelData>("Levels/CurrentEditorLevel").GridColumns;
    }

    public void ShowSettings(string panelName)
    {
        titleText.text = panelName.ToUpper() + " SETTINGS";

        switch (panelName)
        {
            default:
                currentSettings = "";
                SetVisible(false);
                break;

            case "Dice":
                currentSettings = "Dice";
                EnableSettingPanel();
                SetVisible(true);
                break;            
            
            case "Power":
                currentSettings = "Power";
                EnableSettingPanel();
                SetVisible(true);
                break;
            
            case "Grid":
                currentSettings = "Grid";
                EnableSettingPanel();
                SetVisible(true);
                break;
        }
    }

    private void SetVisible(bool value)
    {
        transform.GetChild(0).gameObject.SetActive(value);
    }

    private void EnableSettingPanel()
    {
        diceSettings.SetActive(false);
        powerSettings.SetActive(false);
        gridSettings.SetActive(false);

        switch(currentSettings)
        {
            case "":
                break;

            case "Dice":
                diceSettings.SetActive(true);
                break;
            
            case "Power":
                powerSettings.SetActive(true);
                break;

            case "Teleport":
                teleportSettings.SetActive(true);
                break;
            
            case "Grid":
                gridSettings.SetActive(true);
                break;
        } 
    }

    public void UpdateSlider(Slider slider)
    {
        switch (currentSettings)
        {
            case "":
                break;

            case "Dice":
                grid.LevelData.DiceValue = (int)slider.value;
                break;

            case "Power":
                editor.SetPowerValue((int)slider.value);
                break;

            case "Teleport":
                break;

            case "Grid":
                if(slider == gridRowsSlider)
                {
                    Debug.Log(grid.GetLastTileRow());
                    if(grid.GetLastTileRow() < grid.LevelData.GridRows - 1  || slider.value > grid.LevelData.GridRows) grid.SetRows((int)slider.value);
                    else
                    {
                        slider.value = grid.LevelData.GridRows;
                        ErrorsPanelScript.SendError.Invoke("You need to remove <color=white>border tiles<color=red> to decrease grid size");
                    } 
                }
                else if(slider == gridColumnsSlider)
                {
                    Debug.Log(grid.GetLastTileColumn());
                    if(grid.GetLastTileColumn() < grid.LevelData.GridColumns - 1 || slider.value > grid.LevelData.GridColumns) grid.SetColumns((int)slider.value);
                    else slider.value = grid.LevelData.GridColumns;
                }

                break;
        }

        slider.transform.parent.Find("SliderValue").GetComponent<TMPro.TextMeshProUGUI>().text = slider.value.ToString();
    }

    public void UpdatePowerType(TMPro.TMP_Dropdown dropdown)
    {
        PowerTypeTest powerType = (PowerTypeTest)System.Enum.Parse(typeof(PowerTypeTest), System.Enum.GetNames(typeof(PowerTypeTest))[dropdown.value]);

        switch(powerType)
        {
            case PowerTypeTest.Add:
                powerSettings.transform.Find("PowerValue").gameObject.SetActive(true);
                break;

            case PowerTypeTest.Remove:
                powerSettings.transform.Find("PowerValue").gameObject.SetActive(true);
                break;

            case PowerTypeTest.Double:
                powerSettings.transform.Find("PowerValue").gameObject.SetActive(false);
                break;

            case PowerTypeTest.Split:
                powerSettings.transform.Find("PowerValue").gameObject.SetActive(false);
                break;
        }

        editor.SetPowerType(powerType);
    }
}
