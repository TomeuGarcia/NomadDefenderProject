using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using TMPro;

public class FacilityCameraTransitioner : AFacilityInteractable
{
    [Header("REFERENCES")]
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private CinemachineVirtualCamera _vcam;
    [SerializeField] private FacilityUITransitioner _facilityUITransitioner;
    [SerializeField] private List<Image> _titleArrows = new();
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private Collider _interactableCollider;

    [Header("PARAMETERS")]
    [SerializeField] private Color32 _hoveredTextColor;
    [Space]
    [SerializeField] private float _horizontalTime;
    [SerializeField] private float _verticalTime;
    [SerializeField] private float _rotationTime;
    [SerializeField] private float _cameraNoiseTime;
    [Space]
    [SerializeField] private Vector3 _rotationGoal;
    [SerializeField] private Vector3 _positionGoal;
    [Space]
    [SerializeField] private Ease _horizontalEase;
    [SerializeField] private Ease _verticalEase;
    [SerializeField] private Ease _rotationEase;

    private bool _hasInteracted = false;
    private Color32 _defaultTextColor;

    private RunInfo _runInfo;

    protected override void DoAwake()
    {
        _defaultTextColor = _titleText.color;

        _runInfo = ServiceLocator.GetInstance().RunInfo;
    }

    protected override bool ExtraInteractionConditions()
    {
        return _manager.IsPCOn && !_hasInteracted;
    }

    protected override IEnumerator DoInteract()
    {
        _hasInteracted = true;
        Destroy(_interactableCollider.gameObject.GetComponent<PointAndClickClickableObject>());
        TransitionToComputer();

        yield return new WaitForSeconds(_duration);
    }

    public void TransitionToComputer()
    {
        _facilityUITransitioner.PrepareLoading();

        DOTween.To(() => _vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain,
            x => _vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = x,
            0.0f,
            _cameraNoiseTime
        );
        DOTween.To(() => _vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain,
            x => _vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = x,
            0.0f,
            _cameraNoiseTime
        );

        Sequence mySequence = DOTween.Sequence();
        mySequence.Insert(0.0f, _targetTransform.DOMoveZ(_positionGoal.z, _horizontalTime).SetEase(_horizontalEase));
        mySequence.Insert(0.0f, _targetTransform.DOMoveY(_positionGoal.y, _verticalTime).SetEase(_verticalEase));
        mySequence.Insert(0.0f, _targetTransform.DORotate(_rotationGoal, _rotationTime).SetEase(_rotationEase));

        if(_runInfo.IsNewGame) {
            mySequence.OnComplete(_facilityUITransitioner.StartLoading);
        } else {
            mySequence.OnComplete(_facilityUITransitioner.TransitionToNextScene);
        }
    }

    public override void Hovered()
    {
        _titleText.color = _hoveredTextColor;

        foreach (Image arrow in _titleArrows)
        {
            arrow.color = _hoveredTextColor;
        }

        GameAudioManager.GetInstance().PlayCardInfoShown();
    }

    public override void Unhovered()
    {
        _titleText.color = _defaultTextColor;

        foreach (Image arrow in _titleArrows)
        {
            arrow.color = _defaultTextColor;
        }

        GameAudioManager.GetInstance().PlayCardInfoHidden();
    }
}
