using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempListScript : MonoBehaviour
{
    //Just a script will a well-known list in it, to distinguish between menus
    public List<AssetPlacerScriptableObject> readFrom = new List<AssetPlacerScriptableObject>();

    /*void Awake()
    {
        //Populates the script with a list of planets
        readFrom.Add("Mercury");
        readFrom.Add("Venus");
        readFrom.Add("Earth");
        readFrom.Add("Mars");
        readFrom.Add("Jupiter");
        readFrom.Add("Saturn");
        readFrom.Add("Uranus");
        readFrom.Add("Neptune");
    }*/
}
