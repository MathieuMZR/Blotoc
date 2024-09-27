using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private GameObject psAddObject;

    [SerializeField] private float placeDeleteAnimDuration = 0.25f;
    [SerializeField] private AnimationCurve placeCurve;
    [SerializeField] private AnimationCurve deleteCurve;
    [SerializeField] private AnimationCurve groupCurve;

    [SerializeField] private Vector2 offsetGroupHide;
    [SerializeField] private GameObject[] go;
    [SerializeField] private Transform p;

    private Vector2 _basePosGroup;
    private bool _isGroupVisible = true;

    private bool[,] grid;
    
    private Button[] _objectButtons;
    private GameObject _objectPreview;
    private Button _lastSelectedButton;
    
    void Start()
    {
        SetupGrid();
        SetEditorState(true);
            
        SetupAllObjectButtonsInEditor();

        _basePosGroup = objectsGroupParent.anchoredPosition;
        ShowHideGroup();

        HUD.Instance.onLevelChange += SetGroupHidedTotally;

        GridDebug();
    }
    
    private void Update()
    {
        ManageObjectPreviewVisibility();
        ManageObjectPreviewPosition();
        VerifyPlacement();
        
        if(Input.GetKeyDown(KeyCode.X)) Unselect();
    }
    
    private void GridDebug()
    {
        return;
        
        foreach (Transform t in p)
        {
            if(t != p) Destroy(t.gameObject);
        }

        for (int x = 0; x < 50; x++)
        {
            for (int y = 0; y < 50; y++)
            {
                Instantiate(GetGridValue(new Vector3(x, 0, y)) ? go[0] : go[1], new Vector3(x, 0, y), Quaternion.identity, p);
            }
        }
    }

    private void SetupGrid()
    {
        var sizeX = 50;
        var sizeY = 50;
        grid = new bool[sizeX, sizeY];
    }

    private void RefreshGridValue(bool value, int posX, int posY)
    {
        grid[posX, posY] = value;
        
        GridDebug();
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
            b.onClick.AddListener(() => SelectObject(index, b));
            
            //Temporary
            string[] sArray = SplitString(GetCurrentObjectIndex(index).obj.name);
            string finalString = "";
            foreach (var s in sArray)
            {
                finalString += s + " ";
            }

            var c = b.GetComponent<UIEditorLevelItem>();
            c.SetImage(GetCurrentObjectIndex(index).sprite);
            c.SetText(finalString);
            c.SetColor(GetCurrentObjectIndex(index).colorUI);
        }
    }

    void SelectObject(int index, Button b)
    {
        currentSelectedObject = index;
        CreateObjectPreview();

        _lastSelectedButton = b;
        
        foreach (var ui in FindObjectsOfType<UIEditorLevelItem>())
        {
            if (ui == b.GetComponent<UIEditorLevelItem>()) continue;
            ui.Deselect();
        }

        b.GetComponent<UIEditorLevelItem>().isSelected = true;
    }
    
    public void Unselect()
    {
        currentSelectedObject = -1;
        DestroyObjectPreview();

        _lastSelectedButton.GetComponent<UIEditorLevelItem>().isSelected = false;
        _lastSelectedButton.GetComponent<UIEditorLevelItem>().Deselect();
        
        _lastSelectedButton = null;
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
                GetWorldMousePosition().Item1, Time.deltaTime * 50f);
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
            var point = GetWorldMousePosition().Item1;

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

            var pos = GetWorldMousePosition().Item1;
            var obj = Instantiate(GetCurrentObjectIndex(currentSelectedObject).obj, pos, Quaternion.identity);
            obj.name = "PLACED";

            Instantiate(psAddObject, pos, Quaternion.identity);
            
            RefreshGridValue(true, (int)pos.x, (int)pos.z);
            
            PlaceObjectAnimation(obj);
        }

        if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
        {
            var col = GetWorldMousePosition().Item2.collider;
            var point = GetWorldMousePosition().Item1;

            if (col.gameObject.layer == LayerMask.NameToLayer("Ground")) return;

            if (GetGridValue(point))
            {
                Ray ray = GameManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
                Debug.DrawRay(ray.origin, ray.direction * 500f, Color.red, 100f);
                
                RefreshGridValue(false, (int)point.x, (int)point.z);
                col.transform.DOScale(new Vector3(0,1,0), placeDeleteAnimDuration / 2f).SetEase(deleteCurve).OnComplete(() =>
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
    
    public void SetGroupVisibilityManual(bool visibility)
    {
        _isGroupVisible = visibility;
        ShowHideGroup();
    }
    
    public void SetGroupHidedTotally()
    {
        objectsGroupParent.DOKill();
        var offset = _basePosGroup + new Vector2(0, offsetGroupHide.y * 2f);
        objectsGroupParent.DOAnchorPos(offset, 0.25f).SetEase(groupCurve);
    }
    
    void ShowHideGroup()
    {
        objectsGroupParent.DOKill();
        var offset = _basePosGroup + (!_isGroupVisible ? offsetGroupHide : Vector2.zero);
        objectsGroupParent.DOAnchorPos(offset, 0.25f).SetEase(groupCurve);
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
        if (Physics.Raycast(ray, out RaycastHit hit, 500f))
        {
            outValue = hit.point;
        }

        var v3 = SnapToGrid(outValue, 1f);
        var noYV3 = new Vector3(v3.x, 0, v3.z);
        
        return (noYV3, hit);
    }

    private Vector3 SnapToGrid(Vector3 pos, float gridSize)
    {
        Vector3 position = pos;

        // Calculate the snapped position
        float snappedX = Mathf.Round(position.x / gridSize) * gridSize;
        float snappedY = Mathf.Round(position.y / gridSize) * gridSize;
        float snappedZ = Mathf.Round(position.z / gridSize) * gridSize;

        // Set the object's position to the snapped position
        return new Vector3((int)snappedX, (int)snappedY, (int)snappedZ);
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
        var pos = SnapToGrid(point, 1f);
        return grid[(int)pos.x, (int)pos.z];
    }
}

[Serializable]
public class LevelEditorObject
{
    public GameObject obj;
    public Sprite sprite;
    public Color colorUI;
}
