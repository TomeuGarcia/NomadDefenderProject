using System;
using DG.Tweening;
using UnityEngine;

namespace Project.Scripts.Upgrades.CopyAbility
{
    public class AbilityManagerConfirmButton : MonoBehaviour
    {
        [SerializeField] private GameObject _viewHolder;
        [SerializeField] private MouseOverNotifier _mouseOverNotifier;
        [SerializeField] private Transform _canBeSelectedTransform;
        [SerializeField] private GameObject _selectedMark;

        private bool _isEnabled;
        private Vector3 _canBeSelectedTransformDefaultScale;

        public Action<AbilityManagerConfirmButton> OnClicked;


        private void OnEnable()
        {
            _mouseOverNotifier.OnMouseEntered += OnHover;
            _mouseOverNotifier.OnMouseExited += OnUnhover;
            _mouseOverNotifier.OnMousePressed += OnPressed;
        }
        private void OnDisable()
        {
            _mouseOverNotifier.OnMouseEntered -= OnHover;
            _mouseOverNotifier.OnMouseExited -= OnUnhover;
            _mouseOverNotifier.OnMousePressed -= OnPressed;
        }

        private void Awake()
        {
            _canBeSelectedTransformDefaultScale = _canBeSelectedTransform.localScale;
            SetDisabled();
        }


        public void SetEnabled()
        {
            _isEnabled = true;
            _viewHolder.SetActive(true);
            PlayWaitingForSelectedAnimation();
        }
        public void SetDisabled()
        {
            _isEnabled = false;
            _viewHolder.SetActive(false);
            SetNotSelected();
        }
        
        public void SetFinalDisabled()
        {
            _isEnabled = false;
            // Extra button animation would go here
        }
        
        
        private void SetSelected()
        {
            SetHighlighted(true);
            PlaySelectedAnimation();
            StopWaitingForSelectedAnimation();
        }
        private void SetNotSelected()
        {
            SetHighlighted(false);
            StopWaitingForSelectedAnimation();
        }



        private void OnHover()
        {
            if (!_isEnabled) return;

            SetHighlighted(true);
            StopWaitingForSelectedAnimation();
        }
        private void OnUnhover()
        {
            if (!_isEnabled) return;

            SetHighlighted(false);
            PlayWaitingForSelectedAnimation();
        }
        private void OnPressed()
        {
            if (!_isEnabled) return;

            SetSelected();
            OnClicked?.Invoke(this);
            StopWaitingForSelectedAnimation();
        }


        private void SetHighlighted(bool highlighted)
        {
            _selectedMark.gameObject.SetActive(highlighted);
        }

        
        
        
        private void PlayWaitingForSelectedAnimation()
        {
            Vector3 scaleChange = _canBeSelectedTransformDefaultScale * 0.1f;

            _canBeSelectedTransform.DOBlendableScaleBy(-scaleChange, 0.75f)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                    _canBeSelectedTransform.DOBlendableScaleBy(scaleChange, 0.75f)
                        .SetEase(Ease.InOutSine)
                        .OnComplete(PlayWaitingForSelectedAnimation)
                );
        }

        private void StopWaitingForSelectedAnimation()
        {
            _canBeSelectedTransform.DOKill();
            _canBeSelectedTransform.localScale = _canBeSelectedTransformDefaultScale;
        }
        
        private void PlaySelectedAnimation()
        {
            _selectedMark.transform.DOPunchScale(Vector3.one * 0.1f, 0.5f, 5)
                .SetEase(Ease.OutSine);
        }
    }
}