using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingCardMotion
{
    private Transform _effectTransform;
    private CardMotionConfig.RotationEffect _rotationEffect;
    private float _effectAmount;


    public FloatingCardMotion(Transform effectTransform, CardMotionConfig.RotationEffect rotationEffect)
    {
        _effectTransform = effectTransform;
        _rotationEffect = rotationEffect;
        _effectAmount = 0f;
    }

    public void Update()
    {
        float rotationTime = (_rotationEffect.RotationSpeed * Time.time) + _effectTransform.gameObject.GetInstanceID();
        Vector2 rotationAngles = _rotationEffect.MaxRotationAngles * _effectAmount;
        Vector3 localRotationEuler = new Vector3(
            Mathf.Sin(rotationTime) * rotationAngles.x,
            Mathf.Cos(rotationTime) * rotationAngles.y,
            0f
        );

        _effectTransform.localRotation = Quaternion.Euler(localRotationEuler);
    }


    public void Start()
    {
        _effectAmount = 0f;
    }

    public void Finish()
    {
        _effectAmount = 0f;
    }
}
