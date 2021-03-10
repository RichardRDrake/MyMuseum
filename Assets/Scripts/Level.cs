using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{


    // Placeholder variable
    // This is the variable that will be put into the save file, when saving takes place
    public int placeholder = 5;

    // Update is called once per frame
    void Update()
    {

        // For now, if J is pressed, the "level" will be saved to a new file.
        if (Input.GetKeyDown("j"))
        {
            SaveLevel.SaveLevels(this);
        }
        if (Input.GetKeyDown("h"))
        {
            LevelData data = SaveLevel.LoadLevels();
            Debug.Log(data.placeholder);
        }
    }
}


