using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{

    public GameObject cam1;
    public GameObject cam2;

    // Start is called before the first frame update
    void Start()
    {
        // By default, the game begins using the third person cam
        cam1.SetActive(true);
        cam2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // If "k" is pressed,
        if (Input.GetKeyUp("k"))
        {
            // Flips active states on cameras (e.g. if cam1 is active, cam1 is now inactive and cam2 is now active, after pressing "k")
            cam1.SetActive(!cam1.activeSelf);
            cam2.SetActive(!cam2.activeSelf);
        }
    }
}
