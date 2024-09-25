using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEditorLevelItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private float scaleMultiplierImage;
    [SerializeField] private float scaleMultiplierButton;
    [SerializeField] private float animSpeed;
    [SerializeField] private Vector2 offset;
    
    [SerializeField] private AnimationCurve curve;
    
    [SerializeField] private Image image;
    [SerializeField] private Image imageOpacity;
    [SerializeField] private Image bandeau;
    
    [SerializeField] private float color;
    
    [SerializeField] private TextMeshProUGUI text;

    public bool isSelected;

    private Color _color;
    private Color _defaultColor;
    private Vector2 _basePos;

    private void Start()
    {
        _basePos = GetComponent<RectTransform>().anchoredPosition;
        GetComponent<RectTransform>().anchoredPosition += offset;
    }

    public void SetImage(Sprite sprite)
    { 
        image.sprite = sprite;
        imageOpacity.sprite = sprite;
    }
    
    public void SetText(string s) => text.text = s;

    public void SetColor(Color c)
    {
        _defaultColor = Helper.Saturation(c, color);
        GetComponent<Button>().image.color = _defaultColor;
        
        _color = c;

        var transformLocalScale = bandeau.transform.localScale;
        transformLocalScale.x = 0;
        bandeau.transform.localScale = transformLocalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.transform.DOKill();
        image.transform.DOScale(Vector3.one * scaleMultiplierImage, animSpeed).SetEase(curve);
        
        imageOpacity.transform.DOKill();
        imageOpacity.transform.DOScale(Vector3.one * scaleMultiplierImage * 2f, animSpeed).SetEase(curve);
        
        transform.DOKill();
        transform.DOScale(Vector3.one * scaleMultiplierButton, animSpeed).SetEase(curve);

        GetComponent<Button>().image.DOKill();
        GetComponent<Button>().image.DOColor(_color, animSpeed).SetEase(Ease.OutSine);

        bandeau.DOKill();
        bandeau.DOColor(Color.white, animSpeed).SetEase(Ease.OutSine);
        bandeau.transform.DOScaleX(1f, animSpeed).SetEase(curve);

        GetComponent<RectTransform>().DOKill();
        GetComponent<RectTransform>().DOAnchorPos(_basePos, animSpeed).SetEase(curve);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelected) return;
        Deselect();
    }

    public void Deselect()
    {
        image.transform.DOKill();
        image.transform.DOScale(Vector3.one, animSpeed).SetEase(curve);
        
        imageOpacity.transform.DOKill();
        imageOpacity.transform.DOScale(Vector3.one, animSpeed).SetEase(curve);
        
        transform.DOKill();
        transform.DOScale(Vector3.one, animSpeed).SetEase(curve);
        
        GetComponent<Button>().image.DOKill();
        GetComponent<Button>().image.DOColor(_defaultColor, animSpeed / 2f).SetEase(Ease.OutSine);
        
        bandeau.DOKill();
        bandeau.DOColor(_defaultColor * new Color(1,1,1,0), animSpeed).SetEase(Ease.OutSine);
        bandeau.transform.DOScaleX(0f, animSpeed).SetEase(Ease.OutExpo);
        
        GetComponent<RectTransform>().DOKill();
        GetComponent<RectTransform>().DOAnchorPos(_basePos + offset, animSpeed).SetEase(curve);

        isSelected = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(Vector3.one / 1.25f, 0.075f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}
