using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ListCreator : MonoBehaviour
{
    //ListCreator is purely a stand-in for whatever the list script is reading from or is written to.
    //It shouldn't exist in the real build.
    //public List<string> writeTo = new List<string>();

    //Unique identifier for which menu this is.
    //0 for floor-based objects, 1 for objects that can connect to either the floor or walls, 3 for small objects which fit in frames, 4 for planar objects

    //Connect this to UI_Build Mode.
    [SerializeField] private GameObject Controller;
    private UI_Controller UI_Controller;

    // Start is called before the first frame update
    
    void Start()
    {
        UI_Controller = Controller.GetComponent<UI_Controller>();
   
    }
    

    public void WasPressed()
    {
        //Tells UI_Controller to read from this specific instance
        //Then sets it up to begin populating its menus.
        UI_Controller.MenuInit(this.gameObject.GetComponent<TempListScript>());
        UI_Controller.displayMenu = true;
    }   
}
