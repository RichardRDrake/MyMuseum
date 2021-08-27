using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempListScript : MonoBehaviour
{
    //Just a script will a well-known list in it, to distinguish between menus
    public List<AssetPlacerScriptableObject> readFrom = new List<AssetPlacerScriptableObject>();
    [SerializeField] public ArtefactCategory FolderName;

    void Awake()
    {
        //readFrom = GetList(FolderName);
        //var objects = Resources.LoadAll(FolderName);

        //foreach (AssetPlacerScriptableObject t in objects)
        //{
        //    readFrom.Add(t);
        //    Debug.Log(objects[0].name);
        //}

    }
    public List<AssetPlacerScriptableObject> GetList(ArtefactCategory folderName)
    {
        Object[] textureList = Resources.LoadAll("FineArt", typeof(AssetPlacerScriptableObject));

        List<AssetPlacerScriptableObject> list = new List<AssetPlacerScriptableObject>();

        foreach (AssetPlacerScriptableObject t in textureList)
        {
            if (t.CategoryType == folderName)
            {
                list.Add(t);
                Debug.Log(t.name);
            }
        }

        return list;
    }
}
