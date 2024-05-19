using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPress : MonoBehaviour
{
    public Switch circuitSwitch;

    void OnMouseDown()
    {
        if (circuitSwitch != null)
        {
            Debug.Log("Button pressed!");
            circuitSwitch.ToggleSwitch();
        }
    }
}
