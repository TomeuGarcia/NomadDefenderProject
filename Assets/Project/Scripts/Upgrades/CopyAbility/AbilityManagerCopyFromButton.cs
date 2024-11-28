using System;
using DG.Tweening;
using UnityEngine;

namespace Project.Scripts.Upgrades.CopyAbility
{
    public class AbilityManagerCopyFromButton : MonoBehaviour
    {
        [SerializeField] private MouseOverNotifier _mouseOverNotifier;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private GameObject _selectedMark;

        private bool _isSelected;
        private bool _isEnabled;
        
        public ATurretPassiveAbilityDataModel AbilityDataModel { get; private set; }


        public Action<AbilityManagerCopyFromButton> OnClicked;


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


        public void SetEnabled(ATurretPassiveAbilityDataModel abilityDataModel)
        {
            AbilityDataModel = abilityDataModel;

            _spriteRenderer.sprite = AbilityDataModel.View.Sprite;
            _spriteRenderer.color = AbilityDataModel.View.Color;
            
            _spriteRenderer.gameObject.SetActive(true);

            _isEnabled = true;
        }


        public void SetDisabled()
        {
            _spriteRenderer.gameObject.SetActive(false);
            SetNotSelected();
            _isEnabled = false;
        }

        public void SetFinalDisabled()
        {
            SetDisabled();
            // Extra button animation would go here
        }


        public void SetSelected()
        {
            _isSelected = true;
            SetHighlighted(true);
            PlaySelectedAnimation();
        }
        public void SetNotSelected()
        {
            _isSelected = false;
            SetHighlighted(false);
        }



        private void OnHover()
        {
            if (_isSelected || !_isEnabled) return;
            
            SetHighlighted(true);
        }
        private void OnUnhover()
        {
            if (_isSelected || !_isEnabled) return;

            SetHighlighted(false);
        }
        private void OnPressed()
        {
            if (_isSelected || !_isEnabled) return;
            
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