using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArtefactData", menuName = "ScriptableObjects/ArtefactData")]
public class AssetPlacerScriptableObject : ScriptableObject
{
    public string ArtefactName;

    [SerializeField] private GameObject ArtefactPrefab; //I didn't want to use preabs, but it appears to be needed*

    [SerializeField] private Texture2D[] PreviewImages = new Texture2D[4];

    public enum ArtefactPlacementType
    {
        FloorGrid, //Placed on the floor grid
        PlinthGrid,
        WallGrid,
        Misc        
    }

    [SerializeField] ArtefactPlacementType PlacementType = ArtefactPlacementType.Misc; //Overriden in Inspector

    public GameObject GetArtefact()
    {

        if (!ArtefactPrefab)
        {
            Debug.Log("Artefact missing content!");
            return null;
        }

        return ArtefactPrefab;
    }
}
