using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceLineRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;

    [SerializeField] private List<Vector3> _points = new List<Vector3>();
    public List<Vector3> Points => _points;

    public Vector3 OriginPosition => transform.position;

    public void UpdateLine()
    {
        _lineRenderer.positionCount = _points.Count;
        for (int i = 0; i < _points.Count; ++i)
        {
            _lineRenderer.SetPosition(i, _points[i] + OriginPosition);
        }
    }


    public Vector3 GetPointWorldSpace(int index)
    {
        return _points[index] + OriginPosition;
    }
    public void MovePoint(int index, Vector3 worldSpacePoint)
    {
        _points[index] = worldSpacePoint - OriginPosition;
    }
}
