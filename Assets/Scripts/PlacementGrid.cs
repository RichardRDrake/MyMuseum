using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementGrid : MonoBehaviour
{
    //space between objects
    [SerializeField] private float size = 1.0f; //Space between points
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
        GridManager gridManager = FindObjectOfType<GridManager>();
        if (gridManager)
        {
            gridManager.RegisterNewGrid(this);
        }
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

        if (xCount < 1)
        {
            xCount = 1;
        }
        if (yCount < 1)
        {
            yCount = 1;
        }
        if (zCount < 1)
        {
            zCount = 1;
        }

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
                    // Position per axis = (x - xCount/2) + offsetX + GridObjectPosition
                    // if there are an even number of points in this axis, additional offset is needed
                    float _gridPosX = ((x * size) - (xCount / 2) * size) + offsetX + GridObjectPosition.x;
                    if (xCount % 2 == 0)
                    {
                        _gridPosX = _gridPosX + (size / 2);
                    }
                    float _gridPosY = ((y * size) - (yCount / 2) * size) + offsetY + GridObjectPosition.y;
                    if (yCount % 2 == 0)
                    {
                        _gridPosY = _gridPosY + (size / 2);
                    }
                    float _gridPosZ = ((z * size) - (zCount / 2) * size) + offsetZ + GridObjectPosition.z;
                    if (zCount % 2 == 0)
                    {
                        _gridPosZ = _gridPosZ + (size / 2);
                    }

                    //Add to array
                    Vector3 _gridPos = new Vector3(_gridPosX, _gridPosY, _gridPosZ);
                    gridPositions[x, y, z] = new GridPosition(_gridPos);

                    //Debug.Log("Generated Grid Point " + gridPositions[x, y, z] + " at " + _gridPos);
                }
            }
        }
    }

    public NearestPointResponse GetNearestPointOnGrid(Vector3 position)
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
            return new NearestPointResponse(closestPoint, closestDistance, this);
        }
        else
        {
            return null;
        }
        
        //We return the whole GridPosition because some functions care about if the point is occupied, but future functions may not.
    }

    public void OnObjectPlaced(Vector3 placedAt, GameObject placedObject)
    {
        //Object has been placed at given position on grid.
        //Find point and set it to occupied
        GetNearestPointOnGrid(placedAt).gridPosition.occupied = placedObject;
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
    public GameObject occupied = null;

    public GridPosition(Vector3 _position)
    {
        position = _position;
    }
}

public class NearestPointResponse
{
    public GridPosition gridPosition { get; private set; }
    public float distance;
    public PlacementGrid grid;

    public NearestPointResponse(GridPosition responcePos, float responceDist, PlacementGrid parentGrid )
    {
        gridPosition = responcePos;
        distance = responceDist;
        grid = parentGrid;
    }
}

