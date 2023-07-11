using System;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmActionUI : MonoBehaviour
{
    public static ConfirmActionUI Instance { get; private set; }
    private Transform panel;
    private Transform imagePanel;

    private TMPro.TextMeshProUGUI panelText;
    private TMPro.TextMeshProUGUI imagePanelText;

    private Button negativeBtn;
    private Button positiveBtn;
    private Button imgNegativeBtn;
    private Button imgPositiveBtn;

    void Awake()
    {
        Instance = this;

        panel = transform.GetChild(0);
        panel.gameObject.SetActive(false);
        
        panelText = panel.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
        negativeBtn = panel.GetChild(1).GetComponent<Button>();
        positiveBtn = panel.GetChild(2).GetComponent<Button>();
        
        
        imagePanel = transform.GetChild(1);
        imagePanel.gameObject.SetActive(false);

        imagePanelText = imagePanel.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
        imgNegativeBtn = imagePanel.GetChild(1).GetComponent<Button>();
        imgPositiveBtn = imagePanel.GetChild(2).GetComponent<Button>();
    }

    public void ConfirmAction(string text, string negativeText, string positiveText, Action negativeAction, Action positiveAction, bool transparentBackground = false, Texture2D image = null)
    {
        panel.gameObject.SetActive(true);

        panelText.text = text;
        imagePanelText.text = text;

        negativeBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = negativeText;
        positiveBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = positiveText;

        negativeBtn.onClick.AddListener(() => {
            Hide();
            negativeAction();
        });

        positiveBtn.onClick.AddListener(() =>
        {
            Hide();
            positiveAction();
        });


        if(transparentBackground)
        {
            panel.GetComponent<Image>().color = Color.clear;
            imagePanel.GetComponent<Image>().color = Color.clear;
        }  
        else 
        {
            panel.GetComponent<Image>().color = new Color32(56, 56, 56, 150);
            imagePanel.GetComponent<Image>().color = new Color32(56, 56, 56, 150);
        }

        if(image != null)
        {
            imagePanel.gameObject.SetActive(true);
            panel.gameObject.SetActive(false);

            imagePanel.GetChild(3).GetComponentInChildren<RawImage>().texture = image;

            imgNegativeBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = negativeText;
            imgPositiveBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = positiveText;

            imgNegativeBtn.onClick.AddListener(() => {
                Hide();
                negativeAction();
            });

            imgPositiveBtn.onClick.AddListener(() =>
            {
                Hide();
                positiveAction();
            });
        } 
        else
        {
            imagePanel.gameObject.SetActive(false);
            panel.gameObject.SetActive(true);
        } 
    }

    private void Hide()
    {
        panel.gameObject.SetActive(false);
        imagePanel.gameObject.SetActive(false);

        negativeBtn.onClick.RemoveAllListeners();
        positiveBtn.onClick.RemoveAllListeners();

        imgNegativeBtn.onClick.RemoveAllListeners();
        imgPositiveBtn.onClick.RemoveAllListeners();
    }
}
