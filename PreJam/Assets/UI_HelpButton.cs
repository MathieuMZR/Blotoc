using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_HelpButton : MonoBehaviour
{
    [SerializeField] private GameObject hint;
    [SerializeField] private Text hintText;
    [SerializeField] private Image blocSprite;

    [SerializeField] private AnimationCurve animCurvePopUp;

    private SO_Bloc _bloc;

    public void SetBloc(SO_Bloc bloc)
    {
        _bloc = bloc;
        
        hintText.text = $"<size=20>{_bloc.name.ToUpper()}</size>\n<color=#ffffff50>{_bloc.description.ToUpper()}</color>";
        blocSprite.sprite = _bloc.sprite;
        
        HideHint(true);
    }
    
    public void DisplayHint()
    {
        hint.transform.DOKill();
        hint.transform.DOScale(Vector3.one, 0.2f).SetEase(animCurvePopUp);
        
        if (FindObjectFromRef(_bloc.prefab.name).Length != 0)
        {
            CameraManager.Instance.CenterCameraOnTarget(FindObjectFromRef(_bloc.prefab.name)[0].transform, 5);
        }

        hint.SetActive(true);
    }

    public void HideHint(bool instant = false)
    {
        if (instant) hint.SetActive(false);
        
        hint.transform.DOKill();
        hint.transform.DOScale(Vector3.zero, 0.2f).SetEase(animCurvePopUp).OnComplete(() => hint.SetActive(false));
        
        CameraManager.Instance.ResetOrthoSize(1f);
    }

    private GameObject[] FindObjectFromRef(string refName, string refSecondary = null)
    {
        // Find all GameObjects in the scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        List<GameObject> foundObjects = new List<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            // Check if the object's name contains an underscore
            int underscoreIndex = obj.name.LastIndexOf('_');

            // Remove the suffix if there's an underscore
            string cleanName = underscoreIndex > -1 ? obj.name.Substring(0, underscoreIndex) : obj.name;
            string newCleanName = cleanName.Replace("(Clone)", "");

            // Compare the cleaned name with the target name
            if (newCleanName == refName)
            {
                foundObjects.Add(obj);
            }
        }

        if (foundObjects.Count == 0)
        {
            foundObjects = FindObjectFromRef(refSecondary).ToList();
        }

        return foundObjects.ToArray();
    }
}
