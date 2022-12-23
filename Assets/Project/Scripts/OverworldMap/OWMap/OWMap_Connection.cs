using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OWMap_Connection : MonoBehaviour
{
    [SerializeField] private Transform connectionTransform;


    public void InitTransform(Vector3 cNodeLocalPos, Vector3 nNodeLocalPos, Vector3 mapForwardDir)
    {
        Vector3 currentToNext = nNodeLocalPos - cNodeLocalPos;
        Vector3 dirCurrentToNext = currentToNext.normalized;

        Vector3 currentToNextMidPoint = cNodeLocalPos + (dirCurrentToNext * (currentToNext.magnitude / 2.0f));
        Quaternion connectionRotation = Quaternion.FromToRotation(mapForwardDir, dirCurrentToNext);

        connectionTransform.localPosition = currentToNextMidPoint;
        connectionTransform.localRotation = connectionRotation;
    }


}
