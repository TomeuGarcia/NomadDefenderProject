using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental;
using UnityEngine;

public class FIHangingObject : AFacilityInteractable
{
    [Header("REFERENCES")]
    [SerializeField] private Transform _transformForceA;
    [SerializeField] private Transform _transformForceB;
    [SerializeField] private RandomSoundsCollection _lightHangSound;

    [Header("PARAMETERS")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Vector2 _strenghtRange;
    [SerializeField] private float _soundVelocityThreshold;
    [SerializeField] private float _minSwingVelocityMag;
    [SerializeField] private float _timeToMuteHang;
    [SerializeField] private AnimationCurve _hangVolumeCurve;

    private float _timeSinceLastInteraction;
    private bool _interacted;
    private bool _onSwing;
    private Vector3 _positionDifference;

    protected override void DoAwake()
    {
        _timeSinceLastInteraction = 0.0f;
        _interacted = false;
        _onSwing = false;
        _positionDifference = _transformForceB.position - _transformForceA.position;
    }

    protected override bool ExtraInteractionConditions()
    {
        return _manager.IsMultiSocketOn;
    }

    protected override IEnumerator DoInteract()
    {
        _interacted = true;
        _lightHangSound.PlayRandomSoundWithVolumeCoef(1.0f);
        float randomCoef = Random.Range(0.0f, 1.0f);
        _rb.AddForceAtPosition(new Vector3(0, Random.Range(_strenghtRange.x, _strenghtRange.y), 0), _transformForceA.position + _positionDifference * randomCoef, ForceMode.Impulse);

        yield return _duration;
    }

    private void Update()
    {
        if(!_onSwing && _rb.velocity.magnitude > _minSwingVelocityMag)
        {
            _onSwing = true;

            _timeSinceLastInteraction = Mathf.Clamp(_timeSinceLastInteraction, 0.0f, _timeToMuteHang);
            float tParam = _hangVolumeCurve.Evaluate(_timeSinceLastInteraction / _timeToMuteHang);
            _lightHangSound.PlayRandomSoundWithVolumeCoef(tParam);
        }
        else if(_onSwing && _rb.velocity.magnitude < _minSwingVelocityMag)
        {
            _onSwing = false;
        }

        if(_interacted)
        {
            _interacted = false;
            _timeSinceLastInteraction = _timeToMuteHang;
        }

        _timeSinceLastInteraction -= Time.deltaTime;
    }
}
