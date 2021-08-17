﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class RoomData
{
    //What data do we need to record, in order to be able to re-construct the room later?
    // 0 The name of the Save
    // 1 The Room itself (Addressable prefab)
    // 2 The Objects placed on the grid (Addressable prefabs)
    // 3 The Position of each of those objects

    public string saveName;

    //The Room itself
    public string roomString;
    public List<AssetData> Assets;

    public RoomData(string assetString)
    {
        roomString = assetString;
        Assets = new List<AssetData>();
    }
}

[Serializable]
public class AssetData
{
    public string assetString;
    //public AssetPlacerScriptableObject scriptObject;
    public Vector3 assetPos;
    public Quaternion assetRot;
    public Vector2 pixelSize;

    public string assetName;
    public string assetContent;
    public ArtefactPlacementType assetPlacement;

    public AssetData(string GUID, Asset asset, Vector3 pos, Quaternion rot)
    {
        assetString = GUID;
        assetName = asset.Name;
        assetContent = asset.Content;
        assetPlacement = asset.placementType;
        pixelSize = asset.pixelSize;
        assetPos = pos;
        assetRot = rot;
    }
}

//d94b37ac3c9111a40bcd3a9305f8ba0b