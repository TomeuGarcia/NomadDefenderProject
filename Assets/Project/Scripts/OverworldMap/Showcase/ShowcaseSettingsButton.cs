using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OWmapShowcase
{


    public class ShowcaseSettingsButton : MonoBehaviour
    {
        [Header("VALUES")]
        [SerializeField] private float _maxValue = 0;
        [SerializeField] private float _minValue = 0;
        [SerializeField] private float _step = 1;
        private float _value;

        [Header("COMPONENTS")]
        [SerializeField] private TextMeshProUGUI _valueText;
        [SerializeField] private Button _addButton;
        [SerializeField] private Button _subtractButton;


        public Action<float> OnValueUpdated;


        private void OnEnable()
        {
            ShowcaseHUD.OnGenerationStart += DisableButtons;
            ShowcaseHUD.OnGenerationFinish += UpdateButtons;
        }
        private void OnDisable()
        {
            ShowcaseHUD.OnGenerationStart -= DisableButtons;
            ShowcaseHUD.OnGenerationFinish -= UpdateButtons;
        }


        public void Init(float value)
        {
            _value = value;

            _addButton.onClick.AddListener(AddStepToValue);
            _subtractButton.onClick.AddListener(SubtractStepToValue);

            DoUpdate();
        }

        private void AddStepToValue()
        {
            _value += _step;
            _value = Mathf.Min(_value, _maxValue);

            DoUpdate();
        }
        private void SubtractStepToValue()
        {
            _value -= _step;
            _value = Mathf.Max(_value, _minValue);

            DoUpdate();
        }


        private void UpdateButtons()
        {
            bool canAdd = _value < _maxValue;
            _addButton.enabled = canAdd;
            _addButton.image.color = canAdd ? Color.white : Color.gray;

            bool canSubtract = _value > _minValue;
            _subtractButton.enabled = canSubtract;
            _subtractButton.image.color = canSubtract ? Color.white : Color.gray;
        }

        private void UpdateText()
        {
            _valueText.text = _value.ToString();
        }

        private void DoUpdate()
        {
            UpdateButtons();
            UpdateText();
            OnValueUpdated?.Invoke(_value);
        }


        private void DisableButtons(bool isInstantly)
        {
            if (isInstantly) return;

            _addButton.enabled = false;
            _addButton.image.color = Color.gray;

            _subtractButton.enabled = false;
            _subtractButton.image.color = Color.gray;
        }


    }


}