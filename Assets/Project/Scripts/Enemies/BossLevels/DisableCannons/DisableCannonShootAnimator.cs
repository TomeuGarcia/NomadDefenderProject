using System;
using System.Collections;
using UnityEngine;

public class DisableCannonShootAnimator : MonoBehaviour
{
    [System.Serializable]
    private class CannonRecoilBeat
    {
        [SerializeField] private Transform _transform;
        [SerializeField, Min(0)] private float _delay = 0f;
        [SerializeField, Min(0)] private float _duration = 1f;
        [SerializeField] private Vector3 _maxMoveAmount;
        [SerializeField] private AnimationCurve _amountMultiplier;

        private Timer _animationTimer;
        private Vector3 _defaultLocalPosition;

        public void Init()
        {
            _animationTimer = new Timer(_duration);
            _defaultLocalPosition = _transform.localPosition;
        }

        public IEnumerator PlayAnimation()
        {
            _animationTimer.Duration = _duration;
            _animationTimer.Reset();
            
            yield return new WaitForSeconds(_delay);

            while (!_animationTimer.HasFinished())
            {
                float offsetMultiplier = _amountMultiplier.Evaluate(_animationTimer.Ratio01);
                Vector3 currentLocalPosition = _defaultLocalPosition + (_maxMoveAmount * offsetMultiplier);
                    
                _transform.localPosition = currentLocalPosition;
                
                _animationTimer.Update(GameTime.DeltaTime);
                yield return null;
            }
        }
    }

    [System.Serializable]
    private class CannonLightBeat
    {
        [SerializeField] private Light _light;
        [SerializeField, Min(0)] private float _delay = 0f;
        [SerializeField, Min(0)] private float _duration = 1f;
        [SerializeField] private Vector2 _intensityFade = new Vector2(1f, 0.03f);
        [SerializeField] private AnimationCurve _intensityMultiplier;
        
        private Timer _animationTimer;

        public void Init()
        {
            _animationTimer = new Timer(_duration);
            _light.intensity = _intensityFade.y;
        }

        public IEnumerator PlayAnimation()
        {
            _animationTimer.Duration = _duration;
            _animationTimer.Reset();
            
            yield return new WaitForSeconds(_delay);

            while (!_animationTimer.HasFinished())
            {
                float t = _intensityMultiplier.Evaluate(_animationTimer.Ratio01);
                _light.intensity = Mathf.LerpUnclamped(_intensityFade.x, _intensityFade.y, t);

                _animationTimer.Update(GameTime.DeltaTime);
                yield return null;
            }
        }
    }


    [SerializeField] private CannonRecoilBeat[] _cannonRecoilBeats;
    [SerializeField] private CannonLightBeat[] _cannonLightBeats;
    
    
    
    
    public void Init()
    {
        foreach (CannonRecoilBeat cannonRecoilBeat in _cannonRecoilBeats)
        {
            cannonRecoilBeat.Init();
        }
        foreach (CannonLightBeat cannonLightBeat in _cannonLightBeats)
        {
            cannonLightBeat.Init();
        }
    }
    
    public void PlayAnimation()
    {
        foreach (CannonRecoilBeat cannonRecoilBeat in _cannonRecoilBeats)
        {
            StartCoroutine(cannonRecoilBeat.PlayAnimation());
        }
        foreach (CannonLightBeat cannonLightBeat in _cannonLightBeats)
        {
            StartCoroutine(cannonLightBeat.PlayAnimation());
        }
    }
    
}