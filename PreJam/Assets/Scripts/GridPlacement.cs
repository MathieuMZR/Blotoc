using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class GridPlacement : MonoBehaviour
{
    public Vector3 offset;
    
    #if UNITY_EDITOR
    // Update is called once per frame
    void Update()
    {
        if (Selection.activeObject == gameObject)
        {
            SnapToGrid(1);
        }
    }
    
    private void SnapToGrid(float gridSize)
    {
        Vector3 position = transform.position;

        // Calculate the snapped position
        float snappedX = Mathf.Round(position.x / gridSize) * gridSize;
        float snappedY = Mathf.Round(position.y / gridSize) * gridSize;
        float snappedZ = Mathf.Round(position.z / gridSize) * gridSize;

        // Set the object's position to the snapped position
        transform.position = new Vector3(snappedX + offset.x, snappedY + offset.y, snappedZ + offset.z);
    }
    #endif
}
