using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class UnderwaterStateChanger : MonoBehaviour
{
    public Transform mainCamera;
    public Volume postProcessingVolume;
    public VolumeProfile surfacePostProcessing;
    public VolumeProfile underwaterPostProcessing;

    private float depth = 0;

    void Update()
    {
        depth = WaterPos();

        if (mainCamera.position.y < depth)
        {
            EnableEffects(true);
        }
        else
        {
            EnableEffects(false);
        }
    }

    private float WaterPos()
    {
        Vector3 waterPos = GameObject.Find("Water(Clone)").GetComponent<Transform>().position;
        return waterPos.y;
    }

    private void EnableEffects(bool active)
    {
        if (active)
        {
            RenderSettings.fog = true;
            postProcessingVolume.profile = underwaterPostProcessing;
        }
        else
        {
            RenderSettings.fog = false;
            postProcessingVolume.profile = surfacePostProcessing;
        }
    }
}
