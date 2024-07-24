using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FIMultiSocketSwitch : AFacilityInteractable
{
    [Header("REFERENCES")]
    [SerializeField] private FIComputerButton _computerButton;
    [SerializeField] private Transform _button;
    [SerializeField] private Transform _buttonMR;
    [SerializeField] private Light _multiSocketSwitchLight;
    [SerializeField] private FlickeringLightGroup _roomLightGroup;
    [SerializeField] private Transform _roomLightMR;
    [SerializeField] private RandomSoundsCollection _switchAudio;

    [Header("PARAMETERS")]
    [SerializeField] private Color32 _onLightColor;
    [SerializeField] private Color32 _offLightColor;
    [SerializeField] private float _lightMatDuration;
    [SerializeField] private float _buttonSwitchDuration;
    [SerializeField] private float _buttonSwitchRotation;
    [SerializeField] private Ease _buttonEase;

    private Material _buttonMat;
    private Material _roomLightMat;
    private bool _on = false;

    protected override void DoInit()
    {
        _buttonMat = _buttonMR.GetComponent<MeshRenderer>().material;
        _roomLightMat = _roomLightMR.GetComponent<MeshRenderer>().materials[0];
    }

    protected override IEnumerator DoInteract()
    {
        _switchAudio.PlayRandomSound();

        if (_on) yield return TurnOff();
        else yield return TurnOn();

        yield return new WaitForSeconds(_duration);
    }

    private IEnumerator TurnOn()
    {
        _on = true;
        _multiSocketSwitchLight.color = _onLightColor;
        _roomLightMat.DOFloat(1.0f, "_Activate", _lightMatDuration);

        _buttonMat.DOFloat(1.0f, "_Activate", _buttonSwitchDuration);
        _button.DOLocalRotate(new Vector3(0.0f, 0.0f, _buttonSwitchRotation), _buttonSwitchDuration)
            .SetEase(_buttonEase);

        _roomLightGroup.Activate();

        _computerButton.ChangeElectricityState(true);

        _manager.IsMultiSocketOn = true;
        yield return null;
    }

    private IEnumerator TurnOff()
    {
        _on = false;
        _multiSocketSwitchLight.color = _offLightColor;
        _roomLightMat.DOFloat(0.0f, "_Activate", _lightMatDuration);

        _buttonMat.DOFloat(0.0f, "_Activate", _buttonSwitchDuration);
        _button.DOLocalRotate(new Vector3(0.0f, 0.0f, -_buttonSwitchRotation), _buttonSwitchDuration)
            .SetEase(_buttonEase);

        _roomLightGroup.Deactivate();

        _computerButton.ChangeElectricityState(false);

        _manager.IsMultiSocketOn = false;
        yield return null;
    }

    public override void InteractedStart()
    {
        StartCoroutine(TurnOn());
    }
}
