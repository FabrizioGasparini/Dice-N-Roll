using UnityEngine;

public class FlagScript : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private float power;
    [SerializeField] private float frequency;
    [SerializeField] private float height;

    void Update()
    {
        transform.position = new Vector3(transform.position.x , height + Mathf.Sin(Time.time * frequency) * power, transform.position.z);
        transform.eulerAngles += new Vector3(0, .1f, 0);
    }
}
