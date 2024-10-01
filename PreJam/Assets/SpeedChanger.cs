using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpeedChanger : MonoBehaviour
{
    [SerializeField] private float speed = 1;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CubeObject>())
        {
            other.GetComponent<CubeObject>().SetNewSpeedModifier(speed);
        }
    }
}
