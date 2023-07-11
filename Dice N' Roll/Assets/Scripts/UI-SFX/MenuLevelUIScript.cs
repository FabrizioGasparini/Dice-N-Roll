using UnityEngine;
using UnityEngine.UI;

public class MenuLevelUIScript : MonoBehaviour
{
    [SerializeField] Image levelImage;
    [SerializeField] TMPro.TextMeshProUGUI levelName;

    public void SetLevelInfo(Sprite sprite, string text)
    {
        levelImage.sprite = sprite;
        levelName.text = text;
    }
}
