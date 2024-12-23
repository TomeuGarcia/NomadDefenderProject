using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MachineDisplayMovement : MachineMovablePart
{
    [Header("REFERENCES")]
    [SerializeField] private Transform[] _arm0Pivot;
    [SerializeField] private Transform[] _arm1Pivot;
    [SerializeField] private Transform _screenPivot;

    [Header("PARAMETERS")]
    [SerializeField] private float _arm0PivotStartRotation;
    [SerializeField] private float _arm0PivotStartRotationDuration;
    [SerializeField] private Ease _arm0PivotStartRotationEase;
    [SerializeField] private float _delay0;
    [SerializeField] private float _arm1PivotStartRotation;
    [SerializeField] private float _arm1PivotStartRotationDuration;
    [SerializeField] private Ease _arm1PivotStartRotationEase;
    [SerializeField] private float _delay1;
    [SerializeField] private float _screenPivotStartRotation;
    [SerializeField] private float _screenPivotStartRotationDuration;
    [SerializeField] private Ease _screenPivotStartRotationEase;
    [SerializeField] private float _delay2;
    private float _arm0PivotEndRotation;
    private float _arm1PivotEndRotation;
    private float _screenPivotEndRotation;
    private float _screenPivotEndRotationDiff;

    [SerializeField] private float _startScreenZRot;
    [SerializeField] private float _screenZRotDuration;
    [SerializeField] private Ease _screenZRotEase;

    public override void Init()
    {
        _screenPivot.SetParent(_arm1Pivot[0]);

        _arm0PivotEndRotation = _arm0Pivot[0].localRotation.eulerAngles.x;
        _arm1PivotEndRotation = _arm1Pivot[0].localRotation.eulerAngles.x;
        _screenPivotEndRotation = _screenPivot.localRotation.eulerAngles.x;
        _screenPivotEndRotationDiff = _screenPivotStartRotation - _screenPivotEndRotation;

        for (int i = 0; i < _arm0Pivot.Length; i++)
        {
            _arm0Pivot[i].localRotation = Quaternion.Euler(_arm0PivotStartRotation, 0, 0);
        }
        for (int i = 0; i < _arm1Pivot.Length; i++)
        {
            _arm1Pivot[i].localRotation = Quaternion.Euler(_arm1PivotStartRotation, 0, 0);
        }
        _screenPivot.localRotation = Quaternion.Euler(_arm0PivotStartRotation, 0, _startScreenZRot);
    }

    public override IEnumerator EnterAnimation()
    {
        for (int i = 0; i < _arm1Pivot.Length; i++)
        {
            _arm1Pivot[i].DOLocalRotate(Vector3.right * _arm1PivotEndRotation, _arm1PivotStartRotationDuration).SetEase(_arm1PivotStartRotationEase);
        }
        yield return new WaitForSeconds(_delay1);

        for (int i = 0; i < _arm0Pivot.Length; i++)
        {
            _arm0Pivot[i].DOLocalRotate(Vector3.right * _arm0PivotEndRotation, _arm0PivotStartRotationDuration).SetEase(_arm0PivotStartRotationEase);
        }
        yield return new WaitForSeconds(_delay0);

        _screenPivot.DOBlendableLocalRotateBy(Vector3.right * _screenPivotEndRotationDiff, _screenPivotStartRotationDuration).SetEase(_screenPivotStartRotationEase);
        //_screenPivot.DOLocalRotate(Vector3.right * _screenPivotEndRotation, _screenPivotStartRotationDuration).SetEase(_screenPivotStartRotationEase);
        yield return new WaitForSeconds(_delay2);

        _screenPivot.DOBlendableRotateBy(Vector3.back * _startScreenZRot, _screenZRotDuration).SetEase(_screenZRotEase);
    }

    public override IEnumerator ExitAnimation()
    {
        yield return null;
    }
}
