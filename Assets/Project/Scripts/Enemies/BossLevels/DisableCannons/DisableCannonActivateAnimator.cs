using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class DisableCannonActivateAnimator : MonoBehaviour
{
    [System.Serializable]
    private class CannonPart
    {
        [System.Serializable]
        private class Transition
        {
            [SerializeField, Min(0)] private float _startDelay;
            [SerializeField, Min(0)] private int _blinkTimes;
            [SerializeField, Min(0)] private float _delayBetweenBlinks;
            [SerializeField] private Material[] _materials;

            public void ApplyMaterials(MeshRenderer meshRenderer)
            {
                meshRenderer.materials = _materials;
            }
            
            public IEnumerator PlayTransition(MeshRenderer meshRenderer, Transition otherTransition)
            {
                otherTransition.ApplyMaterials(meshRenderer);
                yield return new WaitForSeconds(_startDelay);
                
                for (int i = 0; i < _blinkTimes; ++i)
                {
                    ApplyMaterials(meshRenderer);
                    yield return new WaitForSeconds(_delayBetweenBlinks);
                    otherTransition.ApplyMaterials(meshRenderer);
                    yield return new WaitForSeconds(_delayBetweenBlinks);
                }
                
                ApplyMaterials(meshRenderer);
            }
        }
        
        [SerializeField] private MeshRenderer _meshRenderer;
        [Space(5)]
        [SerializeField] private Transition _activeTransition;
        [Space(5)]
        [SerializeField] private Transition _notActiveTransition;

        public void Init()
        {
            SetNotActiveInstantly();
        }
        
        public void SetActiveInstantly()
        {
            _activeTransition.ApplyMaterials(_meshRenderer);
        }
        public void SetNotActiveInstantly()
        {
            _notActiveTransition.ApplyMaterials(_meshRenderer);
        }
        
        public IEnumerator PlayActive(MonoBehaviour source)
        {
            yield return source.StartCoroutine(
                _activeTransition.PlayTransition(_meshRenderer, _notActiveTransition));
        }
        public IEnumerator PlayNotActive(MonoBehaviour source)
        {
            yield return source.StartCoroutine(
                _notActiveTransition.PlayTransition(_meshRenderer, _activeTransition));
        }

    }

    [SerializeField] private CannonPart[] _cannonParts;

    [SerializeField] private DisableCannonShootAnimator.CannonRecoilBeat[] _activeMovementBeats;
    [SerializeField] private DisableCannonShootAnimator.CannonRecoilBeat[] _notActiveMovementBeats;

    private void Awake()
    {
        foreach (CannonPart cannonPart in _cannonParts)
        {
            cannonPart.Init();
        }
        foreach (DisableCannonShootAnimator.CannonRecoilBeat activeMovementBeat in _activeMovementBeats)
        {
            activeMovementBeat.Init();
        }
        foreach (DisableCannonShootAnimator.CannonRecoilBeat notActiveMovementBeat in _notActiveMovementBeats)
        {
            notActiveMovementBeat.Init();
        }
    }


    [Button()]
    public void PlayEnterActiveAnimation()
    {
        foreach (CannonPart cannonPart in _cannonParts)
        {
            StartCoroutine(cannonPart.PlayActive(this));
        }
        foreach (DisableCannonShootAnimator.CannonRecoilBeat activeMovementBeat in _activeMovementBeats)
        {
            StartCoroutine(activeMovementBeat.PlayAnimation(this));
        }
    }
    
    [Button()]
    public void PlayEnterNotActiveAnimation()
    {
        foreach (CannonPart cannonPart in _cannonParts)
        {
            StartCoroutine(cannonPart.PlayNotActive(this));
        }
        foreach (DisableCannonShootAnimator.CannonRecoilBeat notActiveMovementBeats in _notActiveMovementBeats)
        {
            StartCoroutine(notActiveMovementBeats.PlayAnimation(this));
        }
    }
    
    
}
