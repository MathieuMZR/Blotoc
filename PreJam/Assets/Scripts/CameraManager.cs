using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraManager : GenericSingletonClass<CameraManager>
{
    [SerializeField] private float multiplierCamOrthoBase = 1.5f;
    [SerializeField] private float offsetMouseDivider = 150f;
    [SerializeField] private float offsetSpeed = 5f;
    [SerializeField] private Transform mouseOffsetPivot;
    [SerializeField] private Transform psMouse;
    
    private float _orthoBase;
    private Camera cam;

    public override void Awake()
    {
        base.Awake();
        
        cam = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        if (GameManager.Instance.isWaitingGameToStart)
        {
            var pos = Input.mousePosition;
            var center = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
            var dir = (pos - center);
            
            mouseOffsetPivot.localPosition = Vector3.Lerp(mouseOffsetPivot.localPosition, dir.normalized * (dir.magnitude / offsetMouseDivider), 
                Time.deltaTime * offsetSpeed);
        }
        else 
            mouseOffsetPivot.localPosition = Vector3.Lerp(mouseOffsetPivot.localPosition, Vector3.zero, Time.deltaTime * offsetSpeed);

        psMouse.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0)) psMouse.GetComponent<ParticleSystem>().Play();
    }

    public void InitCameraLevel()
    {
        _orthoBase = cam.orthographicSize;
        cam.orthographicSize = _orthoBase * multiplierCamOrthoBase;
    }

    public void StartLevelCamera()
    {
        cam.DOOrthoSize(_orthoBase, 1f);
    }

    public void CameraShake(float duration = 0.35f, float strength = 0.025f, int vibrato = 100)
    {
        cam.transform.DOKill();
        cam.DOShakePosition(duration, strength, vibrato).SetEase(Ease.OutSine);
    }
    
    public void CenterOnImpactDeath(Transform target, float orthoDivider, float duration = 1f)
    {
        // Get the parent transform of the camera
        Transform parentTransform = cam.transform.parent;

        // Keep the parent's current rotation (55, 45, 0), we won't change that

        // Calculate the offset from the target position to the parent's current position
        Vector3 offset = parentTransform.position - cam.transform.position;

        // Set the new position for the parent, which centers on the target
        Vector3 newPosition = target.position + offset;

        // Move the parent to the new position smoothly
        parentTransform.DOMove(newPosition, duration).SetUpdate(true).SetEase(Ease.OutSine);

        // Adjust the orthographic size of the camera smoothly
        float currentOrtho = cam.orthographicSize;
        cam.DOOrthoSize(currentOrtho / orthoDivider, duration).SetEase(Ease.OutSine);
    }
}
