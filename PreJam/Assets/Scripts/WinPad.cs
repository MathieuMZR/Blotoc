using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WinPad : MonoBehaviour
{
    [SerializeField] private AnimationCurve cubeCurve;
    [SerializeField] private float duration = 1f;
    [SerializeField] private GameObject arrowIndicator;

    private void Start()
    {
        GameManager.Instance.onGameStart += DeactivateArrowOnPlay;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CubeObject>())
        {
            GameManager.Instance.winCube++;
            GameManager.Instance.VerifyGame(other.GetComponent<CubeObject>());
            
            var obj = other.GetComponent<CubeObject>();
            obj.CollisionManaging(false);
            obj.StopAllCoroutines();
            
            other.transform.DOScale(Vector3.zero, duration).SetEase(cubeCurve);
        }
    }

    void DeactivateArrowOnPlay()
    {
        arrowIndicator.GetComponentInChildren<ParticleSystem>().transform.DOScale(Vector3.zero, 0.25f);
        arrowIndicator.transform.DOScale(Vector3.zero, 0.25f).OnComplete(() =>
        {
            arrowIndicator.SetActive(false);
        });
    }
}
