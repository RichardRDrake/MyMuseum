using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a demo script to instantiate a placeable object
/// </summary>
public class DC_ObjectCreator : MonoBehaviour
{
    [Header("Demo placement object")]
    public DC_Placeable[] _DemoObjectPrefabs;

    public void Button_CreatePlaceable(int id)
    {
        id = Mathf.Clamp(id, 0, _DemoObjectPrefabs.Length - 1);

        Instantiate(_DemoObjectPrefabs[id], transform, false);
    }
}
