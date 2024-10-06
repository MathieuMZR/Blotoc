using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class DirectionChanger : MonoBehaviour, IInteract
{
    public Vector3 direction;
    
    [SerializeField] private bool canMove;
    [SerializeField] private GameObject directionIndicator;
    [SerializeField] private Material mat;
    [SerializeField] private MeshRenderer directionIndicatorPad;
    [SerializeField] private Color32[] directionIndicatorPadColors;
    [SerializeField] private AnimationCurve rotateCurve;

    private void Start()
    {
        directionIndicatorPad.material = Instantiate(mat);

        SetRotation();
        SetColorFromDirection();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CubeObject>())
        {
            other.GetComponent<CubeObject>().SetNewDirection(direction);
        }
    }

    private void SetNewDirection(int mode)
    {
        if (!canMove) return;
        
        var dir = Vector3.zero;
        switch (mode)
        {
            case 0:
                dir = Quaternion.Euler(0, -90, 0) * direction;
                break;
            case 1:
                dir = Quaternion.Euler(0, 90, 0) * direction;
                break;
        }
        direction = dir.Round();
        
        SetRotation();
        SetColorFromDirection();
    }

    void SetRotation(float duration = 0.25f)
    {
        Vector3 normalizedDirection = direction.normalized;
        
        Quaternion targetRotation = Quaternion.LookRotation(normalizedDirection);
        
        directionIndicatorPad.transform.DOKill();
        directionIndicator.transform.DORotateQuaternion(targetRotation, duration).SetEase(rotateCurve);
    }

    void SetColorFromDirection(float duration = 0.25f)
    {
        if (!canMove) return;
        
        var index = 0;
        if (direction == new Vector3(1, 0, 0) || direction == new Vector3(-1, 0, 0))
        {
            index = 0;
        }
        else if (direction == new Vector3(0, 0, 1) || direction == new Vector3(0, 0, -1))
        {
            index = 1;
        }

        if (directionIndicatorPad.material.color == directionIndicatorPadColors[index]) return;
        
        directionIndicatorPad.material.DOKill();
        directionIndicatorPad.material.DOColor(directionIndicatorPadColors[index], duration).SetEase(Ease.InOutSine);
    }
    
    public void Interact(int mode)
    {
        SetNewDirection(mode);

        transform.TryGetComponent(typeof(PadVFX), out var c);
        if(c) (c as PadVFX).ActivatePadVFX();
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.DrawLine(transform.position, transform.position + direction * 2f, 3f);
    }
    #endif
}
