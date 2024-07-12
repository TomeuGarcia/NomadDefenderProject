using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FacilityUITransitioner : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private GameObject _titleText;
    [SerializeField] private GameObject _loadingText;
    [SerializeField] private Image _loadingBar;

    [Header("PARAMETERS")]
    [SerializeField] private float _loadingBardTime;
    [SerializeField] private float _timeToTransition;

    private void Start()
    {
        _loadingBar.fillAmount = 0.0f;
    }

    public void StartLoading()
    {
        StartCoroutine(LoadingSequence());
    }

    private IEnumerator LoadingSequence()
    {
        //TODO - use text decoder and make this fancy af
        GameAudioManager.GetInstance().PlayCardSelected();
        yield return new WaitForSeconds(0.5f);

        _titleText.gameObject.SetActive(false);
        _loadingText.gameObject.SetActive(true);
        _loadingBar.gameObject.SetActive(true);
        
        DOTween.To(() => _loadingBar.fillAmount,
            x => _loadingBar.fillAmount = x,
            1.0f,
            _loadingBardTime
        );
        yield return new WaitForSeconds(_loadingBardTime);

        GameAudioManager.GetInstance().PlayCardSelected();
        yield return new WaitForSeconds(_timeToTransition);

        //TODO - WRITE A BUNC OF CODE ON CONSOLE
        //yield return new WaitForSeconds(_timeToTransition);

        //Load necessary scene
        //TODO - if(firstTime -> to tuto), else(to deckselector)
        SceneLoader.GetInstance().LoadDeckSelector();

    }
}
