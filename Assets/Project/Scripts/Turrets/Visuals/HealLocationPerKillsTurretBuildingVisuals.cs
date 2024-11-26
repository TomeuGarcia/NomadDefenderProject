using System;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Turrets.Visuals
{
    public class HealLocationPerKillsTurretBuildingVisuals : MonoBehaviour
    {
        [Header("COMPONENTS")]
        [SerializeField] private TMP_Text _currentKillsText;
        [SerializeField] private Image _killsProgressImage;
        [SerializeField] private Transform _textsHolder;

        [Header("ANIMATIONS")] 
        [SerializeField] private TweenPunchConfig _updateKillsCountScalePunch; 
        [SerializeField] private TweenPunchConfig _updateKillsCountRotationPunch; 
        [SerializeField] private TweenPunchConfig _almostReachedKillsCountRotationPunch; 
        [SerializeField] private TweenColorConfig _updateKillsCountColorPunch; 
        [SerializeField] private TweenPunchConfig _updateMaxKillsScalePunch;


        private int _currentKills;
        private int _maxKills;
        private bool _currentValueUpdated = false;
        
        public void Init(int currentKills, int maxKills)
        {
            UpdateCurrentKillsText(currentKills);
            UpdateMaxKillsText(maxKills);
        }

        public void UpdateCurrentKills(int currentKills)
        {
            UpdateCurrentKillsText(currentKills);
            _currentKillsText.transform.PunchScale(_updateKillsCountScalePunch, true);
            _currentKillsText.transform.PunchRotation(_updateKillsCountRotationPunch);

            _currentKillsText.DOComplete();
            _currentKillsText.color = Color.white;
            _currentKillsText.PunchColor(_updateKillsCountColorPunch);
            
            if (_currentKills == _maxKills - 3)
            {
                _textsHolder.PunchRotation(_almostReachedKillsCountRotationPunch).SetLoops(-1);
            }
        }

        public async void DelayedCompleteKills(int newMaxKills, float delay)
        {
            _textsHolder.DOKill();
            _textsHolder.localRotation = Quaternion.identity;
            _textsHolder.localScale = Vector3.one;
            _currentValueUpdated = false;
            
            await Task.Delay(TimeSpan.FromSeconds(delay));

            UpdateMaxKillsText(newMaxKills);
            await _textsHolder.transform.PunchScale(_updateMaxKillsScalePunch).AsyncWaitForCompletion();
            if (!_currentValueUpdated)
            {
                UpdateCurrentKillsText(0);
            }
        }

        private void UpdateCurrentKillsText(int currentKills)
        {
            _currentKills = currentKills;
            _currentKillsText.text = currentKills.ToString();
            _currentValueUpdated = true;
            UpdateProgress();
        }
        private void UpdateMaxKillsText(int maxKills)
        {
            _maxKills = maxKills;
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            _killsProgressImage.fillAmount = (float)_currentKills / _maxKills;
        }
    }
}