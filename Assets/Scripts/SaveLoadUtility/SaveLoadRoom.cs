﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SaveLoadRoom : MonoBehaviour
{

    [SerializeField] private AssetReference savedRoom; //Temp, not useful until there are more rooms to save
    private GameObject spawnedRoom;

    public List<string> savesList = new List<string>();

    private AssetPlacer placer;

    [SerializeField] private bool isMainMenu = false;

    private void Awake()
    {        
        //Get the names of current saves and append them to savesList
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] info = dir.GetFiles("*.save*");

        foreach (FileInfo file in info)
        {
            savesList.Add(file.Name);
            //Debug.Log(file.Name);
        }

    }

    private void Start()
    {
        //save();
        //load();
        //MakeRoom(savedRoom.AssetGUID, null, false);
        if (isMainMenu)
        {
            return;
        }

        placer = FindObjectOfType<AssetPlacer>();

        //If the player choose to load a room from the start menu, load that here
        if (PlayerPrefs.GetInt("RoomSetup") == 0)
        {
            //auto-load a room
            if (!PlayerPrefs.HasKey("CurrentSave"))
            {
                //Make back-up room
                MakeRoom(savedRoom.AssetGUID, null, false);
            }

            int save = PlayerPrefs.GetInt("CurrentSave");
            Load(savesList[save]);
        }
    }

    public BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        SurrogateSelector selector = new SurrogateSelector();

        Vector3SerializationSurrogate v3Surrogate = new Vector3SerializationSurrogate();
        QuaternionSerializationSurrogate qSurrogate = new QuaternionSerializationSurrogate();
        //AssetSOSerializationSurrogate aSurrogate = new AssetSOSerializationSurrogate();

        selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), v3Surrogate);
        selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), qSurrogate);
        //selector.AddSurrogate(typeof(AssetPlacerScriptableObject), new StreamingContext(StreamingContextStates.All), aSurrogate);

        formatter.SurrogateSelector = selector;

        return formatter;
    }

    public void Save(string name)
    {
        //Step 0 - Check the given name is legal
        name = IllegalCharacterCheck(name);
        
        //Step 1 - Get the Room we're using
        // Since Room selection isn't a thing yet, this will have to wait
        // Instead, we'll just pretend we found it lol
        string GUID = ARtoGuid(savedRoom);
        RoomData savedRoomData = new RoomData(GUID);

        //Step 2 - Find all the Artefacts placed in the room, save their GUID and Position (and rotation)
        //Step 2.1 - Find the GridManager, it contains all the grids in the room, which in turn contain everything we'd want to save
        GridManager gridManager = FindObjectOfType<GridManager>();
        if (!gridManager)
        {
            Debug.Log("Could not find Grid Manager in scene!");
        }

        //Step 2.2 - Go through each grid and extract the data we need
        //Step 2.3 - Then bundle each of those into a AssetData class (see RoomData.cs)
        int numberOfGrids = gridManager.GetGridCount();

        for (int i = 0; i < numberOfGrids; i++)
        {
            PlacementGrid grid = gridManager.GetGrid(i);
            foreach (GridPosition pos in grid.gridPositions)
            {
                ProcessContent(pos, savedRoomData);
            }
        }


        BinaryFormatter formatter = GetBinaryFormatter();
        string filepath = Application.persistentDataPath +"/" + name +".save";
        FileStream stream = new FileStream(filepath, FileMode.Create);

        formatter.Serialize(stream, savedRoomData);

        stream.Close();

        savesList.Add( name +".save");
        Debug.Log("Saved Room Data of size " + savedRoomData.Assets.Count);
    }

    public void Load(string name)
    {
        //When room placement's sorted we'll clear out the room here

        //Clear out all current objects
        GridManager gridManager = FindObjectOfType<GridManager>();
        if (!gridManager)
        {
            Debug.Log("Could not find Grid Manager in scene!");
        }

        //Clean up
        gridManager.ClearGrids();
        Destroy(spawnedRoom);
        //spawnedObjects.Clear();

        string filepath = Application.persistentDataPath + "/" + name;
        if (File.Exists(filepath))
        {
            BinaryFormatter formatter = GetBinaryFormatter();
            FileStream stream = new FileStream(filepath, FileMode.Open);

            RoomData data = formatter.Deserialize(stream) as RoomData;
            stream.Close();

            //Spawn the RoomObject
            MakeRoom(data.roomString, data.Assets, true);

            //Make room calls the rest of the function, as it is asyncronous                       
        }
        else
        {
            Debug.LogError("Save file not found");
        }
    }

    public void DeleteRoom(string name)
    {
        //Get the full file path
        string filepath = Application.persistentDataPath + "/" + name;
        if (File.Exists(filepath))
        {
            File.Delete(filepath);
            savesList.Remove(name);
        }
        else
        {
            Debug.LogError("Save file not found");
        }
    }

    private void MakeRoom(string GUID, List<AssetData> assetData, bool b)
    {
        AssetReference asset = new AssetReference(GUID);
        //GameObject spawnedAsset;

        asset.InstantiateAsync(Vector3.zero, Quaternion.identity).Completed += op =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded && b)
            {
                //once the room has been spawned, spawn the assets
                spawnedRoom = op.Result;
                SpawnAssets(assetData);
            }
        };
    }

    private void SpawnAssets(List<AssetData> assetData)
    {
        foreach (AssetData asset in assetData)
        {
            MakeAsset(asset, placer);
            //Spawned Asynchronously, wont be ready immidiately
            //spawnedObjects.Add(spawned);
            //AssetData assetData = data.Assets[i];
        }
    }

    private void MakeAsset(AssetData assetData, AssetPlacer placer)
    {
        string GUID = assetData.assetString;

        Vector3 pos = assetData.assetPos;
        Quaternion rot = assetData.assetRot;

        AssetReference asset = new AssetReference(GUID);
        GameObject spawnedAsset;

        //asset.LoadAssetAsync<GameObject>();
        asset.InstantiateAsync(pos, rot).Completed += op =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                spawnedAsset = op.Result;
                //Attach each object to the grid (closest point will be correct spot, or not make a difference)
                GridPosition gPos = placer.PointToGrid(spawnedAsset.transform.position, spawnedAsset).gridPosition;                    
                gPos.occupied = new Asset(assetData.assetName,assetData.assetContent, assetData.assetString, assetData.assetPlacement, op.Result);
            }
        };
    }
    

    private string ARtoGuid(AssetReference AR)
    {
        string badGUID = AR.ToString();
        char[] badChars = { '[', ']', ' ' };

        string GUID = badGUID.Trim(badChars);

        return GUID;
    }

    private void ProcessContent(GridPosition pos, RoomData roomData)
    {
        //If the GridPos is empty, we dont care
        if (pos.occupied == null)
        {
            return;
        }

        //There is content to record
        //Make a new AssetData, and add it to the RoomData list
        string GUID = pos.occupied.AssRef;

        AssetData data = new AssetData(GUID, pos.occupied, pos.position, Quaternion.identity);

        roomData.Assets.Add(data);

    }

    private string IllegalCharacterCheck( string str)
    {
        //There are a bunch of characters that are not allowed to be included in a file name
        char[] badChars = { '<', '>', ':', '"', '/', '|', '?', '*', '\\' };

        str = str.Trim(badChars);

        //There are also a bunch of bad names that may brick a computer if we allow them
        string[] badNames = { "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9"
                                , "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"};

        if (Array.Exists(badNames, element => element == str))
        {
            Debug.Log("Illegal name! Replacing with InvalidName");
            str = "InvalidName";
        }

        return str;
    }

    public string GetDate(string saveName)
    {
        //Get the names of current saves and append them to savesList
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
        //Debug.Log(saveName);
        try
        {
            FileInfo[] info = dir.GetFiles(saveName);

            foreach (FileInfo file in info)
            {
                DateTime time = file.LastWriteTimeUtc;
                return time.ToString();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }       

        return "----/--/--";
    }

    /*
     * NOTES
     *  AssetReference -> ToString() -> Serialize(Save) -> Deserialize(Load) -> AssetReference(String) -> InstantiateAsync(AssetReference)
    */
}

public class Vector3SerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Vector3 v3 = (Vector3)obj;
        info.AddValue("x", v3.x);
        info.AddValue("y", v3.y);
        info.AddValue("z", v3.z);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Vector3 v3 = (Vector3)obj;
        v3.x = (float)info.GetValue("x", typeof(float));
        v3.y = (float)info.GetValue("y", typeof(float));
        v3.z = (float)info.GetValue("z", typeof(float));

        obj = v3;
        return obj;
    }
}

public class QuaternionSerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Quaternion q = (Quaternion)obj;
        info.AddValue("x", q.x);
        info.AddValue("y", q.y);
        info.AddValue("z", q.z);
        info.AddValue("w", q.w);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Quaternion q = (Quaternion)obj;
        q.x = (float)info.GetValue("x", typeof(float));
        q.y = (float)info.GetValue("y", typeof(float));
        q.z = (float)info.GetValue("z", typeof(float));
        q.w = (float)info.GetValue("w", typeof(float));

        obj = q;
        return obj;
    }
}

