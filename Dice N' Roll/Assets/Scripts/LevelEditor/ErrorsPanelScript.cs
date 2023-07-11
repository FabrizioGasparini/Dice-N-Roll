using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ErrorsPanelScript : MonoBehaviour
{
    [SerializeField] private GameObject errorPrefab;
    public static UnityEvent<string> SendError = new UnityEvent<string>();

    private bool canPrintError;

    void Awake()
    {
        canPrintError = true;
        SendError.AddListener(CreateError);
    }
    
    private void CreateError(string errorText)
    {
        if(canPrintError)
        {
            var newError = Instantiate(errorPrefab, transform);
            newError.name = "Error";

            newError.SetActive(true);
            newError.GetComponent<TMPro.TextMeshProUGUI>().text = errorText;

            if(transform.childCount == 9) Destroy(transform.GetChild(0).gameObject);

            StartCoroutine(DebounceTime());
            StartCoroutine(DestroyError(newError.GetComponent<TMPro.TextMeshProUGUI>()));
        }
    }

    private IEnumerator DestroyError(TMPro.TextMeshProUGUI errorToDestroy)
    {
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < 255; i++)
        {
            if(errorToDestroy == null) break;
            errorToDestroy.color = new Color32(255, 0, 0, ((byte)(255 - i)));
            yield return new WaitForSeconds(0.0075f);
        }

        if(errorToDestroy != null) Destroy(errorToDestroy.gameObject);
    }

    private IEnumerator DebounceTime()
    {
        canPrintError = false;
        yield return new WaitForSeconds(0.5f);
        canPrintError = true;
    }
}
