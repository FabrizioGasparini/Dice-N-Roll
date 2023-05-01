using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public int speed;
    private float minFOV = 20;
    private float maxFOV = 50;

    private Grid grid;
    private COOPGrid coopGrid;


    private Vector3 maxPoint;
    private Vector3 minPoint;

    private Vector3 camStartingPos;
    private Vector2 mouseStartingPos;
    private Vector2 mouseCurrentPos;

    private int gridRows;
    private int gridColumns;


    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube((minPoint + maxPoint) / 2, maxPoint - minPoint);
    }

    void Start()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");

        grid = GameObject.FindObjectOfType<Grid>();
        if(grid == null) coopGrid = GameObject.FindObjectOfType<COOPGrid>();

        ResetCamera();
    }

    void Update()
    {   
        maxPoint = new Vector3(minPoint.x + gridRows, camStartingPos.y, minPoint.z + gridColumns);
        
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

                transform.position = new Vector3(transform.position.x + xMovement / speed * gridColumns, transform.position.y, transform.position.z - zMovement / speed * gridColumns);

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
        if(grid)
        {
            gridRows = grid.LevelData.GridRows; 
            gridColumns = grid.LevelData.GridColumns;
        }
        else
        {
            gridRows = coopGrid.LevelData.GridRows;
            gridColumns = coopGrid.LevelData.GridColumns;
        }

       
        float midRow = (gridRows - 1) / 2 + 0; //grid.startPosition.x;
        float midColumns = (gridColumns - 1) / 2 + 0; //grid.startPosition.z;

        camStartingPos = new Vector3(midRow, (gridRows + gridColumns) - 2, midColumns);
        transform.position = camStartingPos;

        minPoint = new Vector3(0 /*grid.startPosition.x*/, camStartingPos.y, 0 /*grid.startPosition.z*/);
        maxPoint = new Vector3((gridRows - 1) + 0 /*grid.startPosition.x*/, camStartingPos.y, (gridColumns - 1) + 0/*grid.startPosition.z*/);

        Camera.main.fieldOfView = maxFOV;

    }
}