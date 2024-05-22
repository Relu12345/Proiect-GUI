using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEnd : MonoBehaviour
{
    private GameObject targetObject;

    void Start()
    {
        GameObject object1 = GameObject.Find("Cheie(Clone)");
        GameObject object2 = GameObject.Find("Lacat(Clone)");

        if (object1 == null || object2 == null)
        {
            Debug.LogError("Objects not found");
            return;
        }

        if (this.gameObject == object1)
        {
            targetObject = object2;
        }
        else if (this.gameObject == object2)
        {
            targetObject = object1;
        }
        else
        {
            Debug.LogError("Script is not attached to the correct object");
            return;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Lock"))
        {
            EndGame();
        }
    }

    public void EndGame()
    {
        SnapInteractor snap = GetComponentInChildren<SnapInteractor>();
        if (snap.SelectedInteractable)
        {
            TMP_Text text1 = GameObject.Find("[BuildingBlock] Camera Rig/TrackingSpace/CenterEyeAnchor/Text (TMP) (1)").GetComponent<TMP_Text>();
            text1.text = "CONGRATULATIONS";
            ParticleSystem confetti = GameObject.Find("[BuildingBlock] Camera Rig").GetComponentInChildren<ParticleSystem>();
            confetti.enableEmission = true;
            WaterLevelRiser wlr = GameObject.Find("Water(Clone)").GetComponent<WaterLevelRiser>();
            wlr.WaterEndGame();
        }
    }
}
