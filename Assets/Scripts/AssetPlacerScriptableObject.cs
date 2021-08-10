﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "ArtefactData", menuName = "ScriptableObjects/ArtefactData")]
[Serializable]
public class AssetPlacerScriptableObject : ScriptableObject
{
    public string ArtefactName;

    public string ArtefactContent;

    [SerializeField] private AssetReference ArtefactPrefab; //AddressableReference (Assigned in Inspector)

    public Vector2 PaintingPixelSize;

    public Texture2D[] PreviewImages = new Texture2D[4];    

    [SerializeField] ArtefactPlacementType PlacementType = ArtefactPlacementType.Misc; //Overriden in Inspector

    public AssetReference GetArtefact()
    {

        if (ArtefactPrefab.RuntimeKey == null)
        {
            Debug.Log("Artefact missing content!");
            return null;
        }

        return ArtefactPrefab;
    }
    public AssetReference GetAssetReference()
    {
        return ArtefactPrefab;
    }

    public ArtefactPlacementType GetPlacementType()
    {
        return PlacementType;
    }
}

public enum ArtefactPlacementType
{
    FloorGrid, //Placed on the floor grid
    PlinthGrid,
    WallGrid,
    Rooms,
    Misc
}

public class Asset
{
    public string Name;
    public string Content;
    public string AssRef;
    public Vector2 pixelSize;
    public ArtefactPlacementType placementType;

    public GameObject asset;

    public Asset(string name, string content, string ar, ArtefactPlacementType apt, Vector2 pixels, GameObject _asset)
    {
        Name = name;
        Content = content;
        AssRef = ar;
        placementType = apt;
        pixelSize = pixels;
        asset = _asset;
    }
}

