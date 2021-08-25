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
    public List<AssetPlacerScriptableObject> GetTextureList(string folderName)
    {
        if (readFrom.Count > 0)
            readFrom.Clear();
        var objects = Resources.LoadAll(folderName);

        AssetPlacerScriptableObject[] textureList = (AssetPlacerScriptableObject[])Resources.FindObjectsOfTypeAll(typeof(AssetPlacerScriptableObject));

        List<AssetPlacerScriptableObject> list = new List<AssetPlacerScriptableObject>();

        foreach (AssetPlacerScriptableObject t in textureList)
        {
            if (t.PaintingPixelSize.x > 0)
            {
                list.Add(t);
                Debug.Log(t.name);
            }
        }

        return list;
    }
}
