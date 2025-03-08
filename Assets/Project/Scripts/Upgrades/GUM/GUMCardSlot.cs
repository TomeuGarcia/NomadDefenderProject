using UnityEngine;

public class GUMCardSlot : MonoBehaviour
{
    //[SerializeField] private GameObject _generalLights;
    [SerializeField] private Light[] _generalLights;
    [SerializeField] private GameObject _hoverLights;
    [SerializeField] private MeshRenderer _topIndicatorMesh;
    private Material _topIndicatorMat;

    private void Awake()
    {
        _topIndicatorMat = _topIndicatorMesh.materials[2];
    }

    public void Hover()
    {
        _topIndicatorMat.SetFloat("_HoverCoef", 1.0f);

        _hoverLights.SetActive(true);
        //_generalLights.SetActive(false);
        foreach (Light light in _generalLights)
        {
            light.range = 0.0f;
        }
    }

    public void Unhover()
    {
        _topIndicatorMat.SetFloat("_HoverCoef", 0.0f);

        _hoverLights.SetActive(false);
        //_generalLights.SetActive(true);
        foreach (Light light in _generalLights)
        {
            light.range = 10.0f;
        }
    }

    public void Upgrading()
    {
        _hoverLights.SetActive(false);
    }
}
