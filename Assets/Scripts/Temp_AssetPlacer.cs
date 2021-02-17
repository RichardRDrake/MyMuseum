using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp_AssetPlacer : MonoBehaviour
{
    //creates a gridmanager object
    private GridManager grid;
    private GameObject painting1;
    // Start is called before the first frame update
    private void Awake()
    {
        grid = FindObjectOfType<GridManager>();
        painting1 = GameObject.Find("painting1");
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
        painting1.transform.position = finalPosition;
        Instantiate(painting1);
      

    }
}
