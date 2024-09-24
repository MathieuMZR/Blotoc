using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpeedEffect : MonoBehaviour
{
    [SerializeField] private GameObject mr;
    [SerializeField] private Material mat;
    [SerializeField] private float delayBetweenSpawn = 0.5f;
    [SerializeField] private float dispawnDuration = 0.25f;
    [SerializeField] private float delayBeforeDispawn = 0.5f;
    [SerializeField] private AnimationCurve dispawnCurve;

    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(nameof(SpeedEffectRoutine));
    }

    IEnumerator SpeedEffectRoutine()
    {
        var cube = gameObject.GetComponent<CubeObject>();
        
        yield return new WaitForSeconds(cube.GetMoveDelay());
        
        var obj = Instantiate(mr.gameObject, mr.transform.position, mr.transform.rotation);
        
        obj.transform.DOScale(Vector3.zero, dispawnDuration).SetEase(dispawnCurve).SetDelay(delayBeforeDispawn);
        obj.GetComponentInChildren<MeshRenderer>().material = mat;
        obj.transform.localScale = Vector3.one / 1.1f;
        
        StartCoroutine(nameof(SpeedEffectRoutine));
    }
}
