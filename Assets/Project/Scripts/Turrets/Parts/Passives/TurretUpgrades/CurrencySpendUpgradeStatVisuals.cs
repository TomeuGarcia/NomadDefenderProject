using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class CurrencySpendUpgradeStatVisuals : MonoBehaviour
{
    [SerializeField] private Transform _canvasHolder;
    [SerializeField] private CanvasGroup _generalCanvasGroup;
    [SerializeField] private Image _fillImage;
    [SerializeField] private Image _divisorImage;
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private TextMeshProUGUI _maxValueText;

    private Queue<UpdateVisualsCommand> _updateVisualsQueue;
    private bool _isAlreadyDequeuing;

    private Color _startColor;


    private class UpdateVisualsCommand
    {
        private int _value;
        private int _maxValue;

        public UpdateVisualsCommand(int value, int maxValue)
        {
            _value = value;
            _maxValue = maxValue;
        }

        public async Task UpdateVisuals(Transform canvasHolder, Image fillImage, TextMeshProUGUI valueText, TextMeshProUGUI maxValueText, Color initialColor)
        {
            float startValuePer1 = fillImage.fillAmount;
            float endValuePer1 = (float)_value / (float)_maxValue;

            float duration = 2.0f * (endValuePer1 - startValuePer1);

            TweenText(valueText, int.Parse(valueText.text), _value, duration);            
            TweenText(maxValueText, int.Parse(maxValueText.text), _maxValue, duration);                       

            fillImage.color = Color.green;
            fillImage.DOColor(initialColor, duration);
            await fillImage.DOFillAmount(endValuePer1, duration).SetEase(Ease.InOutQuad).AsyncWaitForCompletion();

            if (endValuePer1 > 0.999f)
            {
                fillImage.fillAmount = 0.0f;
                canvasHolder.DOPunchPosition(Vector3.up * 0.25f, 1.0f, 6, 0.8f);
            }

        }

        private void TweenText(TextMeshProUGUI text, int startValue, int endValue, float duration)
        {
            int value = startValue;
            DOTween.To(
                () => value,
                (x) => { value = x; text.text = value.ToString(); },
                endValue,
                duration
            );
        }
    }


    private void Awake()
    {
        _updateVisualsQueue = new Queue<UpdateVisualsCommand>();
        _isAlreadyDequeuing = false;

        _startColor = _fillImage.color;
        _generalCanvasGroup.alpha = 0.0f;
    }

    public void QueueUpdateVisuals(int value, int maxValue)
    {       
        _updateVisualsQueue.Enqueue(new UpdateVisualsCommand(value, maxValue));

        if (!_isAlreadyDequeuing)
        {
            StartDequeuingVisualUpdates();
        }        
    }

    private async void StartDequeuingVisualUpdates()
    {
        _isAlreadyDequeuing = true;

        while (_updateVisualsQueue.Count > 0)
        {
            await _updateVisualsQueue.Dequeue().UpdateVisuals(_canvasHolder, _fillImage, _valueText, _maxValueText, _startColor);
        }      

        _isAlreadyDequeuing = false;
    }


    public void Show()
    {
        _generalCanvasGroup.DOFade(1.0f, 2.0f);
    }
    
    public void Hide()
    {
        _generalCanvasGroup.DOFade(0.0f, 1.0f);
    }



}
