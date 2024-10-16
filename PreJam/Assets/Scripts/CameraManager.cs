using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class CameraManager : GenericSingletonClass<CameraManager>
{
    [SerializeField] private float multiplierCamOrthoBase = 1.5f;
    [SerializeField] private float offsetMouseDivider = 150f;
    [SerializeField] private float offsetSpeed = 5f;
    [SerializeField] private Transform mouseOffsetPivot;
    [SerializeField] private Transform psMouse;

    private AudioSource _dragCue;
    
    private float _orthoBase;
    private Vector3 _posBase;
    private Camera cam;

    public override void Awake()
    {
        base.Awake();
        
        cam = GetComponentInChildren<Camera>();
        InitCameraLevel();

        if(_dragCue is null)
            _dragCue = AudioManager.Instance.PlaySoundOut("MouseDragLoop", transform).GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (GameManager.Instance.isWaitingGameToStart && Input.GetMouseButton(1))
        {
            ManageMouseDrag();
            ManageDragSound();
        }
        else 
        {
            ResetMouseDrag(); 
            ResetDragSound();
        }
        
        psMouse.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0)) psMouse.GetComponent<ParticleSystem>().Play();
    }

    private void ManageMouseDrag()
    {
        var pos = Input.mousePosition;
        var center = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        var dir = (pos - center);
            
        mouseOffsetPivot.localPosition = Vector3.Lerp(mouseOffsetPivot.localPosition, dir.normalized * (dir.magnitude / offsetMouseDivider), 
            Time.deltaTime * offsetSpeed);
    }

    private void ResetMouseDrag()
    {
        mouseOffsetPivot.localPosition = Vector3.Lerp(mouseOffsetPivot.localPosition, Vector3.zero, Time.deltaTime * offsetSpeed);
    }

    private Vector3 _lastMousePos;
    private Vector3 _actualMousePosition;
    private void ManageDragSound()
    {
        _lastMousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        //lerp to get some delay with the variable's update
        _actualMousePosition = Vector3.Lerp(_actualMousePosition, _lastMousePos, Time.deltaTime * 0.5f);
        
        if (!_dragCue) return;

        var cue = AudioManager.Instance.GetCueByString("MouseDragLoop");
        var dist = Vector3.Distance(_lastMousePos, _actualMousePosition);
        
        _dragCue.volume = Mathf.Lerp(0f, cue.volume, dist / 15f);
        _dragCue.outputAudioMixerGroup.audioMixer.SetFloat("lowPass", Mathf.Lerp(0, 6000, dist / 5f));
    }

    private void ResetDragSound()
    {
        if (!_dragCue) return;
        _dragCue.volume = Mathf.Lerp(_dragCue.volume, 0f, Time.deltaTime * 3f);
        _dragCue.pitch = Mathf.Lerp(_dragCue.pitch, 1f, Time.deltaTime * 3f);
    }

    private void InitCameraLevel()
    {
        _orthoBase = cam.orthographicSize;
        _posBase =  cam.transform.parent.position;
        
        ResetOrthoSize();
    }

    public void StartLevelCamera()
    {
        cam.DOKill();
        cam.DOOrthoSize(_orthoBase, 1f);
    }
    
    public void CameraShake(float duration = 0.35f, float strength = 0.025f, int vibrato = 100)
    {
        cam.transform.DOKill();
        cam.DOShakePosition(duration, strength, vibrato).SetEase(Ease.OutSine);
    }
    
    public void CenterCameraOnTarget(Transform target, float newOrtho, float duration = 1f)
    {
        // Get the parent transform of the camera
        Transform parentTransform = cam.transform.parent;

        // Keep the parent's current rotation (55, 45, 0), we won't change that

        // Calculate the offset from the target position to the parent's current position
        Vector3 offset = parentTransform.position - cam.transform.position;

        // Set the new position for the parent, which centers on the target
        Vector3 newPosition = target.position + offset;

        parentTransform.DOKill();
        // Move the parent to the new position smoothly
        parentTransform.DOMove(newPosition, duration).SetUpdate(true).SetEase(Ease.InOutSine);

        // Adjust the orthographic size of the camera smoothly
        cam.DOKill();
        cam.DOOrthoSize(newOrtho, duration).SetEase(Ease.InOutSine);
    }

    public void ResetOrthoSize(float duration = 0f)
    {
        if(Vector3.Distance(cam.transform.parent.position, _posBase) > 5f)
            cam.transform.parent.DOMove(_posBase, duration).SetEase(Ease.InOutSine);
        
        cam.DOOrthoSize(_orthoBase * multiplierCamOrthoBase, duration).SetEase(Ease.InOutSine);
    }
}
