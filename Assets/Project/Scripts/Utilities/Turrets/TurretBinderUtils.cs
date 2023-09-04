using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TurretBinderUtils
{
    public static void UpdateTurretBinder(Transform binderTransform, Transform targetTransform, Transform bindOriginTransform)
    {
        Vector3 targetPosition = targetTransform.position + Vector3.up * 0.2f;

        // Find scale
        float distance = Vector3.Distance(bindOriginTransform.position, targetPosition);
        Vector3 binderScale = binderTransform.lossyScale;
        float scaleFactor = 1f;
        binderScale.z = distance * scaleFactor;


        // Find center position
        Vector3 centerPosition = Vector3.Lerp(bindOriginTransform.position, targetPosition, 0.5f);


        // Find rotation
        Vector3 directionToTarget = (targetPosition - bindOriginTransform.position).normalized;
        Quaternion binderRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);


        // Apply changes
        binderTransform.localScale = binderScale;
        binderTransform.position = centerPosition;
        binderTransform.rotation = binderRotation;
    }
}
