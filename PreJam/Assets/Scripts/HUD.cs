using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HUD : GenericSingletonClass<HUD>
{
    [SerializeField] private CanvasGroup fadeBeforePlay;
    [SerializeField] private Animator animator;
    
    [SerializeField] private Text levelNameNumber;
    [SerializeField] private Text levelNameText;
    [SerializeField] private Text cubeAmount;
    [SerializeField] private Button levelNext;
    [SerializeField] private Button levelPrevious;
    
    [SerializeField] private Transform difficultyContainer;
    [SerializeField] private GameObject difficultyStar;

    [SerializeField] private GameObject[] disableOnEditor;

    public Action onLevelChange;
    
    private void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        
        SetLevelInfos();
        SetDifficultyStars();
        
        GameManager.Instance.onGameStart += FadeOutOnPlay;
        GameManager.Instance.onGameStart += () => animator.Play("HUDPlay");
        
        GameManager.Instance.onGameEnd += () => animator.Play("HUDHide");
        
        GameManager.Instance.onGameWin += () => animator.Play("HUDHide");
        
        onLevelChange += () => animator.CrossFade("HUDHide", 1f);

        levelNext.onClick.AddListener(()=>
        {
            StartCoroutine(TransitionAndVolume.Instance.NextLevel());
        });
        levelPrevious.onClick.AddListener(()=>
        {
            StartCoroutine(TransitionAndVolume.Instance.PreviousLevel());
        });
        
        WaitForLevelStart();
    }
    
    private void WaitForLevelStart()
    {
        fadeBeforePlay.enabled = true;
        AnimateFadeBeforePlay();
    }

    private void AnimateFadeBeforePlay()
    {
        fadeBeforePlay.alpha = 0f;
        fadeBeforePlay.DOFade(1, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).SetUpdate(true);
    }

    private void FadeOutOnPlay()
    {
        fadeBeforePlay.DOKill();
        fadeBeforePlay.DOFade(0f, 0.5f).SetUpdate(true);
    }

    public void SetLevelInfos()
    {
        levelNameNumber.text = ("Level " + (SceneManager.GetActiveScene().buildIndex)).ToUpper();
        levelNameText.text = GameManager.Instance.level.name.ToUpper();
        
        int amountOfCube = FindObjectsOfType<CubeSpawner>().Length;
        cubeAmount.text = $"{GameManager.Instance.winCube.ToString().ToUpper()} / {amountOfCube.ToString().ToUpper()}";
    }

    private void SetDifficultyStars()
    {
        StartCoroutine(SetDifficultyStarRoutine());
    }

    private IEnumerator SetDifficultyStarRoutine()
    {
        for (int i = 0; i < GameManager.Instance.level.difficulty; i++)
        {
            var obj = Instantiate(difficultyStar, transform.position, Quaternion.identity, difficultyContainer);
            obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void DisableLevelButton(int index, bool disable)
    {
        if (index == 0) levelPrevious.gameObject.SetActive(!disable);
        else levelNext.gameObject.SetActive(!disable);
    }
}
