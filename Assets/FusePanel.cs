using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FusePanel : MonoBehaviour
{
    public Fusible[] fuses; // Array de fusibili
    public float maxCurrent = 10f; // Curentul maxim suportat
    public float currentLoad = 0f; // Curentul actual care trece prin panoul cu siguran?e

    void Update()
    {
        if (currentLoad > maxCurrent)
        {
            foreach (Fusible fuse in fuses)
            {
                if (fuse.isIntact)
                {
                    fuse.BlowFuse();
                    Debug.Log("Blown fuse");
                }
            }
        }
    }

    public void ResetFuses()
    {
        foreach (Fusible fuse in fuses)
        {
            fuse.ResetFuse();
        }
    }
}
