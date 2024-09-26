using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : GenericSingletonClass<GameManager>
{
    public LevelInfo level;
    public int tileSize = 10;

    public Camera cam;
    [SerializeField] private AnimationCurve winLevelCurve;

    public int winCube;
    public CubeObject lastCube;
    
    public Action onGameStart;
    public Action onGameEnd;
    public Action onGameWin;

    private bool _isWaitingGameToStart = true;

    private void Start()
    {
        Time.timeScale = 1f;
        cam = Camera.main;
        
        var ortho = cam.orthographicSize;
        cam.orthographicSize = ortho * 1.5f;

        VerifyLevelIndex();
        
        foreach (SwitchBloc sw in FindObjectsOfType<SwitchBloc>())
        {
            sw.EnableColliderEditor(false);
        }
    }

    private void StartGame()
    {
        var ortho = cam.orthographicSize;
        cam.DOOrthoSize(ortho / 1.5f, 1f);
        
        onGameStart.Invoke();
    }
    
    public void EndGame()
    {
        onGameEnd.Invoke();

        DOTween.To(()=> Time.timeScale, x => Time.timeScale = x, 0f, 1f).SetUpdate(true);
        
        foreach (SwitchBloc sw in FindObjectsOfType<SwitchBloc>())
        {
            sw.EnableColliderEditor(true);
        }
    }

    public void VerifyGame(CubeObject cube)
    {
        int amountOfCube = FindObjectsOfType<CubeSpawner>().Length;
        HUD.Instance.SetLevelInfos();
        
        if (winCube == amountOfCube)
        {
            lastCube = cube;
            WinGame();
        }
    }

    private void VerifyLevelIndex()
    {
        HUD.Instance.DisableLevelButton(0, SceneManager.GetActiveScene().buildIndex == 0);
        HUD.Instance.DisableLevelButton(1, SceneManager.GetActiveScene().buildIndex == 10);
    }

    private void WinGame()
    {
        onGameWin.Invoke();

        CenterOnImpactDeath(lastCube.transform, 1.5f);
    }

    void Update()
    {
        if (_isWaitingGameToStart)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _isWaitingGameToStart = false;
                if (IsInEditor())
                {
                    LevelEditor.Instance.SetEditorState(false);
                    foreach (SwitchBloc sw in FindObjectsOfType<SwitchBloc>())
                    {
                        sw.EnableColliderEditor(false);
                    }
                }
                StartGame();
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            InputAction(RotateMode.ClockWise);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            InputAction(RotateMode.CounterClockWise);
        }
    }

    void InputAction(RotateMode mode)
    {
        if (IsInEditor()) return;
        
        cam.transform.DOKill();
        cam.DOShakePosition(.35f, .025f, 100).SetEase(Ease.OutSine);
        
        foreach (var v in FindObjectsOfType<DirectionChanger>())
        {
            v.SetNewDirection((int)mode);
        }
        foreach (var v in FindObjectsOfType<SwitchBloc>())
        {
            v.Switch();
        }
        foreach (var t in FindObjectsOfType<Teleporter>())
        {
            if(t.canSwitch) t.Switch();
        }
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

    private bool IsInEditor() => LevelEditor.Instance && LevelEditor.Instance.isInEditor;
    
    private enum RotateMode
    {
        ClockWise,
        CounterClockWise
    }
}

[Serializable]
public class LevelInfo
{
    public string name;
    public int difficulty = 1;
}