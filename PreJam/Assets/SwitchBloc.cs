using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class SwitchBloc : MonoBehaviour
{
    [SerializeField] private bool startSwitch;
    
    [SerializeField] private AnimationCurve switchCurveAnimX;
    [SerializeField] private AnimationCurve switchCurveAnimY;
    [SerializeField] private AnimationCurve switchCurveAnimZ;
    [SerializeField] private float animDuration = 0.35f;
    
    [SerializeField] private MeshRenderer mr;
    [SerializeField] private ParticleSystem rotate;

    private bool _switchState;
    private Collider _c;

    private void Start()
    {
        var mat = mr.material;
        mr.material = Instantiate(mat);
        
        _c = GetComponent<Collider>();

        _switchState = startSwitch;
        _c.enabled = _switchState;
        
        mr.material.SetFloat("_Switch", _switchState ? 1 : 0);
    }

    public void Switch()
    {
        _switchState = !_switchState;
        _c.enabled = _switchState;
        
        mr.material.SetFloat("_Switch", _switchState ? 1 : 0);

        transform.DOKill();
        transform.localScale = Vector3.one;
        transform.DOScaleX(0f, animDuration).SetEase(switchCurveAnimX);
        transform.DOScaleY(0f, animDuration).SetEase(switchCurveAnimY);
        transform.DOScaleZ(0f, animDuration).SetEase(switchCurveAnimZ);
        
        rotate.Stop();
        rotate.Play();
    }
}
