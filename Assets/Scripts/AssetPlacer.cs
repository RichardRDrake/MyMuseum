using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetPlacer : MonoBehaviour
{
    //Component that recives objects selected in the Artefact Browser and allow the user to place them on a grid
    //Parts of this will be subject to change, denoted with [PH]

    //Active grid, since this relies on Artefact browser, we will temporary use 1-3 to select objects
    [SerializeField] PlacementGrid activeGrid = null; //[PH] Assigned during run-time

    [SerializeField] GameObject cameraObj = null; //[PH] Assigned during run-time
    private Camera camera;

    private GameObject objectToBePlaced = null;
    private bool validPlacement = false;

    //Example objects [PH]
    public GameObject exampleObject_1 = null; //[PH] Assigned in inspector
    public GameObject exampleObject_2 = null; //[PH] Assigned in inspector
    public GameObject exampleObject_3 = null; //[PH] Assigned in inspector

    // Start is called before the first frame update
    void Start()
    {
        activeGrid = FindObjectOfType<PlacementGrid>(); //[PH]
        if (exampleObject_1 == null)
        {
            Debug.Log("example placement objects not assigned!");
        }

        if (cameraObj)
        {
            camera = cameraObj.GetComponent<Camera>();
        }
        else
        {
            Debug.Log("Please assign camera Object");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Look to see if the user is giving any inputs
        ProcessInput();

        //If we're in the process of placing an object, show where the object will go
        if (objectToBePlaced != null)
        {
            Vector3 validPosition = RayToGrid();
            if (validPosition != Vector3.zero)
            {
                objectToBePlaced.transform.position = validPosition;
                validPosition = Vector3.zero;
                ChangeColour(objectToBePlaced, Color.green);
                validPlacement = true;
            }
            else
            {
                ChangeColour(objectToBePlaced, Color.red);
                validPlacement = true;
            }
            
        }
    }

    private void ProcessInput()
    {
        if (Input.GetMouseButtonDown(0) && objectToBePlaced != null && validPlacement == true)
        {
            //Release selected object
            ChangeColour(objectToBePlaced, Color.white);

            //Let the grid know an object has been placed on it
            activeGrid.OnObjectPlaced(objectToBePlaced.transform.position);

            objectToBePlaced = null;
            validPlacement = false;
        }

        //This whole section will be [PH]
        if (Input.GetKeyDown(KeyCode.Alpha1) && objectToBePlaced == null)
        {
            //Queue ex_obj_1 
            objectToBePlaced = Instantiate(exampleObject_1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && objectToBePlaced == null)
        {
            //Queue ex_obj_2 
            objectToBePlaced = Instantiate(exampleObject_2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && objectToBePlaced == null)
        {
            //Queue ex_obj_3 
            objectToBePlaced = Instantiate(exampleObject_3);
        }
    }

    private Vector3 RayToGrid()
    {
        //Turns a raycast into the closest point on the active grid
        RaycastHit hitInfo;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitInfo))
        {
            GridPosition gridPosition = activeGrid.GetNearestPointOnGrid(hitInfo.point);
            //Check to see is position is valid (not occupied)
            if (gridPosition != null && gridPosition.occupied == false)
            {
                return gridPosition.position;
            }
            else return Vector3.zero;
        }

        return Vector3.zero;
    }

    private void ChangeColour(GameObject target, Color newColor) //At this stage this is simple enough, but I anticipate it will get more compicated as more things are completed
    {
        target.GetComponent<MeshRenderer>().material.color = newColor;
    }
}
