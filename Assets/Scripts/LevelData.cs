using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class LevelData
{

    public string date;

    public LevelData (Level level)
    {

        date = level.date;
    }

}
