using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GizmosUtility
{
    public static void DrawCircle(Vector3 center, Vector3 normal, float radius, float distanceStep = 0.25f)
    {
        const float TAU = 2f * Mathf.PI;
        float circlePerimeter = TAU * radius;
            

        normal.Normalize();
        bool normalIsUp = Vector3.Dot(normal, Vector3.up) > 0.99f;
        List<Vector3> circlePositions = new List<Vector3>(Mathf.FloorToInt(circlePerimeter / distanceStep));
        Vector3 up = normalIsUp
            ? Vector3.Cross(normal, Vector3.right) 
            : Vector3.Cross(normal, Vector3.up);
        up *= radius;
        Vector3 right = normalIsUp
            ? Vector3.Cross(normal, Vector3.forward) 
            : Vector3.Cross(normal, Vector3.right);
        right *= radius;
            

        float distanceCounter = 0f;
        while (distanceCounter < circlePerimeter)
        {
            float circleRatio = distanceCounter / circlePerimeter;
            float angle = TAU * circleRatio;

            Vector3 position = center 
                               + right * Mathf.Cos(angle)
                               + up * Mathf.Sin(angle);
            circlePositions.Add(position);
                
            distanceCounter += distanceStep;
        }

        Vector3 closingPosition = center 
                                  + right * Mathf.Cos(TAU)
                                  + up * Mathf.Sin(TAU);
        circlePositions.Add(closingPosition);
            
        Gizmos.DrawLineStrip(circlePositions.ToArray(), true);
    }



    public static void DrawArrow(Vector3 from, Vector3 to, float headDistance = 0.25f)
    {
        Vector3 fromToVector = to - from;
        float fromToDistance = fromToVector.magnitude;

        if (fromToDistance.AlmostZero())
        {
            return;
        }
        
        Vector3 fromToDirection = fromToVector / fromToDistance;

        Vector3 headNormal = Mathf.Abs(Vector3.Dot(fromToDirection, Vector3.up)).AlmostEquals(1f)
            ? Vector3.right
            : Vector3.up;

        Vector3 headAxis = Vector3.Cross(headNormal, fromToDirection);

        Vector3 toFromDirection = -fromToDirection;
        Vector3 headA = Quaternion.AngleAxis(45f, headAxis) * toFromDirection * headDistance;
        Vector3 headB = Quaternion.AngleAxis(-45f, headAxis) * toFromDirection * headDistance;
        
        Gizmos.DrawLine(from, to);
        Gizmos.DrawLine(to, to + headA);
        Gizmos.DrawLine(to, to + headB);
    }
}
