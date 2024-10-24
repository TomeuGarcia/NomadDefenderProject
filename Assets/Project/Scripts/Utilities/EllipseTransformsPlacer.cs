using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class EllipseTransformsPlacer : MonoBehaviour
{
    [SerializeField] private float _transformLength = 0.4f;
    [SerializeField] private Transform[] _transforms;
        

    [Button()]
    private void Place()
    {
        int numberOfTransforms = _transforms.Length;
        int numberOfTransformsPlusOne = numberOfTransforms + 1;
            
        float circumference = (numberOfTransformsPlusOne) * _transformLength;
        float radius = circumference / (2 * Mathf.PI);
            
            
        for (int i = 0; i < _transforms.Length; ++i)
        {
            Vector3 startOffset = GetUnitCirclePosition(i, numberOfTransforms) * radius;
            int nextIndex = (i+1) % numberOfTransforms;
            Vector3 endOffset = GetUnitCirclePosition(nextIndex, numberOfTransforms) * radius;
                
                
            Vector3 start = startOffset;
            Vector3 end = endOffset;
            Vector3 startToEndDirection = (end - start).normalized;

            Vector3 position = Vector3.LerpUnclamped(start, end, 0.5f);
            Quaternion rotation = Quaternion.LookRotation(startToEndDirection, Vector3.up);

            _transforms[i].localPosition = position;
            _transforms[i].localRotation = rotation;
        }
    }

    private Vector3 GetUnitCirclePosition(int index, float total)
    {
        float t = index / total;
        float circumferenceT = 2 * Mathf.PI * t;

        float offsetX = Mathf.Cos(circumferenceT);
        float offsetZ = Mathf.Sin(circumferenceT);

        return new Vector3(offsetX, 0, offsetZ);
    }
}
