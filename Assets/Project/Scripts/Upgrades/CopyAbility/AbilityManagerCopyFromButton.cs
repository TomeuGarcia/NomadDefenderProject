using System;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

namespace Project.Scripts.Upgrades.CopyAbility
{
    public class AbilityManagerCopyFromButton : MonoBehaviour
    {
        [SerializeField] private MouseOverNotifier _mouseOverNotifier;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private GameObject _selectedMark;
        [SerializeField] private MeshRenderer _mesh;
        private Material _material;

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
            _material = _mesh.material;
            //_spriteDefaultScale = _spriteRenderer.transform.localScale;
            _selectionDefaultScale = _selectedMark.transform.localScale;
            SetDisabled();
        }


        public void SetEnabled(ATurretPassiveAbilityDataModel abilityDataModel)
        {
            //Debug.Log("SetEnabled");
            AbilityDataModel = abilityDataModel;

            _material.DOFloat(1.0f, "_EnableCoef", 0.25f).SetEase(Ease.OutCubic);
            _material.SetTexture("_Texture", AbilityDataModel.View.Sprite.texture);
            _material.SetColor("_InnerColor", AbilityDataModel.View.Color);
            
            //_spriteRenderer.gameObject.SetActive(true);

            _isEnabled = true;
            
            PlayWaitingForSelectedAnimation();
        }


        public void SetDisabled()
        {
            //Debug.Log("SetDisabled");
            _material.DOFloat(0.0f, "_EnableCoef", 0.25f).SetEase(Ease.InCubic);
            //_material.SetFloat("_Enabled", 0f);
            //_spriteRenderer.gameObject.SetActive(false);
            SetNotSelected();
            _isEnabled = false;
            StopWaitingForSelectedAnimation();
        }

        public void SetFinalDisabled()
        {
            //Debug.Log("SetFinalDisabled");
            SetDisabled();

            _material.DOFloat(0.0f, "_AlphaCoef", 0.25f).SetEase(Ease.OutCubic);
        }


        public void SetSelected()
        {
            //Debug.Log("SetSelected");
            _isSelected = true;
            SetSelectedView(true);
            PlaySelectedAnimation();
        }
        public void SetNotSelected()
        {
            //Debug.Log("SetNotSelected");
            _isSelected = false;
            SetSelectedView(false);
            SetHoveredView(false);
            _selectedMark.transform.localScale = _selectionDefaultScale;
            StopWaitingForSelectedAnimation();
        }



        private void OnHover()
        {
            if (_isSelected || !_isEnabled) return;
            GameAudioManager.GetInstance().PlayCardInfoHidden();

            SetHoveredView(true);
            StopWaitingForSelectedAnimation();
        }
        private void OnUnhover()
        {
            if (_isSelected || !_isEnabled) return;
            GameAudioManager.GetInstance().PlayCardInfoHidden();

            SetHoveredView(false);
            PlayWaitingForSelectedAnimation();
        }
        private void OnPressed()
        {
            if (_isSelected || !_isEnabled) return;
            GameAudioManager.GetInstance().PlayCardInfoHidden();

            OnClicked?.Invoke(this);
            StopWaitingForSelectedAnimation();
        }


        private void SetSelectedView(bool selected)
        {
            _material.DOFloat(selected ? 1.0f : 0.0f, "_SelectCoef", 0.2f).SetEase(Ease.OutCubic);
        }

        private void SetHoveredView(bool highlighted)
        {
            //_material.SetFloat("_HoverCoef", highlighted ? 1.0f : 0.0f);
            
            _material.DOComplete();
            _material.DOFloat(highlighted ? 1.0f : 0.0f, "_HoverCoef", 0.1f);
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