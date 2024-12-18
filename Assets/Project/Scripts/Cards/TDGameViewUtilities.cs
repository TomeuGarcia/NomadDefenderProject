using System;
using DG.Tweening;
using UnityEngine;

namespace Project.Scripts.Cards
{
    public class TDGameViewUtilities : MonoBehaviour
    {
        [SerializeField] private Material _groundTileTopMaterial;
        private readonly int _wireColorMultiplierPropertyId = Shader.PropertyToID("_WiresColorMultiplier");
        private readonly float _tileMarkDuration = 0.2f;

        private void Awake()
        {
            ResetMarkGroundTiles();
        }

        private void OnDestroy()
        {
            ResetMarkGroundTiles();
        }

        private void ResetMarkGroundTiles()
        {
            _groundTileTopMaterial.SetFloat(_wireColorMultiplierPropertyId, 1f);
        }
        

        public void StartMarkingGroundTiles()
        {
            DoMarkGroundTiles(4f);
        }
        
        public void StopMarkingGroundTiles()
        {
            DoMarkGroundTiles(1f);
        }

        private void DoMarkGroundTiles(float goalValue)
        {
            _groundTileTopMaterial.DOKill();
            _groundTileTopMaterial.DOFloat(goalValue, _wireColorMultiplierPropertyId, _tileMarkDuration)
                .SetEase(Ease.InOutSine);
        }
        
    }
}