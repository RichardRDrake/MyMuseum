using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetPlacerScriptableObject : ScriptableObject
{
    public string prefabName;

    public int numberOfPrefabsToCreate;
    public Vector3[] spawnPoints;
}
