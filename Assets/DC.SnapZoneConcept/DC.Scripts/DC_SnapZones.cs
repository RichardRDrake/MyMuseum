using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a demo script to instantiate the Snap zones as "DC_SnapZone" as a proof of concept
/// Should be used on the Room Prefab
/// </summary>
public class DC_SnapZones : MonoBehaviour
{
    [Header("Sprite settings")]
    public GameObject _NonDirectionalZoneSpritePrefab;
    public GameObject _DirectionalZoneSpritePrefab;

    [Header("Snap zone settings")]
    [Tooltip("How far from a snap zone an object being placed can be before being snapped to the nearest zone")]
    [Range(0.01f, 1.0f)] public float _Threshold = 0.1f;

    void Start()
    {
        foreach(Transform transformComponent in GetComponentsInChildren<Transform>())
        {
            if(transformComponent.name.Contains("Snap_Zone_Floor_Directional"))
            {
                GameObject temp = Instantiate(_DirectionalZoneSpritePrefab, transform, false);
                temp.name = transformComponent.name;
                temp.transform.position = transformComponent.position + temp.transform.localPosition;
                temp.transform.rotation = transformComponent.rotation;
                temp.transform.localScale = Vector3.one;
                temp.layer = LayerMask.NameToLayer("FloorSnapDirectional");
                DC_SnapZone script = temp.GetComponent<DC_SnapZone>();
                script.Init(_Threshold, true);
            }
            else if (transformComponent.name.Contains("Snap_Zone_Floor"))
            {
                GameObject temp = Instantiate(_NonDirectionalZoneSpritePrefab, transform, false);
                temp.name = transformComponent.name;
                temp.transform.position = transformComponent.position + temp.transform.localPosition;
                temp.transform.localScale = Vector3.one;
                temp.layer = LayerMask.NameToLayer("FloorSnap");
                DC_SnapZone script = temp.GetComponent<DC_SnapZone>();
                script.Init(_Threshold);
            }
            else if(transformComponent.name.Contains("Snap_Zone_Wall"))
            {
                GameObject temp = Instantiate(_NonDirectionalZoneSpritePrefab, transform, false);
                temp.name = transformComponent.name + "_Directional";
                temp.transform.rotation = transformComponent.rotation;
                temp.transform.position = transformComponent.position + temp.transform.localPosition;
                temp.transform.localScale = Vector3.one;
                temp.layer = LayerMask.NameToLayer("WallSnap");
                DC_SnapZone script = temp.GetComponent<DC_SnapZone>();
                script.Init(_Threshold, true);
            }
        }
    }
}
