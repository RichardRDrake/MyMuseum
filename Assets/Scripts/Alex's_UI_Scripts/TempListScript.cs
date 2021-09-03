using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempListScript : MonoBehaviour
{
    //Just a script will a well-known list in it, to distinguish between menus
    public List<AssetPlacerScriptableObject> readFrom = new List<AssetPlacerScriptableObject>();
    [SerializeField] public ArtefactCategory Category;
    public string folderName;

    void Awake()
    {
        readFrom = GetList(Category, folderName);
    }
    //searches through the resources folder to find the asset scriptable objects by folder name and by category
    public List<AssetPlacerScriptableObject> GetList(ArtefactCategory category, string Foldername)
    {
        Object[] objectList = Resources.LoadAll(Foldername, typeof(AssetPlacerScriptableObject));

        List<AssetPlacerScriptableObject> list = new List<AssetPlacerScriptableObject>();

        foreach (AssetPlacerScriptableObject t in objectList)
        {
            if (t.CategoryType == category)
            {
                list.Add(t);
                Debug.Log(t.name);
            }
        }

        return list;
    }
}
