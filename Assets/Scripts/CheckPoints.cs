using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CheckPoints : MonoBehaviour
{   
    public static CheckPoints Instance { get; private set; }
    
    private string path = "Assets/Resources/Sample-track.txt";
    public List<Vector3> positions;

    private LineRenderer lineRenderer;
    public GameObject checkPointPrefab;
    
    // Start is called before the first frame update
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
        
        positions = ParseCheckPointsFile();
        for (int i = 0; i < positions.Count; i++)
        {
            GameObject checkPoint = Instantiate(checkPointPrefab, new Vector3(positions[i][0], positions[i][1], positions[i][2]), Quaternion.identity);
            checkPoint.name = $"CheckPoint,{i}";
        }
        
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = positions.Count;
        lineRenderer.loop = false;
        lineRenderer.SetPositions(positions.ToArray());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    List<Vector3> ParseCheckPointsFile ()
    {
        float ScaleFactor = 1.0f/39.37f;
        List<Vector3> positions = new List<Vector3>();

        StreamReader sr = new StreamReader(path);
        string content = sr.ReadToEnd();
        string[] lines = content.Split('\n');
        
        for (int i = 0; i < lines.Length; i++)
        {
            string[] coords = lines[i].Split(' ');
            Vector3 pos = new Vector3(float.Parse(coords[0]),
                float.Parse(coords[1]), float.Parse(coords[2]));
            positions.Add(pos * ScaleFactor);
        }
        
        return positions;
    }

}
