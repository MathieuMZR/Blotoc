using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    public void Play()
    {
        StartCoroutine(TransitionAndVolume.Instance.OpenLevel(1));
    }

    public void Quit()
    {
        Application.Quit();
    }
}
