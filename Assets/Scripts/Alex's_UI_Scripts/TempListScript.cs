using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempListScript : MonoBehaviour
{
    //Just a script will a well-known list in it, to distinguish between menus
    public List<AssetPlacerScriptableObject> readFrom = new List<AssetPlacerScriptableObject>();
    public string FolderName;

    void Awake()
    {
        //var objects = Resources.LoadAll(FolderName);

        //foreach (AssetPlacerScriptableObject t in objects)
        //{
        //    readFrom.Add(t);
        //    Debug.Log(objects[0].name);
        //}

    }
    public List<Texture2D> GetTextureList(string folderName)
    {
        if (readFrom.Count > 0)
            readFrom.Clear();
        var objects = Resources.LoadAll(folderName);

        List<Texture2D> textureList = new List<Texture2D>();

        foreach (Texture2D t in objects)
        {
            textureList.Add(t);
            Debug.Log(objects[0].name);
        }

        return textureList;
    }
}
