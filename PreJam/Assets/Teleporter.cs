using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private bool isSecond;
    [SerializeField] private GameObject pairTeleporter;
    
    [SerializeField] private MeshRenderer mr;
    [SerializeField] private Material[] mats;

    public bool canSwitch = true;

    private void Start()
    {
        canSwitch = true;
        SetMaterialFromIndex();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<CubeObject>())
        {
            canSwitch = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CubeObject>())
        {
            canSwitch = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isSecond) return;
        if (other.GetComponent<CubeObject>())
        {
            var c = other.GetComponent<CubeObject>();
            c.onMoveFinished += () =>
            {
                c.StopAllCoroutines();
                c.CollisionManaging(false);
                c.transform.DOScale(Vector3.zero, c.GetMoveDelay() / 2f).SetEase(Ease.Linear).OnComplete(
                    () =>
                    {
                        c.transform.DOMove(pairTeleporter.transform.position, 0.1f).OnComplete(() =>
                        {
                            c.transform.DOScale(Vector3.one, c.GetMoveDelay() / 2f).SetEase(Ease.Linear)
                                .OnComplete(() =>
                                {
                                    c.onMoveFinished = null;
                                    c.CollisionManaging(true);
                                    c.StartCoroutine(c.CubeMove());
                                });
                        });
                    });
            };
        }
    }

    IEnumerator DelayedCollision(float delay = 0.1f)
    {
        yield return new WaitForSeconds(delay);
        GetComponent<Collider>().enabled = true;
    }

    public void Switch()
    {
        isSecond = !isSecond;
        SetMaterialFromIndex();
    }

    void SetMaterialFromIndex()
    {
        mr.material = mats[isSecond ? 1 : 0];
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.Label(transform.position, isSecond ? "SECOND" : "FIRST");
        if (pairTeleporter) Handles.DrawLine(transform.position, pairTeleporter.transform.position, 3f);
    }
#endif
}
