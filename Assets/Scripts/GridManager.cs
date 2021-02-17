using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
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
   
}
