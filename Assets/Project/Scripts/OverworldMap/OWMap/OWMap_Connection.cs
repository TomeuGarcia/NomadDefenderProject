using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OWMap_Connection : MonoBehaviour
{
    [SerializeField] private static Color defaultColor = new Color(106f / 255f, 106f / 255f, 106f / 255f);
    [SerializeField] private static Color selectedColor = new Color(38f / 255f, 142f / 255f, 138f / 255f);
    [SerializeField] private static Color unavailableColor = new Color(140f / 255f, 7f / 255f, 36f / 255f);

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
