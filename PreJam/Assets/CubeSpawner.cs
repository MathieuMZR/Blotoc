using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class CubeSpawner : MonoBehaviour, ILevelEditorModify
{
    public Vector3 direction;
    [SerializeField] private CubeObject cube;

    private void Start()
    {
        GameManager.Instance.onGameStart += SpawnCube;
    }

    private void OnDestroy()
    {
        GameManager.Instance.onGameStart -= SpawnCube;
    }

    private void SpawnCube()
    {
        var c = Instantiate(cube, transform.position + new Vector3(0, -1, 0), Quaternion.identity);
        c.SetNewDirection(direction);
    }
    
    public void ModifyFromLevelEditor()
    {
        Vector3 v = Quaternion.AngleAxis(90f, Vector3.up) * direction;
        direction = v;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.DrawLine(transform.position, transform.position + direction * 2f, 3f);
    }
    #endif
}
