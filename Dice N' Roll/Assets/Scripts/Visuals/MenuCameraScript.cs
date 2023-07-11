using System.Collections;
using UnityEngine;

public class MenuCameraScript : MonoBehaviour
{
    private float speed = 25;
    private float minFOV = 20;
    private float maxFOV = 50;

    private Grid grid;

    private Vector3 maxPoint;
    private Vector3 minPoint;

    private Vector3 camStartingPos;
    private Vector2 mouseStartingPos;
    private Vector2 mouseCurrentPos;

    private Vector3 menuPosition;
    private Vector3 levelPosition;

    private bool inMenu = true;
    [HideInInspector] public bool canMove = false;

    void Awake()
    {
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();

        ResetCamera();

        transform.position = menuPosition;
    }

    void Update()
    {
        if(canMove)
        {
            maxPoint = new Vector3(minPoint.x + grid.LevelData.GridRows, camStartingPos.y, minPoint.z + grid.LevelData.GridColumns);

            if (Input.mouseScrollDelta.y != 0) Camera.main.fieldOfView -= Input.mouseScrollDelta.y;

            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, minFOV, maxFOV);

            if (Input.GetMouseButtonDown(2))
            {
                mouseStartingPos = Input.mousePosition;
            }

            if (Input.GetMouseButton(2))
            {
                if (Input.mousePosition.x != mouseStartingPos.x || Input.mousePosition.y != mouseStartingPos.y)
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
            levelPosition = transform.position;
        }

        MoveCamera();
    }

    public void ResetCamera()
    {
        float midRow = (grid.LevelData.GridRows - 1) / 2 + grid.startPosition.x;
        float midColumns = (grid.LevelData.GridColumns - 1) / 2 + grid.startPosition.z;

        camStartingPos = new Vector3(midRow, (grid.LevelData.GridRows + grid.LevelData.GridColumns) - 2, midColumns);
        levelPosition = camStartingPos;
        menuPosition = new Vector3(-7, 0, 0) + camStartingPos;

        minPoint = new Vector3(grid.startPosition.x, camStartingPos.y, grid.startPosition.z);
        maxPoint = new Vector3((grid.LevelData.GridRows - 1) + grid.startPosition.x, camStartingPos.y, (grid.LevelData.GridColumns - 1) + grid.startPosition.z);

        Camera.main.fieldOfView = maxFOV;
    }

    private void MoveCamera()
    {
        if(!inMenu) 
        {
            transform.position = Vector3.Lerp(transform.position, levelPosition, 2 * Time.deltaTime);
            if(!canMove) StartCoroutine(SetCanMove());
        }
        else
        {
            canMove = false;
            transform.position = Vector3.Lerp(transform.position, menuPosition, 2 * Time.deltaTime);
            ResetCamera();
        } 
        
    }

    public void SetMenuValue(bool value)
    {
        if(!inMenu && !canMove) return;
        inMenu = value;
    }

    IEnumerator SetCanMove()
    {
        yield return new WaitForSeconds(1.5f);
        canMove = true;
    }
}
