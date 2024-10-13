using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : GenericSingletonClass<GameManager>
{
    public LevelInfo level;
    public int tileSize = 10;
    
    [SerializeField] private AnimationCurve winLevelCurve;

    public int winCube;
    public CubeObject lastCube;
    
    public Action onGameStart;
    public Action onGameEnd;
    public Action onGameWin;

    public bool isWaitingGameToStart = true;

    private void Start()
    {
        InitGame();
    }

    void InitGame()
    {
        Time.timeScale = 1f;

        VerifyLevelIndex();
    }

    private void StartGame()
    {
        CameraManager.Instance.StartLevelCamera();
        
        onGameStart.Invoke();
        
        Debug.Log("Game Sarted");
    }
    
    public void EndGame()
    {
        onGameEnd.Invoke();

        DOTween.To(()=> Time.timeScale, x => Time.timeScale = x, 0f, 1f).SetUpdate(true);
    }

    public void VerifyGame(CubeObject cube)
    {
        int amountOfCube = FindObjectsOfType<CubeSpawner>().Length;
        HUD.Instance.SetLevelInfos();
        
        AudioManager.Instance?.PlaySoundWithPitch("CubeFinish", null, 1f + (winCube / 2f));
        
        if (winCube == amountOfCube)
        {
            lastCube = cube;
            WinGame();
        }
    }

    private void VerifyLevelIndex()
    {
        HUD.Instance.DisableLevelButton(0, SceneManager.GetActiveScene().buildIndex == 0);
        HUD.Instance.DisableLevelButton(1, SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings);
    }

    private void WinGame()
    {
        CameraManager.Instance.CenterCameraOnTarget(lastCube.transform, 4f);
        
        onGameWin.Invoke();
        
        AudioManager.Instance?.PlaySound("LevelWin");
    }

    void Update()
    {
        if (isWaitingGameToStart)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                isWaitingGameToStart = false;
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
        CameraManager.Instance.CameraShake(0.35f, 0.025f, 100);
        
        AudioManager.Instance?.PlaySound("Switch");

        var list = Helper.Find<IInteract>();

        int index = 0;
        foreach (var i in list.Item1)
        {
            if (!list.Item2[index]) return;
            
            if (list.Item2[index].GetComponent<Teleporter>())
            {
                if (!list.Item2[index].GetComponent<Teleporter>().canSwitch) continue;
                i.Interact((int)mode);
                continue;
            }
            
            i.Interact((int)mode);
            index++;
        }
    }
    
    public enum RotateMode
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

    public List<SO_Bloc> blocAvailables = new List<SO_Bloc>();
}