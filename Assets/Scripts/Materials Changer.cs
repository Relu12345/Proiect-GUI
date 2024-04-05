using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialsChanger : MonoBehaviour
{
    [SerializeField] Material mat;

    public void ChangeSkyboxToSolid(Camera cam)
    {
        cam.clearFlags = CameraClearFlags.SolidColor;
    }

    public void ChangeHandMaterial(SkinnedMeshRenderer mr)
    {
        if (mr.material == null )
        {
            mr.material = mat;
        }
        else
        {
            mr.material = null;
        }
    }
}
