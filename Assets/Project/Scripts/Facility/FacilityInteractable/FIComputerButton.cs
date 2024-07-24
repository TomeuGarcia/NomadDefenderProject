using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[System.Serializable]
public class BootUpFragment
{
    public List<GameObject> TextList = new();
    public float TimeToActivate;
}

public class FIComputerButton : AFacilityInteractable
{
    [Header("REFERENCES")]
    [SerializeField] private Light _buttonLight;
    [SerializeField] private Transform _button;
    [SerializeField] private GameObject _blackScreen;
    [SerializeField] private GameObject _blueScreen;
    [SerializeField] private Transform _screenParent;
    [SerializeField] private Light _screenLight;
    [SerializeField] private Transform _backgroundMap;
    [SerializeField] private Transform _bootupLogo;
    [SerializeField] private Transform _bootUpTextParent;
    [SerializeField] private List<Image> _titleArrows = new();
    [SerializeField] private TextDecoder _titleDecoder;
    [SerializeField, TextArea] private string _titleText;
    [SerializeField] private GameObject _titleTextShadow;
    [SerializeField] private FlickeringLightGroup _flickeringLightGroup;
    [SerializeField] private List<BootUpFragment> _bootUpTexts = new();

    [Header("AUDIO")]
    [SerializeField] private RandomSoundsCollection _computerButtonAudio;
    [SerializeField] private RandomSoundsCollection _computerStartupAudio;
    [SerializeField] private RandomSoundsCollection _computerNoiseStartupAudio;
    [SerializeField] private RandomSoundsCollection _computerNoiseBackgroundAudio;
    [SerializeField] private RandomSoundsCollection _computerNoiseShutDownAudio;

    [Header("PARAMETERS")]
    [SerializeField] private Color32 _buttonLightOnColor;
    [SerializeField] private Color32 _buttonLightOffColor;
    [SerializeField] private Color32 _screenBlueLightColor;
    [SerializeField] private Vector3 _deformedTextScale;
    [SerializeField] private float _buttonMoveTime;
    [SerializeField] private float _buttonMoveWait;
    [SerializeField] private Ease _buttonMoveEase;
    [SerializeField] private float _firstBlackScreenTime;
    [SerializeField] private float _bootupLogoTime;
    [SerializeField] private float _showTextTime;
    [SerializeField] private float _deformedTextTime;
    [SerializeField] private float _blueScreenTime;
    [SerializeField] private float _lastBlackScreenTime;
    [SerializeField] private Vector3 _deformedMapScale;
    [SerializeField] private float _deformedMapTime;
    [SerializeField] private float _engageTime;

    private Material _buttonMat;
    private float _defaultButtonZPosition;
    private Color32 _defaultScreenLightColor;
    private Color32 _defaultTextColor;

    protected override void DoAwake()
    {
        _buttonMat = _button.GetComponent<MeshRenderer>().material;
        _defaultButtonZPosition = _button.localPosition.z;
        _defaultScreenLightColor = _screenLight.color;
        _bootupLogo.gameObject.SetActive(false);
        _defaultTextColor = _titleDecoder.textComponent.color;

        SetTitleArrowState(false);
    }

    protected override bool ExtraInteractionConditions()
    {
        return _manager.IsMultiSocketOn;
    }

    protected override IEnumerator DoInteract()
    {
        _computerButtonAudio.PlayRandomSound();

        if (_manager.IsPCOn)
        {
            yield return TurnOff();

            yield return new WaitForSeconds(_duration);
        }
        else
        {
            yield return TurnOn();
            StartCoroutine(DecodeTitleText());
        }
    }

    private IEnumerator DecodeTitleText()
    {
        _titleDecoder.ResetDecoder();
        _titleDecoder.gameObject.SetActive(true);
        _titleDecoder.SetTextStrings(_titleText);
        _titleDecoder.Activate();
        _titleDecoder.NextLine();
        yield return new WaitUntil(() => _titleDecoder.IsDoneDecoding() == true);

        SetTitleArrowState(true);
    }

    private IEnumerator TurnOn()
    {
        _titleTextShadow.gameObject.SetActive(false);
        _manager.IsPCOn = true;
        yield return MoveButton();

        _computerNoiseStartupAudio.PlayRandomSound();

        _buttonLight.color = _buttonLightOnColor;

        _blackScreen.gameObject.SetActive(true);
        _screenParent.gameObject.SetActive(true);
        yield return new WaitForSeconds(_firstBlackScreenTime);

        _bootupLogo.gameObject.SetActive(true);
        _computerStartupAudio.PlayRandomSound();
        yield return new WaitForSeconds(_bootupLogoTime);
        _bootupLogo.gameObject.SetActive(false);

        _flickeringLightGroup.Activate();
        _blackScreen.gameObject.SetActive(false);
        _blueScreen.gameObject.SetActive(true);
        _screenLight.color = _screenBlueLightColor;
        yield return new WaitForSeconds(_blueScreenTime);

        foreach (BootUpFragment bootUpFragment in _bootUpTexts)
        {
            yield return new WaitForSeconds(bootUpFragment.TimeToActivate);

            foreach (GameObject text in bootUpFragment.TextList)
            {
                text.SetActive(true);
            }
        }
        yield return new WaitForSeconds(_showTextTime);

        _bootUpTextParent.localScale = _deformedTextScale;
        yield return new WaitForSeconds(_deformedTextTime);

        foreach (BootUpFragment bootUpFragment in _bootUpTexts)
        {
            foreach (GameObject text in bootUpFragment.TextList)
            {
                text.SetActive(false);
            }
        }
        _bootUpTextParent.localScale = Vector3.one;

        _blackScreen.gameObject.SetActive(true);
        _blueScreen.gameObject.SetActive(false);
        _screenLight.color = _defaultScreenLightColor;
        yield return new WaitForSeconds(_lastBlackScreenTime);
        _blackScreen.gameObject.SetActive(false);

        _blueScreen.gameObject.SetActive(true);
        yield return new WaitForSeconds(_engageTime);
        _blueScreen.gameObject.SetActive(false);

        _backgroundMap.localScale = _deformedMapScale;
        _titleTextShadow.gameObject.SetActive(true);
        yield return new WaitForSeconds(_deformedMapTime);
        _backgroundMap.localScale = Vector3.one;
        _computerNoiseBackgroundAudio.StartPlaying();
    }

    private IEnumerator TurnOff()
    {
        _manager.IsPCOn = false;
        yield return MoveButton();

        ComputerTurnOffAudio();

        _flickeringLightGroup.Deactivate();
        _buttonLight.color = _buttonLightOffColor;
        SetTitleArrowState(false);
        _titleDecoder.gameObject.SetActive(false);
        _screenParent.gameObject.SetActive(false);

        yield return null;
    }

    private IEnumerator MoveButton()
    {
        //TODO - Add sound
        _button.DOLocalMoveZ(0.0f, _buttonMoveTime).SetEase(_buttonMoveEase);
        yield return new WaitForSeconds(_buttonMoveTime + _buttonMoveWait);
        _button.DOLocalMoveZ(_defaultButtonZPosition, _buttonMoveTime).SetEase(_buttonMoveEase);
    }

    public void ChangeElectricityState(bool state)
    {
        StopAllCoroutines();

        if (!state && _manager.IsPCOn)
        {
            _manager.IsPCOn = false;
            ComputerTurnOffAudio();
        }

        _titleDecoder.textComponent.color = _defaultTextColor;

        SetTitleArrowState(false);
        foreach (Image arrow in _titleArrows)
        {
            arrow.color = _defaultTextColor;
        }

        _titleDecoder.ResetDecoder();
        _titleDecoder.StopAllCoroutines();

        _flickeringLightGroup.Deactivate();

        _buttonLight.gameObject.SetActive(state);
        _buttonMat.SetFloat("_Activate", state ? 1 : 0);

        _buttonLight.color = _buttonLightOffColor;
        _screenParent.gameObject.SetActive(false);
    }

    public override void InteractedStart()
    {
        StartCoroutine(DecodeTitleText());
        _titleTextShadow.gameObject.SetActive(true);
        _buttonLight.color = _buttonLightOnColor;
        _screenParent.gameObject.SetActive(true);
    }

    private void ComputerTurnOffAudio()
    {
        _computerNoiseBackgroundAudio.StopSound();
        _computerNoiseShutDownAudio.PlayRandomSound();
    }

    private void SetTitleArrowState(bool newState)
    {
        foreach (Image arrow in _titleArrows)
        {
            arrow.gameObject.SetActive(newState);
        }
    }
}
