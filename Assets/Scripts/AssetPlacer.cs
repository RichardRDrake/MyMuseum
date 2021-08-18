using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.AddressableAssets;

public class AssetPlacer : MonoBehaviour //At this point its more accurate to call it a manipulator
{
    //Component that recives objects selected in the Artefact Browser and allow the user to place them on a grid
    //Parts of this will be subject to change, denoted with [PH]

   // [SerializeField] GridManager gridManager = null; //Assigned in Inspector
        
        //Active grid, since this relies on Artefact browser, we will temporary use 1-3 to select objects
    [SerializeField] PlacementGrid activeGrid = null; //[PH] Assigned during run-time

    //[SerializeField] GameObject cameraObj = null; //[PH] Assigned during run-time
    //private Camera camera;

    private GameObject objectToBePlaced = null;
    private Asset asset = null;
    //private bool validPlacement = false;

    //[SerializeField] private AudioManager audioManager;
   

    // Start is called before the first frame update
    void Start()
    {
        //audioManager = FindObjectOfType<AudioManager>();
        //gridManager = FindObjectOfType<GridManager>();
        //if (gridManager == null)
        //{
        //    Debug.LogError("Asset Placer does not have a Grid Manager assigned.");
        //}
        //if (cameraObj)
        //{
        //    camera = cameraObj.GetComponent<Camera>();
        //}
        //else
        //{
        //    Debug.Log("Please assign camera Object");
        //}
    }

    // Update is called once per frame
    /*void Update()
    {
        Vector3 validPosition = RayToGrid();

        //Look to see if the user is giving any inputs
        ProcessInput();
        
        //If we're in the process of placing an object, show where the object will go
        //Debug.Log(objectToBePlaced + " and " + activeGrid);

        if (objectToBePlaced != null && activeGrid != null)
        {
            if (Input.GetKeyDown(KeyCode.A) ||  Input.GetKeyDown(KeyCode.LeftArrow))
            {
                objectToBePlaced.transform.Rotate(0.0f, 90.0f, 0.0f, Space.Self);
            }

            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                objectToBePlaced.transform.Rotate(0.0f, -90.0f, 0.0f, Space.Self);
            }

            //Debug.Log("TEST");
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
                validPlacement = false;
            }
            
        }
    }*/

   

    /*private void ProcessInput()
    {
        if (Input.GetMouseButton(0) && objectToBePlaced != null && validPlacement == true)
        {
            Release selected object
            ChangeColour(objectToBePlaced, Color.white);

            Let the grid know an object has been placed on it
            activeGrid.OnObjectPlaced(objectToBePlaced.transform.position, asset);
            audioManager.Play("Place_SFX");
            if there are grids on the object, they must be (re)built here
            PlacementGrid[] childrenGrids;

            

            childrenGrids = objectToBePlaced.GetComponentsInChildren<PlacementGrid>();
            foreach (PlacementGrid grid in childrenGrids)
            {
                grid.BuildGrid();
            }

            objectToBePlaced = null;
            asset = null;
            validPlacement = false;
        }
        just so I can play the other sounds effect
        if (Input.GetMouseButton(0) && objectToBePlaced != null && validPlacement == false)
        {
            audioManager.Play("Can_Not_Place_Here_SFX");
        }
            /*
            This whole section will be [PH]
            if (Input.GetKeyDown(KeyCode.Alpha1) && objectToBePlaced == null)
            {
                Queue ex_obj_1 
                objectToBePlaced = Instantiate(exampleObject_1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && objectToBePlaced == null)
            {
                Queue ex_obj_2 
                objectToBePlaced = Instantiate(exampleObject_2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && objectToBePlaced == null)
            {
                Queue ex_obj_3 
                objectToBePlaced = Instantiate(exampleObject_3);
            }
            
        }*/

   /* private Vector3 RayToGrid()
    {
        Turns a raycast into the closest point on the active grid
        RaycastHit hitInfo;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitInfo))
        {
            NearestPointResponse nearestPoint = gridManager.GetPointOnNearestGrid(hitInfo.point);
            if (nearestPoint == null)
            {
                return Vector3.zero;
            }

            activeGrid = nearestPoint.grid;
            Debug.Log(activeGrid);
            Check to see is position is valid (not occupied)
            if (nearestPoint != null && nearestPoint.gridPosition.occupied == null)
            {
                return nearestPoint.gridPosition.position;
            }
            else return Vector3.zero;
        }

        return Vector3.zero;
    }*/

   /* public NearestPointResponse PointToGrid(Vector3 point, GameObject artefact) //Place object on Gridpoint at saved position (used by loading utility)
    {
        NearestPointResponse nearestPoint = gridManager.GetPointOnNearestGrid(point);
        if (nearestPoint == null)
        {
            return null;
        }

        return nearestPoint;
    }
   */
    /*private void ChangeColour(GameObject target, Color newColor) //At this stage this is simple enough, but I anticipate it will get more compicated as more things are completed
    {
        MeshRenderer mr = target.GetComponent<MeshRenderer>();

        if (mr)
        {
            mr.material.color = newColor;
        }       
    }
    */
}
