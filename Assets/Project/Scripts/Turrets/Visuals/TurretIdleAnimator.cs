using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretIdleAnimator : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Transform _turretHead;

    [Header("PARAMETERS")]
    [SerializeField] private float _duration;
    [SerializeField] private float _strength;
    [SerializeField] private int _vibrato;
    [SerializeField] private float _randomness;

    private void OnEnable()
    {
        PlayAnimation();
    }

    private void OnDisable()
    {
        StopAnimation();
    }

    public void PlayAnimation()
    {
        _turretHead.DOKill();
        _turretHead.localPosition = Vector3.zero;
        _turretHead.DOShakePosition(_duration, _strength, _vibrato, _randomness, false, false, ShakeRandomnessMode.Full).SetLoops(-1);
    }

    public void StopAnimation()
    {
        _turretHead.DOKill();
        _turretHead.localPosition = Vector3.zero;
    }
}
