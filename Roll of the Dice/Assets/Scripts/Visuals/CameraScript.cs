using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Range(10, 200)] private float speed = 25;
    private float minFOV = 20;
    private float maxFOV = 50;

    private Grid grid;

    private Vector3 maxPoint;
    private Vector3 minPoint;

    private Vector3 camStartingPos;
    private Vector2 mouseStartingPos;
    private Vector2 mouseCurrentPos;


    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube((minPoint + maxPoint) / 2, maxPoint - minPoint);
    }

    void Start()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");

        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();

        ResetCamera();
    }

    void Update()
    {    
        maxPoint = new Vector3(minPoint.x + grid.LevelData.GridRows, camStartingPos.y, minPoint.z + grid.LevelData.GridColumns);
        
        if(Input.mouseScrollDelta.y != 0) Camera.main.fieldOfView -= Input.mouseScrollDelta.y;

        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, minFOV, maxFOV);

        if(Input.GetMouseButtonDown(2))
        {
            mouseStartingPos = Input.mousePosition;
        }

        if(Input.GetMouseButton(2))
        {
            if(Input.mousePosition.x != mouseStartingPos.x || Input.mousePosition.y != mouseStartingPos.y)
            {
                mouseCurrentPos = Input.mousePosition;
                float xMovement = mouseCurrentPos.y - mouseStartingPos.y;
                float zMovement = mouseCurrentPos.x - mouseStartingPos.x;

                transform.position = new Vector3(transform.position.x + xMovement / (201 - speed), transform.position.y, transform.position.z - zMovement / (201 - speed));

                mouseStartingPos = mouseCurrentPos;
            }
        }

        float x = Mathf.Clamp(transform.position.x, minPoint.x, maxPoint.x);
        float y = Mathf.Clamp(transform.position.y, minPoint.y, maxPoint.y);
        float z = Mathf.Clamp(transform.position.z, minPoint.z, maxPoint.z);

        transform.position = new Vector3(x, y, z);
    }

    public void ResetCamera()
    {
        float midRow = (grid.LevelData.GridRows - 1) / 2 + grid.startPosition.x;
        float midColumns = (grid.LevelData.GridColumns - 1) / 2 + grid.startPosition.z;

        camStartingPos = new Vector3(midRow, (grid.LevelData.GridRows + grid.LevelData.GridColumns) - 2, midColumns);
        transform.position = camStartingPos;

        minPoint = new Vector3(grid.startPosition.x, camStartingPos.y, grid.startPosition.z);
        maxPoint = new Vector3((grid.LevelData.GridRows - 1) + grid.startPosition.x, camStartingPos.y, (grid.LevelData.GridColumns - 1) + grid.startPosition.z);

        Camera.main.fieldOfView = maxFOV;
    }
}