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

        private Vector3 _spriteDefaultScale;
        private Vector3 _selectionDefaultScale;
        
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
            _spriteDefaultScale = _spriteRenderer.transform.localScale;
            _selectionDefaultScale = _selectedMark.transform.localScale;
            SetDisabled();
        }


        public void SetEnabled(ATurretPassiveAbilityDataModel abilityDataModel)
        {
            AbilityDataModel = abilityDataModel;

            _spriteRenderer.sprite = AbilityDataModel.View.Sprite;
            _spriteRenderer.color = AbilityDataModel.View.Color;
            
            _spriteRenderer.gameObject.SetActive(true);

            _isEnabled = true;
            
            PlayWaitingForSelectedAnimation();
        }


        public void SetDisabled()
        {
            _spriteRenderer.gameObject.SetActive(false);
            SetNotSelected();
            _isEnabled = false;
            StopWaitingForSelectedAnimation();
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
            _selectedMark.transform.localScale = _selectionDefaultScale;
            StopWaitingForSelectedAnimation();
        }



        private void OnHover()
        {
            if (_isSelected || !_isEnabled) return;
            
            SetHighlighted(true);
            StopWaitingForSelectedAnimation();
        }
        private void OnUnhover()
        {
            if (_isSelected || !_isEnabled) return;

            SetHighlighted(false);
            PlayWaitingForSelectedAnimation();
        }
        private void OnPressed()
        {
            if (_isSelected || !_isEnabled) return;
            
            OnClicked?.Invoke(this);
            StopWaitingForSelectedAnimation();
        }


        private void SetHighlighted(bool highlighted)
        {
            _selectedMark.gameObject.SetActive(highlighted);
        }

        private void PlayWaitingForSelectedAnimation()
        {
            Vector3 scaleChange = _spriteDefaultScale * 0.2f;

            _spriteRenderer.transform.DOBlendableScaleBy(-scaleChange, 0.75f)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                    _spriteRenderer.transform.DOBlendableScaleBy(scaleChange, 0.75f)
                        .SetEase(Ease.InOutSine)
                        .OnComplete(PlayWaitingForSelectedAnimation)
                );
        }

        private void StopWaitingForSelectedAnimation()
        {
            _spriteRenderer.transform.DOKill();
            _spriteRenderer.transform.localScale = _spriteDefaultScale;
        }
        
        private void PlaySelectedAnimation()
        {
            _selectedMark.transform.localScale = _selectionDefaultScale + (Vector3.one * 0.1f);
            _selectedMark.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 5)
                .SetEase(Ease.OutSine);
        }
    }
}