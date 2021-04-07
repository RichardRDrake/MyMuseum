using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "ArtefactData", menuName = "ScriptableObjects/ArtefactData")]
[Serializable]
public class AssetPlacerScriptableObject : ScriptableObject
{
    public string ArtefactName;

    [SerializeField] private AssetReference ArtefactPrefab; //AddressableReference (Assigned in Inspector)

    [SerializeField] private Texture2D[] PreviewImages = new Texture2D[4];

    public enum ArtefactPlacementType
    {
        FloorGrid, //Placed on the floor grid
        PlinthGrid,
        WallGrid,
        Misc        
    }

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
}
