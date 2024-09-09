using System;
using UnityEngine;

namespace Project.Scripts.Utilities
{
    public class CanvasGroupFadeLoop : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Vector2 _fadeInterval;
        [SerializeField] private float _speed;

        private void Update()
        {
            float t = (Mathf.Sin(Time.time * _speed) + 1) / 2f;
            _canvasGroup.alpha =  Mathf.LerpUnclamped(_fadeInterval.x, _fadeInterval.y, t);
        }
    }
}