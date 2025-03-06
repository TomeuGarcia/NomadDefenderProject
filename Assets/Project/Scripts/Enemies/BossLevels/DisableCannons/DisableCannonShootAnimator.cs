using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class DisableCannonShootAnimator : MonoBehaviour
{
    [System.Serializable]
    public class CannonRecoilBeat
    {
        private enum Mode { Position, Rotation }
        
        [SerializeField] private Transform _transform;
        [SerializeField] private Mode _mode = Mode.Position;
        [SerializeField, Min(0)] private float _delay = 0f;
        [SerializeField, Min(0)] private float _duration = 1f;
        [SerializeField] private Vector3 _amount;
        [SerializeField] private AnimationCurve _amountMultiplier;

        private Timer _animationTimer;
        private Vector3 _defaultLocalPosition;
        private Quaternion _defaultLocalRotation;

        private Coroutine _animationCoroutine;
        
        public void Init()
        {
            _animationTimer = new Timer(_duration);
            _defaultLocalPosition = _transform.localPosition;
            _defaultLocalRotation = _transform.localRotation;
        }

        private void CompleteState()
        {
            float t = _amountMultiplier.Evaluate(1);
            if (_mode == Mode.Position)
            {
                Vector3 currentLocalPosition = _defaultLocalPosition + (_amount * t);
                _transform.localPosition = currentLocalPosition;
            }
            else if (_mode == Mode.Rotation)
            {
                Quaternion goalLocalRotation = _defaultLocalRotation * Quaternion.Euler(_amount);
                Quaternion currentLocalRotation = Quaternion.LerpUnclamped(_defaultLocalRotation, goalLocalRotation, t);
                _transform.localRotation = currentLocalRotation;
            }
        }

        public void PlayAnimation(MonoBehaviour source)
        {
            if (_animationCoroutine != null)
            {
                source.StopCoroutine(_animationCoroutine);
                CompleteState();
            }

            _animationCoroutine = source.StartCoroutine(DoPlayAnimation(source));
        }

        public IEnumerator DoPlayAnimation(MonoBehaviour source)
        {
            switch (_mode)
            {
                case Mode.Position:
                    yield return source.StartCoroutine(PlayPositionAnimation(source));
                    break;
                case Mode.Rotation:
                    yield return source.StartCoroutine(PlayRotationAnimation(source));
                    break;
            }
        }
        public IEnumerator PlayPositionAnimation(MonoBehaviour source)
        {
            _animationTimer.Duration = _duration;
            _animationTimer.Reset();
            
            yield return source.StartCoroutine(GameTime.WaitForSeconds(_delay));

            while (!_animationTimer.HasFinished())
            {
                float offsetMultiplier = _amountMultiplier.Evaluate(_animationTimer.Ratio01);
                Vector3 currentLocalPosition = _defaultLocalPosition + (_amount * offsetMultiplier);
                    
                _transform.localPosition = currentLocalPosition;
                
                _animationTimer.Update(Time.deltaTime);
                yield return null;
            }

            _animationCoroutine = null;
        }
        public IEnumerator PlayRotationAnimation(MonoBehaviour source)
        {
            _animationTimer.Duration = _duration;
            _animationTimer.Reset();

            Quaternion goalLocalRotation = _defaultLocalRotation * Quaternion.Euler(_amount);
            
            yield return source.StartCoroutine(GameTime.WaitForSeconds(_delay));

            while (!_animationTimer.HasFinished())
            {
                float t = _amountMultiplier.Evaluate(_animationTimer.Ratio01);
                Quaternion currentLocalRotation = Quaternion.LerpUnclamped(_defaultLocalRotation, goalLocalRotation, t);
                    
                _transform.localRotation = currentLocalRotation;
                
                _animationTimer.Update(Time.deltaTime);
                yield return null;
            }
            
            _animationCoroutine = null;
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
        private Coroutine _animationCoroutine;

        public void Init()
        {
            _animationTimer = new Timer(_duration);
            _light.intensity = _intensityFade.y;
        }
        
        private void CompleteState()
        {
            float t = _intensityMultiplier.Evaluate(1);
            _light.intensity = Mathf.LerpUnclamped(_intensityFade.x, _intensityFade.y, t);
            _animationTimer.Update(Time.deltaTime);
        }

        public void PlayAnimation(MonoBehaviour source)
        {
            if (_animationCoroutine != null)
            {
                source.StopCoroutine(_animationCoroutine);
                CompleteState();
            }

            _animationCoroutine = source.StartCoroutine(DoPlayAnimation());
        }


        private IEnumerator DoPlayAnimation()
        {
            _animationTimer.Duration = _duration;
            _animationTimer.Reset();
            
            yield return new WaitForSeconds(_delay);

            while (!_animationTimer.HasFinished())
            {
                float t = _intensityMultiplier.Evaluate(_animationTimer.Ratio01);
                _light.intensity = Mathf.LerpUnclamped(_intensityFade.x, _intensityFade.y, t);

                _animationTimer.Update(Time.deltaTime);
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
    
    [Button()]
    public void PlayAnimation()
    {
        foreach (CannonRecoilBeat cannonRecoilBeat in _cannonRecoilBeats)
        {
            cannonRecoilBeat.PlayAnimation(this);
        }
        foreach (CannonLightBeat cannonLightBeat in _cannonLightBeats)
        {
            cannonLightBeat.PlayAnimation(this);
        }
    }
    
}