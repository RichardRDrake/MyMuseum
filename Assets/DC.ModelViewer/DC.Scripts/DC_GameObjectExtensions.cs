using UnityEngine;
using System.Collections;

public static class DC_GameObjectExtensions
{

    public static void SetLayer(this GameObject parent, int layer, bool includeChildren = true)
    {
        parent.layer = layer;
        if (includeChildren)
        {
            foreach (Transform trans in parent.transform.GetComponentsInChildren<Transform>(true))
            {
                trans.gameObject.layer = layer;
            }
        }
    }

    /// <summary>
    /// Only changes layers if they match the oldLayer
    /// Unless oldLayer set to -1
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="oldLayer"></param>
    /// <param name=""></param>
    /// <param name="newLayer"></param>
    /// <param name="includeChildren"></param>
    public static void SafeSetLayer(this GameObject parent, int oldLayer, int newLayer, bool includeChildren = true)
    {
        parent.layer = newLayer;

        if (includeChildren)
        {
            foreach (Transform trans in parent.transform.GetComponentsInChildren<Transform>(true))
            {
                if (trans.gameObject.layer == oldLayer || oldLayer == -1)
                    trans.gameObject.layer = newLayer;
            }
        }
    }
}