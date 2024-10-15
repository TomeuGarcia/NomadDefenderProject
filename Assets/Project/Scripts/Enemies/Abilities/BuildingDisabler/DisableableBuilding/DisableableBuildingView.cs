using System;
using DG.Tweening;
using UnityEngine;

public class DisableableBuildingView : MonoBehaviour
{
    [SerializeField] private MeshRenderer _disabledWaveMesh;
    private Material _disabledWaveMaterial;

    [SerializeField] private AnimationCurve _disableProgression;
    [SerializeField] private AnimationCurve _alphaCurve;
    [SerializeField] private TweenPunchConfig _spawnScalePunch;
    

    private void Awake()
    {
        _disabledWaveMaterial = _disabledWaveMesh.material;
        Hide();
    }

    public void UpdateView(float disabledRatio01)
    {
        float disableProgressionT = _disableProgression.Evaluate(disabledRatio01);
        float alphaMultiplier = _alphaCurve.Evaluate(disabledRatio01);
        _disabledWaveMaterial.SetFloat("_EmptyT", disableProgressionT);
        _disabledWaveMaterial.SetFloat("_AlphaMultiplier", alphaMultiplier);
    }


    public void Show()
    {
        _disabledWaveMesh.gameObject.SetActive(true);
        _disabledWaveMesh.transform.PunchScale(_spawnScalePunch);
    }
    public void Hide()
    {
        _disabledWaveMesh.gameObject.SetActive(false);
    }
}