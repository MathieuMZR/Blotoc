using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_HoverColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image sprite;
    
    [SerializeField] private Color newColor;
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private AnimationCurve ease;
    
    private Color _baseColor;

    private void Start()
    {
        _baseColor = sprite.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (DOTween.IsTweening(sprite)) sprite.DOKill();
        sprite.DOColor(newColor, duration).SetEase(ease);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (DOTween.IsTweening(sprite)) sprite.DOKill();
        sprite.DOColor(_baseColor, duration).SetEase(ease);
    }
}
