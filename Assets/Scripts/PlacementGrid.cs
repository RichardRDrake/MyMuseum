using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementGrid : MonoBehaviour
{
    //space between objects
    [SerializeField] private float size = 1.0f; //Probably a poor name choice. This represents the density of points on the grid

    //public variables    

    // Grid Bounds
    public float boundsX;
    public float boundsY;
    public float boundsZ;

    // Offset from Object position
    public float offsetX;
    public float offsetY;
    public float offsetZ;

    // The Grid
    private GridPosition[,,] gridPositions;

    // Start is called before the first frame update
    void Start()
    {
        BuildGrid();
    }

    // Update is called once per frame
    void Update()
    {
        //BuildGrid();
    }

    private void BuildGrid()
    {
        //Build the grid

        // Number of points on a grid axis = (bounds/size rounded down)
        int xCount = Mathf.FloorToInt(boundsX / size);
        int yCount = Mathf.FloorToInt(boundsY / size);
        int zCount = Mathf.FloorToInt(boundsZ / size);

        //Now we know how many points are in each axis, we can build a 3D array to contain them
        gridPositions = new GridPosition[xCount, yCount, zCount];

        Vector3 GridObjectPosition = transform.position;

        //For each position on the grid, assign a gridPosition for it [WARNING - THIS IS A DECEPTIVELY MASSIVE FOR-LOOP]
        for (int x = 0; x < xCount; x++)
        {
            for (int y = 0; y < yCount; y++)
            {
                for (int z = 0; z < zCount; z++)
                {
                    //Work out the position of this point
                    // Position per axis = (Count * Size + offset + object position) - (bounds/2 - 0.5)
                    float _gridPosX = ((x * size) + offsetX + GridObjectPosition.x) - ((boundsX / 2) - 0.5f);
                    float _gridPosY = ((y * size) + offsetY + GridObjectPosition.y) - ((boundsY / 2) - 0.5f);
                    float _gridPosZ = ((z * size) + offsetZ + GridObjectPosition.z) - ((boundsZ / 2) - 0.5f);

                    //Add to array
                    Vector3 _gridPos = new Vector3(_gridPosX, _gridPosY, _gridPosZ);
                    gridPositions[x, y, z] = new GridPosition(_gridPos);

                    //Debug.Log("Generated Grid Point " + gridPositions[x, y, z] + " at " + _gridPos);
                }
            }
        }
    }

    public GridPosition GetNearestPointOnGrid(Vector3 position)
    {
        //Find the closest grid position to the given position
        //Since the order we process them doesn't really matter, I'm going to do it as a big foreach loop

        float closestDistance = 1000;
        GridPosition closestPoint = null;

        foreach (GridPosition point in gridPositions)
        {
            //Debug.Log(gridPositions[0,0,0]);
            Vector3 _point = point.position;
            float distance = Vector3.Distance(_point, position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPoint = point;
            }
        }

        //Check to make sure the point is close to *something* on the grid
        if (closestDistance <= size*0.6)
        {
            return closestPoint;
        }
        else
        {
            return null;
        }
        
        //We return the whole GridPosition because some functions care about if the point is occupied, but future functions may not.
    }

    public void OnObjectPlaced(Vector3 placedAt)
    {
        //Object has been placed at given position on grid.
        //Find point and set it to occupied
        GetNearestPointOnGrid(placedAt).occupied = true;
    }

    private void OnDrawGizmos()
    {
        //Draw a rectangle to represent the size / position of the grid

        //offset vector
        Vector3 offsetVector = new Vector3(offsetX,offsetY,offsetZ);

        //scale vector
        Vector3 scaleVector = new Vector3(boundsX,boundsY,boundsZ);

        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(transform.position + offsetVector, scaleVector);

        if (gridPositions != null)
        {
            foreach (GridPosition point in gridPositions)
            {
                Gizmos.DrawSphere(point.position, 0.1f);
            }
        }
        
    }
}

public class GridPosition
{
    // Small container class to hold data on a grid position
    public Vector3 position { get; private set; }
    public bool occupied = false;

    public GridPosition(Vector3 _position)
    {
        position = _position;
    }
}

