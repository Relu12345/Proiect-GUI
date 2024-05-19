using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fusible : MonoBehaviour
{
    public bool isIntact = true;

    public void BlowFuse()
    {
        isIntact = false;
        Debug.Log(gameObject.name + " fuse has blown.");
        GetComponent<MeshRenderer>().material.color = Color.red;
    }

    public void ResetFuse()
    {
        isIntact = true;
        Debug.Log(gameObject.name + " fuse has been reset.");
        GetComponent<MeshRenderer>().material.color = Color.gray;
    }
}
