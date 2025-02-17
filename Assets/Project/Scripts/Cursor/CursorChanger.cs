using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CursorSet
{
    public CursorData _regularData;
    public CursorData _hoverData;
    public CursorData _forbiddenData;
}

public enum CursorContext
{
    FACILITY, COMPUTER
}

public class CursorChanger : MonoBehaviour
{
    [SerializeField] private CursorSet _facilityCursor;
    [SerializeField] private CursorSet _computerCursor;
    [SerializeField] private RectTransform _cursorMove;
    [SerializeField] private Image _cursorImage;

    private CursorContext _cursorContext = CursorContext.COMPUTER;

    
    private void Start()
    {
        ServiceLocator.GetInstance().CursorChanger = this;
        DontDestroyOnLoad(gameObject);
        
        #if UNITY_EDITOR
        _cursorImage.enabled = false;
        #endif
    }
    
    private void Update()
    {
        _cursorMove.position = Input.mousePosition;
        if (Input.GetKeyDown(KeyCode.H) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift))
        {
            _cursorImage.gameObject.SetActive(!_cursorImage.gameObject.activeInHierarchy);
        }
        #if UNITY_EDITOR
        #else
        Cursor.visible = false;
        if (Cursor.lockState != CursorLockMode.Confined && Input.GetKeyDown(KeyCode.Mouse0))
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        #endif
    }

    public void ChangeCursorContext(CursorContext cursorContext)
    {
        _cursorContext = cursorContext;
    }

    public void RegularCursor()
    {
        if (_cursorContext == CursorContext.FACILITY)
        {
            SetCursor(_facilityCursor._regularData);
        }
        else if (_cursorContext == CursorContext.COMPUTER)
        {
            SetCursor(_computerCursor._regularData);
        }
    }

    public void HoverCursor()
    {
        if (_cursorContext == CursorContext.FACILITY)
        {
            SetCursor(_facilityCursor._hoverData);
        }
        else if (_cursorContext == CursorContext.COMPUTER)
        {
            SetCursor(_computerCursor._hoverData);
        }
    }

    public void ForbiddenCursor()
    {
        if (_cursorContext == CursorContext.FACILITY)
        {
            SetCursor(_facilityCursor._forbiddenData);
        }
        else if (_cursorContext == CursorContext.COMPUTER)
        {
            SetCursor(_computerCursor._forbiddenData);
        }
    }

    private void SetCursor(CursorData cursorData)
    {
        //Cursor.SetCursor(cursorData.Texture, cursorData.HotSpot, cursorData.CursorMode);
        _cursorImage.sprite = cursorData.Sprite;
    }
    
}
