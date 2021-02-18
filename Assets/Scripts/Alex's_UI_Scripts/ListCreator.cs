using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListCreator : MonoBehaviour
{
    //ListCreator is purely a stand-in for whatever the list script is reading from or is written to.
    //It shouldn't exist in the real build.
    public List<string> writeTo = new List<string>();

    //Unique identifier for which menu this is
    [SerializeField] private int MenuNumber;

    [SerializeField] private GameObject Controller;
    private UI_Controller UI_Controller;
    private TempListScript TempListScript;

    // Start is called before the first frame update
    void Start()
    {
        UI_Controller = Controller.GetComponent<UI_Controller>();
        TempListScript = GetComponent<TempListScript>();
        for(int i = 0; i < writeTo.Count; i++)
        {
            TempListScript.readFrom.Add(writeTo[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WasPressed()
    {
        //Tells UI_Controller to read from this specific instance
        //Then sets it up to begin populating its menus.
        UI_Controller.switchLists = MenuNumber;
        UI_Controller.displayMenu = true;
    }
}
