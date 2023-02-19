using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DarknessFog : MonoBehaviour
{
    [Header("DISSIATION")]
    [SerializeField] private float dissipationTime;
    [SerializeField] private float dissipationInitScale;
    [SerializeField] private float dissipationEndScale;
    [SerializeField] private AnimationCurve dissipationCurve;

    [Header("CLEANUP DISSIPATION")]
    [SerializeField] private float cDissipationTime;
    private float cDissipationInitScale;
    [SerializeField] private float cDissipationEndScale;
    [SerializeField] private AnimationCurve cDissipationCurve;

    private void Awake()
    {
        transform.localScale = new Vector3(transform.localScale.x, dissipationInitScale, transform.localScale.y);

        cDissipationInitScale = dissipationEndScale;
    }

    public IEnumerator Dissipate()
    {
        yield return FogDissipateLerp(dissipationTime, dissipationInitScale, dissipationEndScale, dissipationCurve);
        yield return FogDissipateLerp(cDissipationTime, cDissipationInitScale, cDissipationEndScale, cDissipationCurve);
    }

    public IEnumerator FogDissipateLerp(float time, float initScale, float endScale, AnimationCurve curve)
    {
        float currentTime = 0.0f;
        float scaleDiff = endScale - initScale;

        while(currentTime <= time)
        {
            currentTime += Time.deltaTime;
            float tParam = curve.Evaluate(currentTime / dissipationTime);
            transform.localScale = new Vector3(transform.localScale.x, initScale + (scaleDiff * tParam), transform.localScale.y);

            yield return null;
        }

        transform.localScale = new Vector3(transform.localScale.x, endScale, transform.localScale.y);
    }
}
