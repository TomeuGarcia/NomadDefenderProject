using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GUMSideIndicator : MonoBehaviour
{
    [SerializeField] private MeshRenderer _botMeshRenderer;
    private Material _botMaterial;
    [SerializeField] private MeshRenderer _topMeshRenderer;
    private Material _topMaterial;

    [Header("LIGHTS")]
    [SerializeField] private GameObject _warningLight;
    [SerializeField] private GameObject _readyLight;
    [SerializeField] private GameObject _upgradingLight;

    private string _readyProppertyName = "_ReadyCoef";
    private string _upgradingProppertyName = "_UpgradingCoef";

    private void Awake()
    {
        _botMaterial = _botMeshRenderer.material;
        _topMaterial = _topMeshRenderer.material;
    }

    private IEnumerator Blink(string propperty, float initialValue)
    {
        SetFloat(propperty, initialValue);
        yield return new WaitForSeconds(0.1f);
        SetFloat(propperty, 1.0f - initialValue);
        yield return new WaitForSeconds(0.05f);

        SetFloat(propperty, initialValue);
        yield return new WaitForSeconds(0.1f);
        SetFloat(propperty, 1.0f - initialValue);
        yield return new WaitForSeconds(0.05f);

        SetFloat(propperty, initialValue);
    }

    private void SetFloat(string propperty, float value)
    {
        _botMaterial.SetFloat(propperty, value);
        _topMaterial.SetFloat(propperty, value);

        SetLight(propperty, value);
    }

    private void SetLight(string propperty, float value)
    {
        bool active = value == 1.0f ? true : false;

        if (propperty == _readyProppertyName)
        {
            _warningLight.SetActive(!active);
            _readyLight.SetActive(active);
        }
        else
        {
            _readyLight.SetActive(!active);
            _upgradingLight.SetActive(active);
        }
    }

    public void Ready()
    {
        StopAllCoroutines();
        StartCoroutine(Blink(_readyProppertyName, 1.0f));
    }

    public void UnReady()
    {
        StopAllCoroutines();
        StartCoroutine(Blink(_readyProppertyName, 0.0f));
    }

    public void Upgrading()
    {
        StopAllCoroutines();
        StartCoroutine(Blink(_upgradingProppertyName, 1.0f));
    }
}
