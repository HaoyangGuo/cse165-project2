using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionInitializer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int buildingLayer = LayerMask.NameToLayer("Buildings");

        MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter meshFilter in meshFilters)
        {
            MeshCollider collider = meshFilter.gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = meshFilter.mesh;
            
            // Set the object to be on the "Buildings" layer
            meshFilter.gameObject.layer = buildingLayer;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
