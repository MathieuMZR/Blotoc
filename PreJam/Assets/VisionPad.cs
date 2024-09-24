using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionPad : MonoBehaviour
{
    [SerializeField] private bool shouldReveal = false;
    private Coroutine _blinkRoutine;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CubeObject>())
        {
            BlinkingAnim(other.GetComponent<CubeObject>());
        }
    }

    void BlinkingAnim(CubeObject other)
    {
        other.isInvisible = !shouldReveal;
        
        if(_blinkRoutine is not null) StopCoroutine(_blinkRoutine);
        _blinkRoutine = StartCoroutine(other.InvisibleBlink());
    }
}
