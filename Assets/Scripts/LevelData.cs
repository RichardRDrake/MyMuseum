using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class LevelData
{

    public int placeholder;

    public LevelData (Level level)
    {

        placeholder = level.placeholder;
    }

}
