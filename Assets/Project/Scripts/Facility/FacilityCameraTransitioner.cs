using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FacilityCameraTransitioner : AFacilityInteractable
{
    [Header("REFERENCES")]
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private CinemachineVirtualCamera _vcam;
    [SerializeField] private FacilityUITransitioner _facilityUITransitioner;

    [Header("PARAMETERS")]
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

    protected override IEnumerator DoInteract()
    {
        if(_manager.IsPCOn)
        {
            TransitionToComputer();

            yield return new WaitForSeconds(_duration);
        }
    }

    public void TransitionToComputer()
    {
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
        mySequence.OnComplete(_facilityUITransitioner.StartLoading);
    }
}
