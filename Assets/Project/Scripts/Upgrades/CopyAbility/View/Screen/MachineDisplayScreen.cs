using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class MachineDisplayScreen : MachineMovablePart
{
    [Header("DISPLAY")]
    [SerializeField] private MeshRenderer _screenOnOffMesh;
    [SerializeField] private MachineTweenConfig _screenFlashTC;
    [SerializeField] private MachineTweenConfig _screenOnTC;
    private Material _screenOnOffMat;

    [Header("LIGHT INDICATOR")]
    [SerializeField] private float _openLightIndDelay;
    [SerializeField] private float _readyBlinkTimeInterval;
    [SerializeField] private MeshRenderer _lightIndicatorMesh;
    [SerializeField] private Light _waitingLight;
    [SerializeField] private Light _readyLight;
    [SerializeField] private Light _replacingLight;
    private Material _lightIndicatorMaterial;
    private float _originalLightIntensity;

    public override void Init()
    {
        _lightIndicatorMaterial = _lightIndicatorMesh.material;
        _screenOnOffMat = _screenOnOffMesh.material;

        _originalLightIntensity = _readyLight.intensity;
        _lightIndicatorMaterial.SetFloat("_On", 0.0f);
    }

    public override IEnumerator EnterAnimation()
    {
        yield return new WaitForSeconds(_openLightIndDelay);
        //_lightIndicatorMaterial.SetFloat("_On", 1.0f);
        StartCoroutine(EnterWaiting());
    }

    public override IEnumerator ExitAnimation()
    {
        yield return null;
    }

    public IEnumerator EnterWaiting()
    {
        _lightIndicatorMaterial.SetFloat("_Ready", 0.0f);
        _lightIndicatorMaterial.SetFloat("_Replacing", 0.0f);
        _waitingLight.gameObject.SetActive(true);
        _readyLight.gameObject.SetActive(false);
        _replacingLight.gameObject.SetActive(false);

        StopCoroutine(BlinkingReadyLight());
        yield return null;
    }

    public IEnumerator EnterReady()
    {
        //_lightIndicatorMaterial.SetFloat("_Ready", 1.0f);
        //_lightIndicatorMaterial.SetFloat("_Replacing", 0.0f);
        _waitingLight.gameObject.SetActive(false);
        _readyLight.gameObject.SetActive(true);
        _replacingLight.gameObject.SetActive(false);

        StartCoroutine(BlinkingReadyLight());
        yield return null;
    }

    public IEnumerator EnterReplacing()
    {
        //_lightIndicatorMaterial.SetFloat("_Ready", 0.0f);
        //_lightIndicatorMaterial.SetFloat("_Replacing", 1.0f);
        _waitingLight.gameObject.SetActive(false);
        _readyLight.gameObject.SetActive(false);
        _replacingLight.gameObject.SetActive(true);

        StopCoroutine(BlinkingReadyLight());
        yield return null;
    }

    private IEnumerator BlinkingReadyLight()
    {
        while (true)
        {
            _readyLight.intensity = _originalLightIntensity;
            //_lightIndicatorMaterial.SetFloat("_On", 1.0f);
            yield return new WaitForSeconds(_readyBlinkTimeInterval);

            _readyLight.intensity = 0.0f;
            //_lightIndicatorMaterial.SetFloat("_On", 0.0f);
            yield return new WaitForSeconds(_readyBlinkTimeInterval);
        }
    }

    [Button]
    public void Warning()
    {
        StopAllCoroutines();
        //_lightIndicatorMaterial.SetFloat("_On", 1.0f);
        StartCoroutine(EnterWaiting());
        StartCoroutine(WaitingScreen());
    }

    [Button]
    private void Ready()
    {
        StopAllCoroutines();
        //_lightIndicatorMaterial.SetFloat("_On", 1.0f);
        StartCoroutine(EnterReady());
        StartCoroutine(ReadyScreen());
    }

    private IEnumerator WaitingScreen()
    {
        _screenOnOffMat.SetFloat("_OnCoef", 0.0f);
        _screenOnOffMat.SetFloat("_FlashCoef", 0.0f);
        yield return null;
    }

    private IEnumerator ReadyScreen()
    {
        _screenOnOffMat.DOFloat(_screenFlashTC.Value, "_FlashCoef", _screenFlashTC.Duration).SetEase(_screenFlashTC.Ease);
        yield return new WaitForSeconds(_screenFlashTC.Duration);
        _screenOnOffMat.DOFloat(_screenOnTC.Value, "_OnCoef", _screenOnTC.Duration).SetEase(_screenOnTC.Ease);
    }
}
