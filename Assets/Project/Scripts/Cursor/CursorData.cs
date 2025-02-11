using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CursorData_NAME",
    menuName = "Cursor/CursorData")]
public class CursorData : ScriptableObject
{
    [SerializeField] private Texture2D _texture;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private Vector2 _hotSpot;
    [SerializeField] private CursorMode _cursorMode;

    public Texture2D Texture => _texture;
    public Sprite Sprite => _sprite;
    public Vector2 HotSpot => _hotSpot;
    public CursorMode CursorMode => _cursorMode;
}
