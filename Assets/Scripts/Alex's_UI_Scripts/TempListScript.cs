using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempListScript : MonoBehaviour
{
    public List<string> readFrom = new List<string>();

    // Start is called before the first frame update
    void Awake()
    {
        readFrom.Add("Mercury");
        readFrom.Add("Venus");
        readFrom.Add("Earth");
        readFrom.Add("Mars");
        readFrom.Add("Jupiter");
        readFrom.Add("Saturn");
        readFrom.Add("Uranus");
        readFrom.Add("Neptune");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
