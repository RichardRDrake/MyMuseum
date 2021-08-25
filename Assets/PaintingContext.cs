using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintingContext : MonoBehaviour
{
    // Start is called before the first frame update
    public string Context;
    public string Name;
    void Start()
    {
        Name = this.gameObject.name;

       Debug.Log( GetComponent<Image>().mainTexture.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
