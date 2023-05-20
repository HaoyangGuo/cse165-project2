using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct Gesture
{
    public string name;
    public List<Vector3> fingerData;
}

public class FlightControls : MonoBehaviour
{   
    public static FlightControls Instance { get; set; }
    
    public bool debugMode = true;
    public float threshold = 0.05f;

    public OVRSkeleton leftHandSkeleton;
    private List<OVRBone> leftHandFingerBones;
    public List<Gesture> leftHandGestures;

    public OVRSkeleton rightHandSkeleton;
    private OVRBone rightIndexFingerTipBone;

    private List<Vector3> checkPointsPositions;
    private float speed = 65.0f;

    public bool paused = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    // Start is called before the first frame update
    IEnumerator Start()
    {   
        while (leftHandSkeleton.Bones.Count == 0 || rightHandSkeleton.Bones.Count == 0) {
            yield return null;
        }
        
        leftHandFingerBones = new List<OVRBone>(leftHandSkeleton.Bones);
        
        rightIndexFingerTipBone = rightHandSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_IndexTip];

        checkPointsPositions = CheckPoints.Instance.positions;
        gameObject.transform.position = checkPointsPositions[0] + new Vector3(0.0f, 5.0f, 0.0f);
        Vector3 projectedPoint = new Vector3(checkPointsPositions[1].x, checkPointsPositions[0].y, checkPointsPositions[1].z);
        gameObject.transform.LookAt(projectedPoint);
    }

    // Update is called once per frame
    void Update()
    {
        if (leftHandSkeleton.Bones.Count == 0 || rightHandSkeleton.Bones.Count == 0) return;
        
        if (debugMode && Input.GetKeyDown(KeyCode.A))
        {
            SaveLeftHandGesture();
        }

        if (!paused)
        {
            Gesture currentLeftHandGesture = RecognizeLeftHand();
            bool hasRecognizedLeftHand = !currentLeftHandGesture.Equals(new Gesture());
            if (hasRecognizedLeftHand)
            {
                if (currentLeftHandGesture.name == "Fist")
                {
                    transform.position += transform.forward * (speed * Time.deltaTime);
                }
            }

            int layerMask = 1 << LayerMask.NameToLayer("OrientationButtons");
            Collider[] hitColliders = Physics.OverlapSphere(rightIndexFingerTipBone.Transform.position, 0.01f, layerMask);
            if (hitColliders.Length > 0)
            {
                switch (hitColliders[0].name)
                {
                    case "LeftButton":
                        transform.Rotate(Vector3.up, -45 * Time.deltaTime);
                        break;
                    case "RightButton":
                        transform.Rotate(Vector3.up, 45 * Time.deltaTime);
                        break;
                    case "UpButton":
                        transform.position += transform.up * (40 * Time.deltaTime);
                        break;
                    case "DownButton":
                        transform.position += -transform.up * (40 * Time.deltaTime);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void SaveLeftHandGesture()
    {
        Gesture g = new Gesture();
        g.name = "New Gesture";
        List<Vector3> data = new List<Vector3>();
        foreach (var bone in leftHandFingerBones)
        {
            data.Add(leftHandSkeleton.transform.InverseTransformPoint(bone.Transform.position));
        }

        g.fingerData = data;
        leftHandGestures.Add(g);
    }

    Gesture RecognizeLeftHand()
    {
        Gesture currentGesture = new Gesture();
        float currentMin = Mathf.Infinity;

        foreach (var gesture in leftHandGestures)
        {
            float sumDistance = 0;
            bool isDiscarded = false;
            for (int i = 0; i < leftHandFingerBones.Count; i++)
            {
                Vector3 currentData =
                    leftHandSkeleton.transform.InverseTransformPoint(leftHandFingerBones[i].Transform.position);
                float distance = Vector3.Distance(currentData, gesture.fingerData[i]);
                if (distance > threshold)
                {
                    isDiscarded = true;
                    break;
                }

                sumDistance += distance;
            }

            if (!isDiscarded && sumDistance < currentMin)
            {
                currentMin = sumDistance;
                currentGesture = gesture;
            }
        }

        return currentGesture;
    }
}
