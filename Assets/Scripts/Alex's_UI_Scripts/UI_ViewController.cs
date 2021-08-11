using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ViewController : MonoBehaviour
{
    //Gameobjects and relevant components
    [SerializeField] private GameObject MainMenu;
    private UI_MenuController UI_MenuController;

    // Start is called before the first frame update
    void Start()
    {
        UI_MenuController = MainMenu.GetComponent<UI_MenuController>();
    }

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
