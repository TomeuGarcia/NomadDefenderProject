using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CursorSet
{
    public CursorData _regularData;
    public CursorData _hoverData;
    //public CursorData _unavilableData;
}

public enum CursorContext
{
    FACILITY, COMPUTER
}

public class CursorChanger : MonoBehaviour
{
    [SerializeField] private CursorSet _facilityCursor;
    [SerializeField] private CursorSet _computerCursor;

    private CursorContext _cursorContext = CursorContext.FACILITY;

    public void ChangeCursorContext(CursorContext cursorContext)
    {
        _cursorContext = cursorContext;
    }

    public void HoverCursor()
    {
        if(_cursorContext == CursorContext.FACILITY)
        {
            SetCursor(_facilityCursor._hoverData);
        }
        else if(_cursorContext == CursorContext.COMPUTER)
        {
            SetCursor(_computerCursor._hoverData);
        }
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

    private void SetCursor(CursorData cursorData)
    {
        Cursor.SetCursor(cursorData.Texture, cursorData.HotSpot, cursorData.CursorMode);
    }
}
