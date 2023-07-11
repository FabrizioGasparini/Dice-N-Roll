using UnityEngine;
using UnityEngine.UI;

public class TileSelectorPanel : MonoBehaviour
{
    [SerializeField] private GameObject tileButtonTemplate;
    [SerializeField] private TileSettingsPanel tileSettingsPanel;

    private Transform content;
    private string[] tilesTypes = System.Enum.GetNames(typeof(TileType));

    private EditorPlacementScript editor;

    void Awake()
    {
        OpenPanel(false);

        editor = GameObject.FindGameObjectWithTag("Editor").GetComponent<EditorPlacementScript>();

        content = GetComponent<ScrollRect>().content.transform;

        var gridButton = CreateButton("Grid");
        gridButton.onClick.AddListener(() => {
            if (!editor.firstTeleportPlaced) tileSettingsPanel.ShowSettings(gridButton.name);
        });

        for(int i = 0; i < tilesTypes.Length; i++)
        {
            if(tilesTypes[i] == "GhostBlock") return;
            
            var newButton = CreateButton(tilesTypes[i]);
            
            newButton.onClick.AddListener(() => {
                editor.SelectTileType((TileType)System.Enum.Parse(typeof(TileType), newButton.name));
                if (!editor.firstTeleportPlaced) tileSettingsPanel.ShowSettings(newButton.name);
            });
        }
    }

    private Button CreateButton(string name)
    {
        var newButton = Instantiate(tileButtonTemplate, content);
        newButton.name = name;
        newButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = name;

        return newButton.GetComponent<Button>();
    }

    public void OpenPanel(bool value)
    {
        transform.GetChild(0).gameObject.SetActive(value);
        transform.GetChild(1).gameObject.SetActive(!value);
    }
}
