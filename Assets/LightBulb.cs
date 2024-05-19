using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBulb : MonoBehaviour
{
    public GameObject connectedBattery;
    public GameObject connectedSwitch;
    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null )
        {
            Debug.LogWarning("No MeshRenderer found on object " +  gameObject.name);
        }
    }

    void Update()
    {
        if (meshRenderer == null) return;

        if (connectedBattery != null && connectedSwitch != null)
        {
            Battery battery = connectedBattery.GetComponent<Battery>();
            Switch switchComponent = connectedSwitch.GetComponent<Switch>();

            if (switchComponent.isOn)
            {
                float voltage = battery.GetVoltage();
                // Becul se aprinde dac? tensiunea este peste un anumit prag
                if (voltage > 5f)
                {
                    meshRenderer.material.color = Color.yellow; // Bec aprins
                    Debug.Log("LightBulb on!");
                }
                else
                {
                    meshRenderer.material.color = Color.black; // Bec stins
                    Debug.Log("LightBulb on!");
                }
            }
            else
            {
                meshRenderer.material.color = Color.black; // Bec stins dac? întrerup?torul este oprit
            }
        }
    }
}
