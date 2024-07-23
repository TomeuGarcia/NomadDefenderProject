using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private CursorContext _cursorContext = CursorContext.COMPUTER;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            SetCursor(_computerCursor._hoverData);
        }
    }

    private void Start()
    {
        ServiceLocator.GetInstance().CursorChanger = this;
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
        Cursor.SetCursor(cursorData.Texture, cursorData.HotSpot, cursorData.CursorMode);
    }
}
