using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEditorLevelItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float scaleMultiplierImage;
    [SerializeField] private float scaleMultiplierButton;
    [SerializeField] private float animSpeed;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;

    private Color _color;
    private Color _defaultColor;

    private void Start()
    {
        _defaultColor = new Color(1, 1, 1, 1);
        GetComponent<Button>().image.color = _defaultColor;
    }

    public void SetImage(Sprite sprite) => image.sprite = sprite;
    public void SetText(string s) => text.text = s;

    public void SetColor(Color c) => _color = c;

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.transform.DOKill();
        image.transform.DOScale(Vector3.one * scaleMultiplierImage, animSpeed).SetEase(curve);
        
        transform.DOKill();
        transform.DOScale(Vector3.one * scaleMultiplierButton, animSpeed).SetEase(curve);

        GetComponent<Button>().DOKill();
        GetComponent<Button>().image.DOColor(_color, animSpeed).SetEase(Ease.OutSine);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.transform.DOKill();
        image.transform.DOScale(Vector3.one, animSpeed).SetEase(curve);
        
        transform.DOKill();
        transform.DOScale(Vector3.one, animSpeed).SetEase(curve);
        
        GetComponent<Button>().DOKill();
        GetComponent<Button>().image.DOColor(_defaultColor, animSpeed).SetEase(Ease.OutSine);
    }
}
