using System.Collections;
using UnityEngine;

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

    [Header("CARD SLOT")]
    [SerializeField] private GameObject _sWarningLightParent;
    [SerializeField] private GameObject _sReadyLightParent;
    [SerializeField] private MeshRenderer[] _sIndMeshes;
    private Material[] _sIndMaterials;

    private string _readyProppertyName = "_ReadyCoef";
    private string _upgradingProppertyName = "_UpgradingCoef";

    private void Awake()
    {
        _botMaterial = _botMeshRenderer.material;
        _topMaterial = _topMeshRenderer.material;

        _sIndMaterials = new Material[_sIndMeshes.Length];

        for (int i = 0; i < _sIndMeshes.Length; i++)
        {
            _sIndMaterials[i] = _sIndMeshes[i].materials[i+1];
        }
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

        foreach(Material mat in _sIndMaterials)
        {
            mat.SetFloat(propperty, value);
        }

        SetLight(propperty, value);
    }

    private void SetLight(string propperty, float value)
    {
        bool active = value == 1.0f ? true : false;

        if (propperty == _readyProppertyName)
        {
            _warningLight.SetActive(!active);
            _readyLight.SetActive(active);

            foreach (Transform t in _sWarningLightParent.transform)
            {
                t.GetComponent<Light>().enabled = !active;
            }
            foreach (Transform t in _sReadyLightParent.transform)
            {
                t.GetComponent<Light>().enabled = active;
            }
        }
        else
        {
            _readyLight.SetActive(!active);
            _upgradingLight.SetActive(active);

            foreach(Transform t in _sWarningLightParent.transform)
            {
                t.GetComponent<Light>().enabled = !active;
            }
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
