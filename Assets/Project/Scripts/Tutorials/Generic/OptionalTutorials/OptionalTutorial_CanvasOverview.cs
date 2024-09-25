using System.Collections;
using DG.Tweening;
using UnityEngine;

public class OptionalTutorial_CanvasOverview : MonoBehaviour
{
    [SerializeField] private CanvasGroup _blackImageHighlight;
    [SerializeField] private GameObject _object;
    [SerializeField] private TextDecoder _text;
    [SerializeField, Min(0)] private int _textLinesCount = 1;
    [SerializeField] private bool _showTopWarning = false;

    public void Awake()
    {
        _blackImageHighlight.alpha = 0;
        _object.SetActive(false);
    }

    
    public IEnumerator Play()
    {
        if (_showTopWarning)
        {
            ServiceLocator.GetInstance().TutorialViewUtilities.ShowWarningTopBar();
        }
        
        yield return _blackImageHighlight.DOFade(1f, 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.InOutSine);
        
        _object.SetActive(true);
        
        _text.Activate();
        for (int i = 0; i < _textLinesCount - 1; ++i)
        {
            yield return new WaitUntil(() => _text.FinishedLine);
            yield return StartCoroutine(WaitForInput());
            
            _text.NextLine();
        }
        yield return new WaitUntil(() => _text.IsDoneDecoding());
        yield return StartCoroutine(WaitForInput());

        Finish();
        yield return _blackImageHighlight.DOFade(0f, 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.InOutSine);
    }

    public void Finish()
    {
        _object.SetActive(false);
        
        if (_showTopWarning)
        {
            ServiceLocator.GetInstance().TutorialViewUtilities.HideWarningTopBar();
        }
    }

    private IEnumerator WaitForInput()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0));
    }

}