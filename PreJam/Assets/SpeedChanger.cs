using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpeedChanger : MonoBehaviour, ILevelEditorModify
{
    [SerializeField] private float speed = 1;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CubeObject>())
        {
            other.GetComponent<CubeObject>().SetNewSpeedModifier(speed);
        }
    }

    public void ModifyFromLevelEditor()
    {
        transform.DOKill();
        var rot = transform.rotation * Quaternion.Euler(0, 90f, 0);
        transform.DORotateQuaternion(rot, 0.1f).SetEase(Ease.OutSine);
    }
}
