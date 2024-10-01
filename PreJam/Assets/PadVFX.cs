using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PadVFX : MonoBehaviour
{
     public ParticleSystem switchPs;

     public void ActivatePadVFX(float delay = 0f)
     {
          if (!switchPs) return;
          
          StopAllCoroutines();
          StartCoroutine(ActivateRoutine(delay));
     }

     private IEnumerator ActivateRoutine(float delay = 0f)
     {
          yield return new WaitForSeconds(delay);
          
          switchPs.Stop();
          switchPs.Play();
     }
}
