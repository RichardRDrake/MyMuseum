using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class Level : MonoBehaviour
{

    // Lists to be accessed by UI for displaying to to user
    public List<string> SavesList = new List<string>();
    public List<string> DatesList = new List<string>();

    public GameObject[] objects;
    public Vector3[] objPositionList;
    public Vector3[] objRotationList;

    public List<string> objNameList = new List<string>();
    public List<string> objTextDesc = new List<string>();

    public string name;
    public string date = System.DateTime.Now.ToString("dd/MM/yyyy");

    private GameObject objectToBePlaced = null;

    void Start()
    {
        objPositionList = new Vector3[200];
        objRotationList = new Vector3[200];

        date = System.DateTime.Now.ToString("dd/MM/yyyy");
        string filepath = Application.persistentDataPath;
        DirectoryInfo dir = new DirectoryInfo(filepath);

        // Gets all .save files, stores each file in "info" array
        FileInfo[] info = dir.GetFiles("*.save");

        // For every file in the array
        foreach (FileInfo f in info)
        {
            // Adds the file name to SavesList (for UI interactions)
            SavesList.Add(f.Name);
            // Reads the file and gets the "date" value, adds it to the DatesList
            DatesList.Add(SaveLevel.LoadLevel(f.Name).date);
        }
    }

    void Update()
    {
        //We're not using update for anything right now - Alex
        #region Hide Update
        /*
        // Temporary functions for demonstrating functionality/testing

        // Pressing "J" will create a file with the currently set name (editable during runtime as a public variable)
        if (Input.GetKeyDown("j"))
        {

            SaveNewLevel(name);

        }

        // Pressing "H" wil load the file with the currently set name (based, again, on the public variable)
        // The date value of the file will then be logged to console, to confirm load was successful.
        if (Input.GetKeyDown("h"))
        {

            LoadNewLevel(name);

        }
        */
        #endregion
    }


    // Function for creating a new level file and filling it with data

    public void SaveNewLevel(string name)
    {

        // Fills GameObject array with all objects in the scene tagged with "object"
        objects = GameObject.FindGameObjectsWithTag("object");

        // For every "object" within the scene,
        if (objects.Length > 0)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                // Populates list/array with the data from the array filled with every "object"
                objPositionList[i] = objects[i].transform.position;
                objRotationList[i] = objects[i].transform.eulerAngles;
                objNameList.Add(objects[i].name);
            }
        }
        else
        { Debug.Log("No objects found"); }

        // Creates and saves the new level
        SaveLevel.CreateLevel(this, name + ".save");

        // Adds data to lists
        SavesList.Add(name + ".save");
        DatesList.Add(date);
    }

    // Function for loading a new level file and populating the active level with the file data

    public void LoadNewLevel(string name)
    {
        // Creates a new LevelData variable and loads the given level file into it
        LevelData data = SaveLevel.LoadLevel(name);


        // Takes each variable within the loaded level data and populates the "level" with them

        date = data.date;
        objTextDesc = data.objTextDesc;
        objNameList = data.objNameList;

        for (int i = 0; i < objPositionList.Length; i++)
        {
            objPositionList[i].x = data.objPositionsX[i];
            objPositionList[i].y = data.objPositionsY[i];
            objPositionList[i].z = data.objPositionsZ[i];

            objRotationList[i].x = data.objRotationsX[i];
            objRotationList[i].y = data.objRotationsY[i];
            objRotationList[i].z = data.objRotationsZ[i];
        }


        // Fills GameObject array with all objects in the scene tagged with "object"
        objects = GameObject.FindGameObjectsWithTag("object");


        //refresh level, place objects in positions
        if (objects.Length > 0)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                Destroy(objects[i]);
            }
        }
        for (int i = 0; i < data.objNameList.Count; i++)
        {
            // Load object into game,
            // Place it at data position


        }

    }

    // Function for deleting a specific file

    public void DeleteLevel(string name)
    {

        SaveLevel.DeleteLevel(name + ".save");
        int index = SavesList.IndexOf(name + ".save");

        // Removes the file from UI lists 
        SavesList.Remove(name + ".save");
        DatesList.RemoveAt(index);
    }
}

