using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class BlastWave : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private AnimationCurve curve;
    [SerializeField] private int pointAmount;
    [SerializeField] private float maxRadius;
    [SerializeField] private float speed;

    private Vector3[] directions;

    void Awake()
    {
        lineRenderer.positionCount = pointAmount + 1;

        /////////
        float angleBetweenPoints = 360.0f / (float)pointAmount;
        directions = new Vector3[pointAmount + 1];
        for (int i = 0; i <= pointAmount; i++)
        {
            float angle = i * angleBetweenPoints * Mathf.Deg2Rad;
            directions[i] = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0.0f);
        }
    }

    public void Activate()
    {
        StartCoroutine(Blast());
    }

    private IEnumerator Blast()
    {
        float currentRadius = 0.0f;

        while(currentRadius < maxRadius)
        {
            currentRadius += Time.deltaTime * speed;

            for (int i = 0; i <= pointAmount; i++)
            {
                lineRenderer.SetPosition(i, directions[i] * currentRadius);
            }
            lineRenderer.widthMultiplier = curve.Evaluate(1.0f - (currentRadius / maxRadius));

            yield return null;
        }
    }
}
