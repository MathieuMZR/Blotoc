using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_HoverSound : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private string soundName;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySound(soundName);
    }
}
