using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ViewController : MonoBehaviour
{
    //Gameobjects and relevant components
    [SerializeField] private GameObject MainMenu;

    // Update is called once per frame
    void Update()
    {
        //Moves the player back up a menu level from wherever they were.
        if (Input.GetKeyDown("escape") || Input.GetKeyDown("backspace"))
        {
            MainMenu.SetActive(true);
            Cursor.visible = true;
        }
    }

    public void DisableMain()
    {
        //disables main menu
        MainMenu.SetActive(false);
    }
}
