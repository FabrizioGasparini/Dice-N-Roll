using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerScript : MonoBehaviour
{
    public void SetValue(string value)
    {
        GetComponentInChildren<TMPro.TextMeshProUGUI>().text = value;
    }
}
