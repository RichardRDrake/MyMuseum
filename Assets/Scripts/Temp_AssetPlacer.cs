using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp_AssetPlacer : MonoBehaviour
{
    //creates a gridmanager object
    private GridManager grid;
    // Start is called before the first frame update
    private void Awake()
    {
        
        grid = FindObjectOfType<GridManager>();
    }

    // Update is called once per frame
    private void Update() 
    {
        //place object at mouse position
     if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo))
            {
                PlaceCubeNear(hitInfo.point);
            }
        }
    }
    private void PlaceCubeNear(Vector3 clickPoint)
    {
        var finalPosition = grid.GetNearestPointOnGrid(clickPoint);
        GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = finalPosition;
    }
}
