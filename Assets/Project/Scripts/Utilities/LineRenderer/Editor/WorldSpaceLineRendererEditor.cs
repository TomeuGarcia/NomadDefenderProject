
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldSpaceLineRenderer))]
public class WorldSpaceLineRendererEditor : UnityEditor.Editor
{
    private WorldSpaceLineRenderer _worldSpaceLineRenderer;
    
    private void OnEnable()
    {
        _worldSpaceLineRenderer = target as WorldSpaceLineRenderer;
    }

    private void OnSceneGUI()
    {
        DrawButtons();
        _worldSpaceLineRenderer.UpdateLine();
    }

    private void DrawButtons()
    {
        for (int i = 0; i < _worldSpaceLineRenderer.Points.Count; ++i)
        {
            Vector3 position = _worldSpaceLineRenderer.GetPointWorldSpace(i);
            Vector3 newPosition =
                Handles.DoPositionHandle(position, Quaternion.identity);
            
            if (position != newPosition)
            {
                Undo.RecordObject(_worldSpaceLineRenderer, "Move point");
                _worldSpaceLineRenderer.MovePoint(i, newPosition);
            }
        }
    }
    
}