using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlay : MonoBehaviour
{
    public Text checkPointsCount;
    public Text timeElapsed;
    public Text dFromNextCP;
    public Text countDown;

    public GameObject dynamicArrow;
    private LineRenderer dynamicArrowLine;
    
    private List<Vector3> checkPointsPositions;
    private int totalCheckPoints;
    private int currCheckPointIndex = -1;
    private int nextCheckPointIndex = 0;
    
    
    private float timer = 0;
    private float countDownTimer = 5;
    private bool beginning = true;
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (CheckPoints.Instance.positions == null)
        {
            yield return null;
        }
        
        checkPointsPositions = CheckPoints.Instance.positions;
        totalCheckPoints = checkPointsPositions.Count;

        dynamicArrowLine = dynamicArrow.GetComponent<LineRenderer>();
        dynamicArrowLine.positionCount = 2;
        
        timeElapsed.text = "Time: 0.00";
    }

    // Update is called once per frame
    void Update()
    {
        if (checkPointsPositions == null) return;

        if (countDownTimer > 0)
        {
            countDownTimer -= Time.deltaTime;
            if (countDown != null)
            {
                countDown.text = $"{Convert.ToInt32(countDownTimer)}";
            }
            FlightControls.Instance.paused = true;
        }
        else
        {
            if (countDown != null)
            {
                countDown.text = "";
            }
            FlightControls.Instance.paused = false;
            beginning = false;
        }

        if (nextCheckPointIndex < checkPointsPositions.Count)
        {   
            // Wayfinding Dynamic Arrow
            dynamicArrow.transform.LookAt(checkPointsPositions[nextCheckPointIndex]);
            dynamicArrowLine.SetPosition(0, dynamicArrow.transform.position + dynamicArrow.transform.forward);
            dynamicArrowLine.SetPosition(1, checkPointsPositions[nextCheckPointIndex]);
            
            // Timer
            if (beginning == false)
            {
                timer += Time.deltaTime;
                if (timeElapsed != null)
                {
                    timeElapsed.text = $"Time: {timer.ToString("0.00")}s";
                }
            }

            // Distance
            if (dFromNextCP != null)
            {
                dFromNextCP.text =
                    $"Next: {Convert.ToInt32(Vector3.Distance(gameObject.transform.position, checkPointsPositions[nextCheckPointIndex]))} m";
            }
            
            // Detect Crash
            int layerMask = 1 << LayerMask.NameToLayer("Buildings");
            Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, 0.1f, layerMask);
            if (hitColliders.Length > 0 && currCheckPointIndex != -1)
            {
                gameObject.transform.position = checkPointsPositions[currCheckPointIndex];
                gameObject.transform.LookAt(new Vector3(checkPointsPositions[nextCheckPointIndex].x, checkPointsPositions[currCheckPointIndex].y, checkPointsPositions[nextCheckPointIndex].z));
                countDownTimer = 3.0f;
            }
            
            // Detect Check Point
            layerMask = 1 << LayerMask.NameToLayer("CheckPoints");
            hitColliders = Physics.OverlapSphere(gameObject.transform.position, 0.1f, layerMask);
            if (hitColliders.Length > 0)
            {   
                // Debug.Log($"name: {hitColliders[0].name}");
                if (hitColliders[0].name.Contains("CheckPoint"))
                {
                    int checkPointIndex = Int32.Parse(hitColliders[0].name.Split(",")[1]);
                    if (checkPointIndex == nextCheckPointIndex)
                    {
                        currCheckPointIndex = checkPointIndex;
                        nextCheckPointIndex = checkPointIndex + 1;
                        checkPointsCount.text = $"Check Points: {nextCheckPointIndex}" + " / " + $"{totalCheckPoints}";
                    }
                    // Debug.Log($"next: {nextCheckPointIndex}");
                }
            }
        }
        else
        {
            if (dFromNextCP != null)
            {
                dFromNextCP.text =
                    $"Next: 0 m";
                FlightControls.Instance.paused = true;
            }
        }
    }
}
