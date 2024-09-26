using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TitleScreenButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform icon;
    [SerializeField] private Vector2 iconOffset;
    [SerializeField] private float durationAnim;
    [SerializeField] private AnimationCurve animCurve;

    private Vector2 _iconPos;

    private void Start()
    {
        _iconPos = icon.anchoredPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        icon.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        icon.GetComponent<Image>().DOColor(Color.white, durationAnim).SetEase(animCurve);
        
        icon.DOAnchorPos(_iconPos + iconOffset, durationAnim).SetEase(animCurve);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        icon.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), durationAnim).SetEase(animCurve);
        icon.DOAnchorPos(_iconPos, durationAnim).SetEase(animCurve);
    }
}
