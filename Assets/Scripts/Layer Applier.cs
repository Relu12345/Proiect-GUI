using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerApplier : MonoBehaviour
{
    public void ApplyLayerForRoom()
    {
        MRUKRoom room = FindObjectOfType<MRUKRoom>();
        GameObject roomObject = room.gameObject;

        ApplyLayer(roomObject, "Wall");
    }

    private void ApplyLayer(GameObject obj, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        obj.layer = layer;

        foreach (Transform child in obj.transform) { 
            ApplyLayer(child.gameObject, layerName);
        }
    }
}
