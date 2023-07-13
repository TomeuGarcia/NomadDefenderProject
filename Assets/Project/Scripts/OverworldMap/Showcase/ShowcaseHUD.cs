using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OWmapShowcase
{

    public class ShowcaseHUD : MonoBehaviour
    {
        private Showcase_OWMapGameManager _manager;
        private OWMapGenerationSettings _generationSettings;

        [SerializeField] private Button _startButton;



        [SerializeField] private ShowcaseSettingsButton _numberOfLevelsSB;

        [SerializeField] private ShowcaseSettingsButton _maxWidthGrowStepSB;
        [SerializeField] private ShowcaseSettingsButton _maxWidthShrinkStepSB;
        [SerializeField] private ShowcaseSettingsButton _maxWidthSB;

        [SerializeField] private ShowcaseSettingsButton _max1WideLevelsSB;
        [SerializeField] private ShowcaseSettingsButton _maxRepeatedSameWidthSB;

        [SerializeField] private ShowcaseSettingsButton _removeConnectionPercentageSB;



        static public Action OnGenerationStart;
        static public Action OnGenerationFinish;


        public void AwakeInit(Showcase_OWMapGameManager manager, OWMapGenerationSettings generationSettings)
        {
            _manager = manager;
            _generationSettings = generationSettings;

            _startButton.onClick.AddListener(OnStartButtonPressed);
        }

        void Start()
        {
            _numberOfLevelsSB.Init(_generationSettings.numberOfLevels);

            _maxWidthGrowStepSB.Init(_generationSettings.maxWidthGrowStep);
            _maxWidthShrinkStepSB.Init(_generationSettings.maxWidthShrinkStep);
            _maxWidthSB.Init(_generationSettings.maxWidth);

            _max1WideLevelsSB.Init(_generationSettings.maxNum1Width);
            _maxRepeatedSameWidthSB.Init(_generationSettings.maxSameWidthRepeatTimes);

            _removeConnectionPercentageSB.Init(_generationSettings.removeConnectionThreshold * 100.0f);
        }

        private void OnEnable()
        {
            _numberOfLevelsSB.OnValueUpdated += UpdateNumberOfLevels;

            _maxWidthGrowStepSB.OnValueUpdated += UpdateMaxWidthGrowStep;
            _maxWidthShrinkStepSB.OnValueUpdated += UpdateMaxWidthShrinkStep;
            _maxWidthSB.OnValueUpdated += UpdateMaxWidth;

            _max1WideLevelsSB.OnValueUpdated += UpdateMaxNum1Width;
            _maxRepeatedSameWidthSB.OnValueUpdated += UpdateMaxRepeatedSameWidth;

            _removeConnectionPercentageSB.OnValueUpdated += UpdateRemoveConnectionPercentage;
        }

        private void OnDisable()
        {
            _numberOfLevelsSB.OnValueUpdated -= UpdateNumberOfLevels;

            _maxWidthGrowStepSB.OnValueUpdated -= UpdateMaxWidthGrowStep;
            _maxWidthShrinkStepSB.OnValueUpdated -= UpdateMaxWidthShrinkStep;
            _maxWidthSB.OnValueUpdated -= UpdateMaxWidth;

            _max1WideLevelsSB.OnValueUpdated -= UpdateMaxNum1Width;
            _maxRepeatedSameWidthSB.OnValueUpdated -= UpdateMaxRepeatedSameWidth;

            _removeConnectionPercentageSB.OnValueUpdated -= UpdateRemoveConnectionPercentage;
        }


        private void OnStartButtonPressed()
        {
            _startButton.gameObject.SetActive(false);

            _generationSettings.FixParameters();
            _manager.ResetMap();

            OnGenerationStart?.Invoke();
        }
        public void EnableStartButton()
        {
            _startButton.gameObject.SetActive(true);

            OnGenerationFinish?.Invoke();
        }


        private void UpdateNumberOfLevels(float value)
        {
            _generationSettings.numberOfLevels = (int)value;
        }
        private void UpdateMaxWidthGrowStep(float value)
        {
            _generationSettings.maxWidthGrowStep = (int)value;
        }
        private void UpdateMaxWidthShrinkStep(float value)
        {
            _generationSettings.maxWidthShrinkStep = (int)value;
        }
        private void UpdateMaxWidth(float value)
        {
            _generationSettings.maxWidth = (int)value;
        }
        private void UpdateMaxNum1Width(float value)
        {
            _generationSettings.maxNum1Width = (int)value;
        }
        private void UpdateMaxRepeatedSameWidth(float value)
        {
            _generationSettings.maxSameWidthRepeatTimes = (int)value;
        }
        private void UpdateRemoveConnectionPercentage(float value)
        {
            _generationSettings.removeConnectionThreshold = value / 100.0f;
        }


    }

}