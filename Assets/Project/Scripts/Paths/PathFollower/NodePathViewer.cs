using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodePathViewer : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Vector3 _positionOffset = Vector3.up * 0.2f;

    private void Awake()
    {
        Hide();
    }

    public void Show(PathNode startingNode)
    {
        List<Vector3> points = new();
        while (startingNode != null)
        {
            points.Add(startingNode.Position + _positionOffset);
            startingNode = startingNode.GetNextNode();
        }

        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = points.Count;
        _lineRenderer.SetPositions(points.ToArray());
    }

    public void Hide()
    {
        _lineRenderer.enabled = false;
    }
}
