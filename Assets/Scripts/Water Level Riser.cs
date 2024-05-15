using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaterLevelRiser : MonoBehaviour
{
    public float gameLengthInMinutes = 5f;

    private TMP_Text timerText;
    private float targetCeiling;
    private float gameTimeElapsed = 0f;
    private Vector3 targetPosition;
    private float initialDistanceToTarget;
    private float movementSpeed;

    void Start()
    {
        timerText = GameObject.Find("[BuildingBlock] Camera Rig/TrackingSpace/CenterEyeAnchor/Text (TMP)").GetComponent<TMP_Text>();
        GameObject background = timerText.transform.GetChild(0).gameObject;
        background.SetActive(true);

        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        MRUKAnchor ceilingAnchor = room.GetCeilingAnchor();

        targetCeiling = ceilingAnchor.GetAnchorCenter().y;

        targetPosition = new Vector3(transform.position.x, targetCeiling, transform.position.z);

        initialDistanceToTarget = Vector3.Distance(transform.position, targetPosition);
        movementSpeed = initialDistanceToTarget / (gameLengthInMinutes * 60);
    }

    private void Update()
    {
        gameTimeElapsed += Time.deltaTime;
        UpdateTimer();
        MoveTowardsTarget();
    }

    private void UpdateTimer()
    {
        float remainingTime = Mathf.Max(0, (gameLengthInMinutes * 60) - gameTimeElapsed);

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void MoveTowardsTarget()
    {
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (distanceToTarget > 0.1f)
        {
            float step = movementSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
}