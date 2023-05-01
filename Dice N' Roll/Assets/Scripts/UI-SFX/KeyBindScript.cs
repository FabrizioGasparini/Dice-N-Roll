using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyBindScript : MonoBehaviour
{
    private Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();

    [SerializeField] private TMPro.TextMeshProUGUI up, down, left, right, pause, label;

    private GameObject selectedKey = null;

    void Awake()
    {
        keys.Add("Up", KeyCode.W);
        keys.Add("Down", KeyCode.S);
        keys.Add("Left", KeyCode.A);
        keys.Add("Right", KeyCode.D);
        keys.Add("Pause", KeyCode.Escape);
        
        LoadPlayerKeys();
    }


    void Update()
    {
        up.text = "up [" + keys["Up"].ToString() + "]";
        down.text = "down [" + keys["Down"].ToString() + "]";
        left.text = "left [" + keys["Left"].ToString() + "]";
        right.text = "right [" + keys["Right"].ToString() + "]";
        pause.text = "pause [" + keys["Pause"].ToString() + "]";

        label.text = "- MOVE [" + keys["Up"].ToString() + keys["Left"].ToString() + keys["Down"].ToString() + keys["Right"].ToString() + " / ARROWS]                   - PAUSE [" + keys["Pause"].ToString() + "]";

    }

    public void SetSelectedKey(GameObject pressedKey)
    {
        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.PlayOneShot((AudioClip)Resources.Load("Sounds/ButtonSound"));
        selectedKey = pressedKey;
    }

    void OnGUI()
    {
        if (selectedKey != null){
            Event e = Event.current;
            if (e.isKey){
                keys[selectedKey.name] = e.keyCode;
                selectedKey = null;
                ChangeColors(null);
            }
        }
    }

    public void LoadPlayerKeys()
    {
        if (PlayerPrefs.HasKey("Up"))
        {
            if(PlayerPrefs.GetString("Up") == "") return;
            KeyCode up = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up"));
            keys["Up"] = up;
        }
        if (PlayerPrefs.HasKey("Down"))
        {
            if(PlayerPrefs.GetString("Down") == "") return;
            KeyCode down = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Down"));
            keys["Down"] = down;
        }
        if (PlayerPrefs.HasKey("Left"))
        {
            if(PlayerPrefs.GetString("Left") == "") return;
            KeyCode left = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left"));
            keys["Left"] = left;
        }
        if (PlayerPrefs.HasKey("Right"))
        {
            if(PlayerPrefs.GetString("Right") == "") return;
            KeyCode right = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right"));
            keys["Right"] = right;
        }
        if (PlayerPrefs.HasKey("Pause"))
        {
            if(PlayerPrefs.GetString("Pause") == "") return;
            KeyCode pause = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Pause"));
            keys["Pause"] = pause;
        }
    }

    public void SavePlayerKeys()
    {
        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.PlayOneShot((AudioClip)Resources.Load("Sounds/ButtonSound"));

        PlayerPrefs.SetString("Up", keys["Up"].ToString());
        PlayerPrefs.SetString("Down", keys["Down"].ToString());
        PlayerPrefs.SetString("Left", keys["Left"].ToString());
        PlayerPrefs.SetString("Right", keys["Right"].ToString());
        PlayerPrefs.SetString("Pause", keys["Pause"].ToString());
    }

    public void ChangeColors(GameObject button)
    {
        up.gameObject.transform.parent.Find("Button").gameObject.GetComponent<Image>().color = new Color32(255,178,69,255);
        down.gameObject.transform.parent.Find("Button").gameObject.GetComponent<Image>().color = new Color32(255,178,69,255);
        left.gameObject.transform.parent.Find("Button").gameObject.GetComponent<Image>().color = new Color32(255,178,69,255);
        right.gameObject.transform.parent.Find("Button").gameObject.GetComponent<Image>().color = new Color32(255,178,69,255);
        pause.gameObject.transform.parent.Find("Button").gameObject.GetComponent<Image>().color = new Color32(255,178,69,255);

        if (button != null)
        {
            button.GetComponent<Image>().color = new Color32(185,131,54,255);
        }
    }

    public void Back()
    {
        selectedKey = null;
        ChangeColors(null);
    }
}
