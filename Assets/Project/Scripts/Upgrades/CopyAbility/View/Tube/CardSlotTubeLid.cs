using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardSlotTubeLid : MachineMovablePart
{
    [Header("REFERENCES")]
    [SerializeField] private Transform _lid;
    [SerializeField] private bool _hasButtons;
    [SerializeField, ShowIf("_hasButtons")] private float _delayUntilButtons;
    [SerializeField, ShowIf("_hasButtons")] private Transform _lidButtons;
    [SerializeField, ShowIf("_hasButtons")] private RotateMode _rotationMode;
    private List<MeshRenderer> _buttonMeshes = new();
    private List<Material> _buttonMaterials = new();

    [Header("PARAMETERS")]
    [SerializeField] private TweenConfig _spinConfig;
    [SerializeField, ShowIf("_hasButtons")] private TweenConfig _buttonsTweenConfig;
    [SerializeField, ShowIf("_hasButtons")] private TweenConfig _buttonDisappearConfig;

    public override void Init()
    {
        if(!_hasButtons)
            return;

        foreach (Transform child in _lidButtons)
        {
            _buttonMeshes.Add(child.GetComponent<MeshRenderer>());
        }
        foreach (MeshRenderer mesh in _buttonMeshes)
        {
            _buttonMaterials.Add(mesh.material);
        }
    }

    public override IEnumerator EnterAnimation()
    {
        Spin();
        if(_hasButtons)
        {
            yield return new WaitForSeconds(_delayUntilButtons);
            _lidButtons.DOLocalRotate(_buttonsTweenConfig.Value, _buttonsTweenConfig.Duration, _rotationMode).SetEase(_buttonsTweenConfig.Ease);
        }
    }

    public void Spin(int spinCount = 2)
    {
        _lid.DOBlendableRotateBy(_spinConfig.Value * spinCount, _spinConfig.Duration, RotateMode.FastBeyond360).SetEase(_spinConfig.Ease);
    }

    public IEnumerator ReplaceAnimation()
    {
        if(_hasButtons)
        {
            foreach (Material material in _buttonMaterials)
            {
                material.DOVector(_buttonDisappearConfig.Value, "_RectScale", _buttonDisappearConfig.Duration).SetEase(_buttonDisappearConfig.Ease).OnComplete(() => material.SetFloat("_Alpha", 0.0f));
            }
        }
        yield return new WaitForSeconds(_buttonDisappearConfig.Duration);
        Spin(-2);
    }

    public override IEnumerator ExitAnimation()
    {
        yield return null;
    }
}
