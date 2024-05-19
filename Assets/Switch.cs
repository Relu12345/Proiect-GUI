using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public bool isOn = false;

    public void ToggleSwitch()
    {
        isOn = !isOn;
        Debug.Log("Switch state: " + (isOn ? "On" : "Off"));
    }
}
