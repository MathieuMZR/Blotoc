using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionPad : MonoBehaviour, IInteract
{
    [SerializeField] private bool shouldReveal = false;

    [SerializeField] private Transform switchTransform;
    [SerializeField] private MeshRenderer padColor;
    [SerializeField] private Material[] mats;
    [SerializeField] private MeshRenderer[] switchIcon;

    private void Start()
    {
        SetMaterialFromMode(Convert.ToInt32(shouldReveal));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CubeObject>())
        {
            other.GetComponent<CubeObject>().SetHided(!shouldReveal);
        }
    }

    private void SetMaterialFromMode(int mode)
    {
        padColor.material = mode == 0 ? mats[0] : mats[1];
        switch (mode)
        {
            case 0:
                switchIcon[0].enabled = true;
                switchIcon[1].enabled = true;
                switchIcon[2].enabled = false;
                break;
            
            case 1:
                switchIcon[2].enabled = true;
                switchIcon[0].enabled = false;
                switchIcon[1].enabled = false;
                break;
        }
    }

    public void Interact(int mode)
    {
        shouldReveal = !shouldReveal;
        SetMaterialFromMode(Convert.ToInt32(shouldReveal));
    }
}
