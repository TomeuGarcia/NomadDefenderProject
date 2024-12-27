using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class MachineDisplayScreen : MachineMovablePart
{
    [Header("LIGHT INDICATOR")]
    [SerializeField] private float _readyBlinkTimeInterval;
    [SerializeField] private MeshRenderer _lightIndicatorMesh;
    [SerializeField] private Light _waitingLight;
    [SerializeField] private Light _readyLight;
    [SerializeField] private Light _replacingLight;
    private Material _lightIndicatorMaterial;

    public override void Init()
    {
        _lightIndicatorMaterial = _lightIndicatorMesh.material;
    }

    public override IEnumerator EnterAnimation()
    {
        yield return null;
    }

    public override IEnumerator ExitAnimation()
    {
        yield return null;
    }

    public IEnumerator EnterWaiting()
    {
        _waitingLight.gameObject.SetActive(true);
        _readyLight.gameObject.SetActive(false);
        _replacingLight.gameObject.SetActive(false);

        StopCoroutine(BlinkingReadyLight());
        yield return null;
    }

    public IEnumerator EnterReady()
    {
        _waitingLight.gameObject.SetActive(false);
        _readyLight.gameObject.SetActive(true);
        _replacingLight.gameObject.SetActive(false);

        StartCoroutine(BlinkingReadyLight());
        yield return null;
    }

    public IEnumerator EnterReplacing()
    {
        _waitingLight.gameObject.SetActive(false);
        _readyLight.gameObject.SetActive(false);
        _replacingLight.gameObject.SetActive(true);

        StopCoroutine(BlinkingReadyLight());
        yield return null;
    }

    private IEnumerator BlinkingReadyLight()
    {
        while(true)
        {
            _readyLight.enabled = true;
            _lightIndicatorMaterial.SetFloat("_Blink", 1.0f);
            yield return new WaitForSeconds(_readyBlinkTimeInterval);

            _readyLight.enabled = false;
            _lightIndicatorMaterial.SetFloat("_Blink", 0.0f);
            yield return new WaitForSeconds(_readyBlinkTimeInterval);
        }
    }
}
