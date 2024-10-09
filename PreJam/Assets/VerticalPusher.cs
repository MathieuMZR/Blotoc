using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class VerticalPusher : MonoBehaviour, IInteract
{
    [SerializeField] private Vector3 offset;
    
    [SerializeField] private float duration;
    [SerializeField] private AnimationCurve curve;
    
    [SerializeField] private Transform pivot;

    private CubeObject _cube;
    private bool _isUp;
    
    public void Interact(int mode)
    {
        transform.DOKill();
        var pos = transform.position;
        transform.DOMove(_isUp ? pos + offset : pos - offset, duration).SetEase(curve);
        
        _isUp = !_isUp;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _cube = other.GetComponent<CubeObject>();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _cube = null;
        }
    }
}
