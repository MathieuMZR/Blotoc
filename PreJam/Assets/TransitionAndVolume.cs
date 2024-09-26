using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionAndVolume : GenericSingletonClass<TransitionAndVolume>
{
    private Volume globalVolume;

    [SerializeField] private Image transImg;
    [SerializeField] private float duration;
    [SerializeField] private Ease ease;
    
    private readonly string _transition = "_Transition";

    private void Start()
    {
        globalVolume = FindObjectOfType<Volume>();
        
        var mat = Instantiate(transImg.material);
        transImg.material = mat;

        ShowLevel();
        
        if (!GameManager.Instance) return;
        
        GameManager.Instance.onGameEnd += () => StartCoroutine(nameof(RestartLevel));
        GameManager.Instance.onGameWin += () => StartCoroutine(nameof(WinLevel));
        GameManager.Instance.onGameEnd += Desaturate;
        GameManager.Instance.onGameEnd += FilmGrain;
    }

    private void ShowLevel()
    {
        transImg.material.SetFloat(_transition, 1f);
        transImg.material.DOFloat(0f, _transition, duration).SetEase(ease).SetUpdate(true);
    }
    
    private void HideLevel()
    {
        transImg.material.SetFloat(_transition, 1f);
        transImg.material.DOFloat(0f, _transition, duration).SetEase(ease).SetUpdate(true);
    }

    private IEnumerator RestartLevel()
    {
        transImg.material.DOFloat(1f, _transition, duration).SetEase(ease).SetUpdate(true);

        yield return new WaitForSecondsRealtime(duration + 1f);
        
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
    
    public IEnumerator WinLevel()
    {
        SetImagePosition(GameManager.Instance.lastCube.transform.position);

        transImg.material.DOFloat(1f, _transition, duration).SetEase(ease).SetUpdate(true);

        yield return new WaitForSecondsRealtime(duration + 1f);
        
        var scene = SceneManager.GetActiveScene();
        Debug.Log(SceneManager.sceneCount);
        if (scene.buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(scene.buildIndex + 1);
        }
    }
    
    void SetImagePosition(Vector3 worldPos)
    {
        return;
        var mainCamera = GameManager.Instance.cam;
        
        // Convert the world position to screen position in camera space
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

        // Convert the screen position to local position in the Canvas (Screen Space - Camera)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transImg.GetComponentInParent<Canvas>().GetComponent<RectTransform>(),
            screenPos, mainCamera, out Vector2 localPos);

        // Set the UI image's position to this local position
        RectTransform rectTransform = transImg.GetComponent<RectTransform>();
        rectTransform.localPosition = localPos;
    }
    
    public IEnumerator OpenLevel(int index)
    {
        if(HUD.Instance) HUD.Instance.onLevelChange.Invoke();
        
        transImg.material.DOFloat(1f, _transition, duration).SetEase(ease).SetUpdate(true);

        yield return new WaitForSecondsRealtime(duration + 1f);
        
        SceneManager.LoadScene(index);
    }

    public IEnumerator NextLevel()
    {
        HUD.Instance.onLevelChange.Invoke();
        
        transImg.material.DOFloat(1f, _transition, duration).SetEase(ease).SetUpdate(true);

        yield return new WaitForSecondsRealtime(duration + 1f);
        
        var scene = SceneManager.GetActiveScene();
        Debug.Log(SceneManager.sceneCount);
        if (scene.buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(scene.buildIndex + 1);
        }
    }
    
    public IEnumerator PreviousLevel()
    {
        HUD.Instance.onLevelChange.Invoke();
        
        transImg.material.DOFloat(1f, _transition, duration).SetEase(ease).SetUpdate(true);

        yield return new WaitForSecondsRealtime(duration + 1f);
        
        var scene = SceneManager.GetActiveScene();
        if (scene.buildIndex + 1 > 0)
        {
            SceneManager.LoadScene(scene.buildIndex - 1);
        }
    }

    private void Desaturate()
    {
        // Ensure the global volume has a ColorAdjustments effect
        if (globalVolume.profile.TryGet(out ColorAdjustments ca))
        {
            // You can call the tween function here or from another script
            TweenSaturation(-100, 0.75f, ca); // Tween to 1 saturation over 2 seconds
        }
    }
    
    private void FilmGrain()
    {
        // Ensure the global volume has a ColorAdjustments effect
        if (globalVolume.profile.TryGet(out FilmGrain fg))
        {
            // You can call the tween function here or from another script
            TweenFilmGrain(1f, 0.75f, fg); // Tween to 1 saturation over 2 seconds
        }
    }
    
    private void TweenSaturation(float targetSaturation, float duration, ColorAdjustments colorAdjustments)
    {
        DOTween.To(() => colorAdjustments.saturation.value,
                x => colorAdjustments.saturation.value = x,
                targetSaturation,
                duration)
            .SetEase(Ease.InOutQuad).SetUpdate(true); // You can change the easing function as needed
    }
    
    private void TweenFilmGrain(float targetSaturation, float duration, FilmGrain filmGrain)
    {
        DOTween.To(() => filmGrain.intensity.value,
                x => filmGrain.intensity.value = x,
                targetSaturation,
                duration)
            .SetEase(Ease.InOutQuad).SetUpdate(true); // You can change the easing function as needed
    }
}
