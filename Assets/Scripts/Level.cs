using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Level : MonoBehaviour
{


    // Lists to be accessed by UI for displaying to to user
    public List<string> SavesList = new List<string>();
    public List<string> DatesList = new List<string>();

    public string name;
    public string date = System.DateTime.Now.ToString("dd-MM-yyyy");

    void Start()
    {
        date = System.DateTime.Now.ToString("dd-MM-yyyy");
        string filepath = Application.persistentDataPath;
        DirectoryInfo dir = new DirectoryInfo(filepath);
        FileInfo[] info = dir.GetFiles("*.save");

        foreach (FileInfo f in info)
        {
            
            //Debug.Log(f.Name);
            SavesList.Add(f.Name);
            DatesList.Add(SaveLevel.LoadLevel(f.Name).date);
        }

    }

    // Update is called once per frame
    void Update()
    {

        // Temporary functions for demonstrating functionality/testing

        // Pressing "J" will create a file with the currently set name (editable during runtime as a public variable)
        if (Input.GetKeyDown("j"))
        {
            SaveLevel.CreateLevel(this, name + ".save");

            // Will also add this file to the list (will however create duplicates if the same save file is created twice (even though the file will be overwritten by the function))
            SavesList.Add(name + ".save");
            DatesList.Add(date); 

            // Add level to list
        }

        // Pressing "H" wil load the file with the currently set name (based, again, on the public variable)
        // The date value of the file will then be logged to console, to confirm load was successful.
        if (Input.GetKeyDown("h"))
        {
            LevelData data = SaveLevel.LoadLevel(name + ".save");
           
            Debug.Log(data.date);
        }
    }

    public void SaveNewLevel(string name)
    {
        SaveLevel.CreateLevel(this, name);
        SavesList.Add(name + ".save");
    }

    public void LoadNewLevel(string name)
    {
        LevelData data = SaveLevel.LoadLevel(name);

        // TO-DO Populate level object with the data, read from file (need to know how we want to do this)
        // For now, only difference when loading a new level is the level's date
        date = data.date;

    }



}


