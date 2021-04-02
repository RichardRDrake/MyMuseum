using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class LevelData
{

    // Date variable used for displaying saves.
    public string date;

    // Arrays for holding object positions, size set to 200 for now (cap of objects in scene)
    public float[] objPositionsX = new float[200];
    public float[] objPositionsY = new float[200];
    public float[] objPositionsZ = new float[200];

    public float[] objRotationsX = new float[200];
    public float[] objRotationsY = new float[200];
    public float[] objRotationsZ = new float[200];

    public List<string> objNameList = new List<string>();

    public List<string> objTextDesc = new List<string>();


    public LevelData(Level level)
    {
        // Filling variables with level data for packaging into file

        objNameList = level.objNameList;

        objTextDesc = level.objTextDesc;
        date = level.date;

        for (int i = 0; i < level.objPositionList.Length; i++)
        {
            objPositionsX[i] = level.objPositionList[i].x;
            objPositionsY[i] = level.objPositionList[i].y;
            objPositionsZ[i] = level.objPositionList[i].z;

            objRotationsX[i] = level.objPositionList[i].x;
            objRotationsY[i] = level.objPositionList[i].y;
            objRotationsZ[i] = level.objPositionList[i].z;
        }
    }

}
