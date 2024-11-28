using System;
using DG.Tweening;
using UnityEngine;

namespace Project.Scripts.Upgrades.CopyAbility
{
    public class AbilityManagerConfirmButton : MonoBehaviour
    {
        [SerializeField] private GameObject _viewHolder;
        [SerializeField] private MouseOverNotifier _mouseOverNotifier;
        [SerializeField] private GameObject _selectedMark;

        private bool _isEnabled;
        

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
            SetDisabled();
        }


        public void SetEnabled()
        {
            _isEnabled = true;
            _viewHolder.SetActive(true);
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
        }
        private void SetNotSelected()
        {
            SetHighlighted(false);
        }



        private void OnHover()
        {
            if (!_isEnabled) return;
            
            SetHighlighted(true);
        }
        private void OnUnhover()
        {
            if (!_isEnabled) return;

            SetHighlighted(false);
        }
        private void OnPressed()
        {
            if (!_isEnabled) return;

            SetSelected();
            OnClicked?.Invoke(this);
        }


        private void SetHighlighted(bool highlighted)
        {
            _selectedMark.gameObject.SetActive(highlighted);
        }

        private void PlaySelectedAnimation()
        {
            _selectedMark.transform.DOPunchScale(Vector3.one * 0.1f, 0.5f, 5)
                .SetEase(Ease.OutSine);
        }
    }
}