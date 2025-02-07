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
        [SerializeField] private MeshRenderer _mesh;
        private Material _material;

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
            _material = _mesh.material;
            _canBeSelectedTransformDefaultScale = _canBeSelectedTransform.localScale;
            SetDisabled();
        }


        public void SetEnabled()
        {
            _isEnabled = true;
            //_viewHolder.SetActive(true);
            _material.DOFloat(1.0f, "_Enabled", 0.25f).SetEase(Ease.OutCubic);

            StopWaitingForSelectedAnimation();
            PlayWaitingForSelectedAnimation();
        }
        public void SetDisabled()
        {
            _isEnabled = false;
            _material.DOFloat(0.0f, "_Enabled", 0.25f).SetEase(Ease.OutCubic);

            SetNotSelected();
        }
        
        public void SetFinalDisabled()
        {
            _isEnabled = false;
            _material.DOVector(new Vector2(2.0f, 0.0f), "_RectScale", 0.25f).SetEase(Ease.OutCubic)
                .OnComplete(() => { _material.SetFloat("_Alpha", 0.0f); });
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
            GameAudioManager.GetInstance().PlayCardInfoHidden();

            SetHighlighted(true);
            StopWaitingForSelectedAnimation();
        }
        private void OnUnhover()
        {
            if (!_isEnabled) return;
            GameAudioManager.GetInstance().PlayCardInfoHidden();

            SetHighlighted(false);
            PlayWaitingForSelectedAnimation();
        }
        private void OnPressed()
        {
            if (!_isEnabled) return;
            GameAudioManager.GetInstance().PlayCardInfoHidden();

            SetSelected();
            OnClicked?.Invoke(this);
            StopWaitingForSelectedAnimation();
        }


        private void SetHighlighted(bool highlighted)
        {
            _selectedMark.gameObject.SetActive(highlighted);

            _material.SetFloat("_Hovered", highlighted ? 1.0f : 0.0f);
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