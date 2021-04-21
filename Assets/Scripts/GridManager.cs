using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
   //Lars' previous code, I'm keeping it here In case he wants it for portfolio stuff
    /*
    [SerializeField]

    //space between objects
    private float size = 1.0f;

    //public variables

    public float stateX;
    public float stateY;
    public float stateZ; 
  
    public Vector3 GetNearestPointOnGrid(Vector3 position)   //takes the original position and devides it by the size
    {
        position -= transform.position;
      
        //rounds te values to the nearest int
        int xCount = Mathf.RoundToInt(position.x / size);
        int yCount = Mathf.RoundToInt(position.y / size);
        int zCount = Mathf.RoundToInt(position.z / size);

        Vector3 result = new Vector3(
            (float)xCount * size,
            (float)yCount * size,
            (float)zCount * size);

        //re-adds the position
        result += transform.position;
        return result;
    }
    //draws yellow dots to better visualise the grid in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (float x = stateX; x < (stateX + 40); x += size)
        {
            for (float  z = stateZ; z < (stateZ + 40); z += size)
            {
                var point = GetNearestPointOnGrid(new Vector3(x, stateY + 0, z));
                Gizmos.DrawSphere(point, 0.1f);
            }
        }
    }
    */
   

   //Actual Grid manager stuff...

    //Grid Manager tracks all active grids as Asset placer was designed for single grids
    private List<PlacementGrid> GridList = new List<PlacementGrid>(); 

    public void RegisterNewGrid(PlacementGrid newGrid)
    {
        //New Grid has been created Register it here
        GridList.Add(newGrid);
    }

    public NearestPointResponse GetPointOnNearestGrid(Vector3 point)
    {
        float closestDistance = 1000;
        NearestPointResponse closestPos = null;

        foreach (PlacementGrid grid in GridList)
        {
            NearestPointResponse npr = grid.GetNearestPointOnGrid(point);
            if (npr != null)
            {
                if (npr.distance < closestDistance)
                {
                    closestDistance = npr.distance;
                    closestPos = npr;
                }
            }
            
        }

        return closestPos;
    }

    public int GetGridCount()
    {
        return GridList.Count;
    }

    public PlacementGrid GetGrid(int i)
    {
        return GridList[i];
    }

    public void ClearGrids()
    {
        foreach (PlacementGrid grid in GridList)
        {
            grid.ClearGrid();
        }
    }
} // TODO

//Assign active grid somewhere