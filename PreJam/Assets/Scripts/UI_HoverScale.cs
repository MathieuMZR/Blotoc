using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_HoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float scaleMultiplier = 1.5f;
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private AnimationCurve ease;

    private Vector3 _baseScale;

    private void Start()
    {
        _baseScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (DOTween.IsTweening(transform)) transform.DOKill();
        transform.DOScale(_baseScale * scaleMultiplier, duration).SetEase(ease);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (DOTween.IsTweening(transform)) transform.DOKill();
        transform.DOScale(_baseScale, duration).SetEase(ease);
    }
}
