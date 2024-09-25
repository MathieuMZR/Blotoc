using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelEditor : GenericSingletonClass<LevelEditor>
{
    public bool isInEditor;
    public int currentSelectedObject;
    
    [SerializeField] private LevelEditorObject[] availableObjects;
    [SerializeField] private Transform objectsGroupCanvas;

    [SerializeField] private AnimationCurve placeCurve;
    [SerializeField] private AnimationCurve deleteCurve;
    
    private Button[] _objectButtons;
    private GameObject _objectPreview;
    
    void Start()
    {
        SetEditorState(true);
            
        SetupAllObjectButtonsInEditor();
    }
    
    private void Update()
    {
        ManageObjectPreviewPosition();
        VerifyPlacement();
    }

    public void SetEditorState(bool state)
    {
        currentSelectedObject = -1;
        isInEditor = state;
    }
    
    void SetupAllObjectButtonsInEditor()
    {
        _objectButtons = objectsGroupCanvas.GetComponentsInChildren<Button>();
        
        foreach (var b in _objectButtons)
        {
            var index = b.transform.GetSiblingIndex();
            b.onClick.AddListener(() => SelectObject(index));
            
            //Temporary
            string[] sArray = SplitString(GetCurrentObjectIndex(index).obj.name);
            string finalString = "";
            foreach (var s in sArray)
            {
                finalString += s + " ";
            }
            
            b.GetComponentInChildren<TextMeshProUGUI>().text = finalString;
        }
    }

    void SelectObject(int index)
    {
        currentSelectedObject = index;
        CreateObjectPreview();
    }
    
    public void Unselect()
    {
        currentSelectedObject = -1;
        DestroyObjectPreview();
    }
    
    void CreateObjectPreview()
    {
        DestroyObjectPreview();
        if (_objectPreview == GetCurrentSelectedObject().obj) return;
        if (GetCurrentSelectedObject() is null) return;
        
        _objectPreview = Instantiate(GetCurrentSelectedObject().obj, GetWorldMousePosition().Item1, Quaternion.identity);
    }

    void DestroyObjectPreview()
    {
        if (_objectPreview)
        {
            Destroy(_objectPreview);
            _objectPreview = null;
        }
    }

    void ManageObjectPreviewPosition()
    {
        if (_objectPreview is not null)
        {
            _objectPreview.transform.position = Vector3.Lerp(_objectPreview.transform.position, 
                SnapToGrid(GetWorldMousePosition().Item1, GetCurrentSelectedObject().offsetInWorld, 1f),
                Time.deltaTime * 50f);
        }
    }

    void VerifyPlacement()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            var col = GetWorldMousePosition().Item2.collider;
            if (col.gameObject != _objectPreview && col.CompareTag("PlacedObjects"))
            {
                col.TryGetComponent(out ILevelEditorModify lvl);
                if (lvl is not null && _objectPreview == null)
                {
                    lvl.ModifyFromLevelEditor();
                }
                return;
            }
            
            if (GetCurrentObjectIndex(currentSelectedObject) is null) return;
            if (GetCurrentSelectedObject() is null) return;
            
            var obj = Instantiate(GetCurrentObjectIndex(currentSelectedObject).obj,
                SnapToGrid(GetWorldMousePosition().Item1, GetCurrentSelectedObject().offsetInWorld, 1f),
                Quaternion.identity);
            obj.transform.localScale = Vector3.zero;
            obj.transform.DOScale(Vector3.one, 0.2f).SetEase(placeCurve);
        }

        if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject() && _objectPreview == null)
        {
            var col = GetWorldMousePosition().Item2.collider;
            if (col.CompareTag("PlacedObjects"))
            {
                col.transform.DOScale(Vector3.one, 0.2f).SetEase(deleteCurve).OnComplete(() =>
                {
                    Destroy(col.gameObject);
                });
            }
        }
    }

    //--------------------------- UTILITIES --------------------------------

    LevelEditorObject GetCurrentSelectedObject()
    {
        if (currentSelectedObject < 0) return null;
        return availableObjects[currentSelectedObject];
    }
    
    LevelEditorObject GetCurrentObjectIndex(int index)
    {
        if (index < 0) return null;
        return availableObjects[index];
    }

    (Vector3, RaycastHit) GetWorldMousePosition()
    {
        Vector3 outValue = Vector3.zero;
        Vector3 mousePos = Input.mousePosition;
        Ray ray = GameManager.Instance.cam.ScreenPointToRay(mousePos);

        // Perform the raycast
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            outValue = hit.point;
        }

        return (new Vector3(outValue.x, 0, outValue.z), hit);
    }

    private Vector3 SnapToGrid(Vector3 pos, Vector3 offset, float gridSize)
    {
        Vector3 position = pos;

        // Calculate the snapped position
        float snappedX = Mathf.Round(position.x / gridSize) * gridSize;
        float snappedY = Mathf.Round(position.y / gridSize) * gridSize;
        float snappedZ = Mathf.Round(position.z / gridSize) * gridSize;

        // Set the object's position to the snapped position
        return new Vector3(snappedX + offset.x, snappedY + offset.y, snappedZ + offset.z);
    }

    string[] SplitString(string inputString)
    {
        List<string> words = new List<string>();
        string currentWord = "";

        foreach (char c in inputString)
        {
            if (char.IsUpper(c))
            {
                if (currentWord != "")
                {
                    words.Add(currentWord);
                }
                currentWord = c.ToString();
            }
            else
            {
                currentWord += c;
            }
        }

        if (currentWord != "")
        {
            words.Add(currentWord);
        }

        return words.ToArray();
    }
}

[Serializable]
public class LevelEditorObject
{
    public GameObject obj;
    public Vector3 offsetInWorld;
}
