using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class UnderwaterStateChanger : MonoBehaviour
{
    public Transform mainCamera;
    public Texture2D normalLutTexture;
    public Texture2D blueLutTexture;
    public OVRPassthroughLayer passthroughLayer;

    private OVRPassthroughColorLut lutNormal;
    private OVRPassthroughColorLut lutBlue;
    private float depth = 0;

    private bool wasUnderwater = false;

    private void Start()
    {
        lutNormal = new OVRPassthroughColorLut(normalLutTexture, true);
        lutBlue = new OVRPassthroughColorLut(blueLutTexture, true);
    }

    void Update()
    {
        if (FloorObjectSpawner.isWaterSpawned == true)
        {
            depth = WaterPos();
            if (mainCamera.position.y < depth + 0.5f)
            {
                EnableEffects(true);
            }
            else
            {
                EnableEffects(false);
            }
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
            passthroughLayer.SetColorLut(lutNormal, lutBlue, 1);
        }
        else
        {
            passthroughLayer.SetColorLut(lutBlue, lutNormal, 1);
        }
    }
}
