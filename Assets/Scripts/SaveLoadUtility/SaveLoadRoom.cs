using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SaveLoadRoom : MonoBehaviour
{

    [SerializeField] private AssetReference savedRoom;

    public List<string> savesList = new List<string>(new string[] { "/Test.save" } );

    private void Start()
    {
        //save();
        //load();
    }

    public static BinaryFormatter GetBinaryFormatter()
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
        string filepath = Application.persistentDataPath + "/Test.save";
        FileStream stream = new FileStream(filepath, FileMode.Create);

        formatter.Serialize(stream, savedRoomData);

        stream.Close();

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

        gridManager.ClearGrids();

        string filepath = Application.persistentDataPath + "/Test.save";
        if (File.Exists(filepath))
        {
            AssetPlacer placer = FindObjectOfType<AssetPlacer>();

            BinaryFormatter formatter = GetBinaryFormatter();
            FileStream stream = new FileStream(filepath, FileMode.Open);

            RoomData data = formatter.Deserialize(stream) as RoomData;
            stream.Close();

            //Spawn the RoomObject
            //MakeAsset(data.roomString, Vector3.zero, Quaternion.identity);

            //Spawn all the artefacts
            //Since we dont know the order objects will be spawned in, we wont attach them to their grid positions until we're finished spawning
            List<GameObject> spawnedObjects = new List<GameObject>();

            foreach (AssetData asset in data.Assets)
            {
                spawnedObjects.Add( MakeAsset(asset.assetString, asset.assetPos, asset.assetRot) ); //Spawned Asynchronously, wont be ready immidiately
            }

            //Wait one frame
            StartCoroutine(WaitOneFrame());

            //if data.Assets and spawnedObjects are different sizes, we've got a problem
            if (spawnedObjects.Count != data.Assets.Count)
            {
                Debug.Log("Size mismatch - we gotta problem");
                return;
            }

            //Attach each object to the grid (closest point will be correct spot, or not make a difference)
            for (int i = 0; i < spawnedObjects.Count; i++)
            {
                GridPosition gPos = placer.PointToGrid(spawnedObjects[i].transform.position, spawnedObjects[i]).gridPosition;
                AssetData assetData = data.Assets[i];
                gPos.occupied = new Asset(assetData.assetName, assetData.assetString, assetData.assetPlacement);

                //Debug.Log(gPos.occupied);
            }
        }
        else
        {
            Debug.LogError("Save file not found");
        }
    }

    private GameObject MakeAsset(string GUID, Vector3 pos, Quaternion rot)
    {
        AssetReference asset = new AssetReference(GUID);

        //asset.LoadAssetAsync<GameObject>();
        GameObject spawnedAsset = asset.InstantiateAsync(pos, rot).Result;
        return spawnedAsset;
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

    private IEnumerator WaitOneFrame()
    {
        yield return 0;
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
/*
public class AssetSOSerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        AssetPlacerScriptableObject a = (AssetPlacerScriptableObject)obj;
        info.AddValue("Name", a.ArtefactName);
        info.AddValue("Ref", a.GetAssetReference().AssetGUID);
        info.AddValue("Type", (int)a.GetPlacementType());
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        AssetPlacerScriptableObject a = (AssetPlacerScriptableObject)obj;
        a.ArtefactName = (string)info.GetValue("Name", typeof(string));
        a.//ArtefactPrefab = new AssetReference((string)info.GetValue("Ref", typeof(string)));
        a.//PlacementType = (ArtefactPlacementType)((int)info.GetValue("Type", typeof(int)));

        obj = a;
        return obj;
    }
}
*/
