using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LevelEditor : GenericSingletonClass<LevelEditor>
{
    public bool isInEditor;
    public int currentSelectedObject;
    
    [SerializeField] private LevelEditorObject[] availableObjects;
    [SerializeField] private RectTransform objectsGroup;
    [SerializeField] private RectTransform objectsGroupParent;

    [SerializeField] private float placeDeleteAnimDuration = 0.25f;
    [SerializeField] private AnimationCurve placeCurve;
    [SerializeField] private AnimationCurve deleteCurve;

    [SerializeField] private Vector2 offsetGroupHide;

    private Vector2 _basePosGroup;
    private bool _isGroupVisible = true;

    private bool[,] grid;
    
    private Button[] _objectButtons;
    private GameObject _objectPreview;
    
    void Start()
    {
        SetupGrid();
        SetEditorState(true);
            
        SetupAllObjectButtonsInEditor();

        _basePosGroup = objectsGroupParent.anchoredPosition;
        ShowHideGroup();
    }
    
    private void Update()
    {
        ManageObjectPreviewVisibility();
        ManageObjectPreviewPosition();
        VerifyPlacement();
        
        if(Input.GetKeyDown(KeyCode.X)) Unselect();
    }

    private void SetupGrid()
    {
        var sizeX = 50;
        var sizeY = 50;
        int beginX = (int)(-sizeX / 2f);
        int beginY = (int)(-sizeY / 2f);
        
        grid = new bool[sizeX, sizeY];
    }

    private void SetGridValue(bool value, int posX, int posY)
    {
        grid[posX, posY] = value;
    }

    public void SetEditorState(bool state)
    {
        currentSelectedObject = -1;
        isInEditor = state;
    }
    
    void SetupAllObjectButtonsInEditor()
    {
        _objectButtons = objectsGroup.GetComponentsInChildren<Button>();
        
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
            b.transform.GetChild(1).GetComponent<Image>().sprite = GetCurrentObjectIndex(index).sprite;
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
        var cols = _objectPreview.GetComponents<Collider>();
        foreach (var c in cols) c.enabled = false;
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

    void ManageObjectPreviewVisibility()
    {
        bool condition = EventSystem.current.IsPointerOverGameObject();
        if(_objectPreview) _objectPreview.SetActive(!condition);
    }

    void VerifyPlacement()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            var col = GetWorldMousePosition().Item2.collider;
            var point = GetWorldMousePosition().Item2.point;

            //Modify object when no item selected
            if (currentSelectedObject == -1 && GetGridValue(point))
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
            if (GetGridValue(point)) return;
            
            var pos = SnapToGrid(GetWorldMousePosition().Item1, GetCurrentSelectedObject().offsetInWorld, 1f);
            var obj = Instantiate(GetCurrentObjectIndex(currentSelectedObject).obj, pos, Quaternion.identity);
            obj.name = "PLACED";
            
            SetGridValue(true, (int)pos.x, (int)pos.z);
            
            PlaceObjectAnimation(obj);
        }

        if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject() && _objectPreview == null)
        {
            var col = GetWorldMousePosition().Item2.collider;
            var point = GetWorldMousePosition().Item2.point;

            if (col.name == "Plane") return;
            
            if (GetGridValue(point))
            {
                col.transform.DOScale(Vector3.zero, placeDeleteAnimDuration / 2f).SetEase(deleteCurve).OnComplete(() =>
                {
                    Destroy(col.gameObject);
                });
            }
        }
    }

    void PlaceObjectAnimation(GameObject obj)
    {
        return;
        var pos = obj.transform.position;
        obj.transform.position = pos + Vector3.up * 20f;
        obj.transform.DOMove(pos, placeDeleteAnimDuration).SetEase(placeCurve);
    }

    public void SetGroupVisibility()
    {
        _isGroupVisible = !_isGroupVisible;
        ShowHideGroup();
    }
    
    void ShowHideGroup()
    {
        objectsGroupParent.DOKill();
        var offset = _basePosGroup + (!_isGroupVisible ? offsetGroupHide : Vector2.zero);
        objectsGroupParent.DOAnchorPos(offset, 0.25f).SetEase(Ease.OutSine);
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

    (Vector3, RaycastHit, Vector2Int) GetWorldMousePosition()
    {
        Vector3 outValue = Vector3.zero;
        Vector3 mousePos = Input.mousePosition;
        Ray ray = GameManager.Instance.cam.ScreenPointToRay(mousePos);

        // Perform the raycast
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            outValue = hit.point;
        }

        var v = new Vector2Int((int)(outValue.x),(int)(outValue.z));
        return (new Vector3(outValue.x, 0, outValue.z), hit, v);
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

    bool GetGridValue(Vector3 point)
    {
        var pos = SnapToGrid(point, Vector3.zero, 1f);
        return grid[(int)pos.x, (int)pos.z];
    }
}

[Serializable]
public class LevelEditorObject
{
    public GameObject obj;
    public Vector3 offsetInWorld;
    public Sprite sprite;
}
