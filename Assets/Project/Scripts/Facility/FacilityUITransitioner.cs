using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FacilityUITransitioner : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private TextDecoder _titleText;
    [SerializeField] private GameObject _titleTextShadow;
    [SerializeField] private TextDecoder _loadingText;
    [SerializeField] private TMP_Text _percentageText;
    [SerializeField] private TextDecoder _poppingText;
    [SerializeField] private GameObject _completedText;
    [SerializeField] private ScriptedSequence _scriptedSequence;
    [SerializeField] private Image _loadingBar;

    [Header("PARAMETERS")]
    [SerializeField] private AnimationCurve _loadingCurve;
    [SerializeField] private float _loadingBardTime;
    [SerializeField] private float _consoleLinesTime;
    [SerializeField] private float _completedWaitingTime;
    [SerializeField] private float _consoleDelayBetweenLines;

    private void Start()
    {
        _loadingBar.fillAmount = 0.0f;

        _loadingText.gameObject.SetActive(false);
        _completedText.gameObject.SetActive(false);
    }

    public void StartLoading()
    {
        StartCoroutine(LoadingSequence());
    }

    private IEnumerator LoadingSequence()
    {
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(PoppingText());
        _titleText.gameObject.SetActive(false);
        _titleTextShadow.gameObject.SetActive(false);
        _loadingText.gameObject.SetActive(true);
        _loadingText.Activate();
        _percentageText.gameObject.SetActive(true);
        StartCoroutine(PercentileAddition());
        _loadingBar.gameObject.SetActive(true);

        DOTween.To(() => _loadingBar.fillAmount,
            x => _loadingBar.fillAmount = x,
            1.0f,
            _loadingBardTime
        ).SetEase(_loadingCurve);
        yield return new WaitForSeconds(_loadingBardTime + 0.75f);

        _loadingText.gameObject.SetActive(false);
        _percentageText.gameObject.SetActive(false);
        _loadingBar.gameObject.SetActive(false);
        _poppingText.gameObject.SetActive(false);
        _completedText.gameObject.SetActive(true);
        GameAudioManager.GetInstance().PlayCardSelected();
        yield return new WaitForSeconds(_completedWaitingTime);

        _completedText.gameObject.SetActive(false);
        StartCoroutine(ConsoleLineSpam());
        yield return new WaitForSeconds(_consoleLinesTime);

        SceneLoader.GetInstance().LoadDeckSelector();
    }

    private IEnumerator PoppingText()
    {
        _poppingText.Activate();
        int totalLines = _poppingText.textStrings.Count;
        int currentLines = 0;
        float timeBetweenLines = _loadingBardTime / totalLines;

        while (currentLines < totalLines)
        {
            _poppingText.NextLine();
            yield return new WaitForSeconds(timeBetweenLines);
            currentLines++;
        }
    }

    private IEnumerator PercentileAddition()
    {
        while (_loadingBar.fillAmount < 1.0f)
        {
            _percentageText.text = (_loadingBar.fillAmount * 100).ToString("00") + "%";
            yield return null;
        }

        _percentageText.text = "100%";
    }

    private IEnumerator ConsoleLineSpam()
    {
        int totalLines = _scriptedSequence.GetLineCount();
        int currentLines = 0;

        while (currentLines < totalLines)
        {
            _scriptedSequence.NextLine();
            yield return new WaitForSeconds(_consoleDelayBetweenLines);
            currentLines++;
        }
    }
}
