using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Project.Scripts.CardCollection
{
    public class CardCollectionAnimations : MonoBehaviour
    {
        [SerializeField] private Transform _leftZip;
        [SerializeField] private Transform _rightZip;

        [SerializeField] private TweenConfig _leftZipMoveTween;
        [SerializeField, Min(0)] private float _rightZipMoveDelay = 0.2f;
        [SerializeField] private TweenConfig _rightZipMoveTween;


        private void Awake()
        {
            _leftZip.localPosition = _leftZip.localPosition - _leftZipMoveTween.Value;
            _rightZip.localPosition = _rightZip.localPosition - _rightZipMoveTween.Value;
        }

        public void PlayProjectiles()
        {
            StartCoroutine(DoPlayProjectiles());
        }

        private IEnumerator DoPlayProjectiles()
        {

            
            _leftZip.BlendableLocalMoveBy(_leftZipMoveTween);
            yield return new WaitForSeconds(_rightZipMoveDelay);
            _rightZip.BlendableLocalMoveBy(_rightZipMoveTween);
        }
        
    }
}