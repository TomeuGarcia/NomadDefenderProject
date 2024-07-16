using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FIMultiSocketSwitch : AFacilityInteractable
{
    [Header("REFERENCES")]
    [SerializeField] private Transform _button;
    [SerializeField] private Transform _buttonMR;
    [SerializeField] private Light _multiSocketSwitchLight;
    [SerializeField] private List<Light> _roomLight = new();
    [SerializeField] private Transform _roomLightMR;

    [Header("PARAMETERS")]
    [SerializeField] private Color32 _onLightColor;
    [SerializeField] private Color32 _offLightColor;
    [SerializeField] private float _lightMatDuration;
    [SerializeField] private float _buttonSwitchDuration;
    [SerializeField] private float _buttonSwitchRotation;
    [SerializeField] private Ease _buttonEase;

    private Material _buttonMaterial;
    private Material _roomLightMat;
    private bool _on = false;

    protected override void DoInit()
    {
        _buttonMaterial = _buttonMR.GetComponent<MeshRenderer>().material;
        _roomLightMat = _roomLightMR.GetComponent<MeshRenderer>().materials[0];
    }

    protected override IEnumerator DoInteract()
    {
        if (_on) yield return TurnOff();
        else yield return TurnOn();
    }

    private IEnumerator TurnOn()
    {
        _multiSocketSwitchLight.color = _onLightColor;
        _roomLightMat.DOFloat(1.0f, "_Activate", _lightMatDuration);

        _buttonMaterial.DOFloat(1.0f, "_Activate", _buttonSwitchDuration);
        _button.DOLocalRotate(new Vector3(0.0f, 0.0f, _buttonSwitchRotation), _buttonSwitchDuration)
            .SetEase(_buttonEase);

        foreach (var light in _roomLight)
        {
            light.gameObject.SetActive(true);
        }

        yield return _duration;
    }

    private IEnumerator TurnOff()
    {
        _multiSocketSwitchLight.color = _offLightColor;
        _roomLightMat.DOFloat(0.0f, "_Activate", _lightMatDuration);

        _buttonMaterial.DOFloat(0.0f, "_Activate", _buttonSwitchDuration);
        _button.DOLocalRotate(new Vector3(0.0f, 0.0f, -_buttonSwitchRotation), _buttonSwitchDuration)
            .SetEase(_buttonEase);

        foreach (var light in _roomLight)
        {
            light.gameObject.SetActive(false);
        }

        yield return _duration;
    }
}
