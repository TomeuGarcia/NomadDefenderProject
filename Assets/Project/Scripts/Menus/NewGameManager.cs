using System;
using System.Collections;
using AYellowpaper;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewGameManager : MonoBehaviour
{
    [Header("HOLDER")] 
    [SerializeField] private GameObject _menuHolder;
    
    [Header("BUTTONS")] 
    [SerializeField] private Button _yesNewGameButton;
    [SerializeField] private Button _noNewGameButton;

    [SerializeField] private TextDecoder[] _textDecoders;
    [SerializeField] private AudioSource _newGameAudioSource;

    [Header("UNLOCKING")] 
    [SerializeField] private UnlockableTrophiesManager _unlockableTrophiesManager;
    [SerializeField] private InterfaceReference<IGameProgressionUpdater, ScriptableObject> _gameProgressionUpdater;
    [SerializeField] private GameDifficultyConfig _gameDifficultyConfig;
    [SerializeField] private CardCollectionDataStorage _cardCollection;


    public void Init()
    {
        Hide();
        _yesNewGameButton.onClick.AddListener(Yes_ProceedNewGame);
        _noNewGameButton.onClick.AddListener(No_ProceedNewGame);
    }

    private void OnDestroy()
    {
        _yesNewGameButton.onClick.RemoveAllListeners();
        _noNewGameButton.onClick.RemoveAllListeners();
    }

    private void Hide()
    {
        _menuHolder.SetActive(false);
        foreach (TextDecoder textDecoder in _textDecoders)
        {
            textDecoder.ClearAndStop();
        }
        
        _yesNewGameButton.gameObject.SetActive(false);
        _yesNewGameButton.interactable = false;
        _noNewGameButton.gameObject.SetActive(false);
        _noNewGameButton.interactable = false;
        
        StopAllCoroutines();
    }
    
    public void Show()
    {
        _menuHolder.SetActive(true);
        StartCoroutine(DoShow());
    }

    private IEnumerator DoShow()
    {
        foreach (TextDecoder textDecoder in _textDecoders)
        {
            textDecoder.Activate();
            yield return new WaitUntil(() => textDecoder.FinishedLine);
            yield return new WaitForSeconds(0.1f);
        }

        const float blinkDuration = 0.1f;
        for (int i = 0; i < 2; ++i)
        {
            _yesNewGameButton.gameObject.SetActive(false);
            yield return new WaitForSeconds(blinkDuration);
            _yesNewGameButton.gameObject.SetActive(true);
            GameAudioManager.GetInstance().PlayCardInfoShown();
            yield return new WaitForSeconds(blinkDuration);
        }
        
        for (int i = 0; i < 2; ++i)
        {
            _noNewGameButton.gameObject.SetActive(false);
            yield return new WaitForSeconds(blinkDuration);
            _noNewGameButton.gameObject.SetActive(true);
            GameAudioManager.GetInstance().PlayCardInfoShown();
            yield return new WaitForSeconds(blinkDuration);
        }

        _yesNewGameButton.interactable = true;
        _noNewGameButton.interactable = true;
    }
    

    
    private void Yes_ProceedNewGame()
    {
        ButtonClickedPunch(_yesNewGameButton.targetGraphic.rectTransform);

        TutorialsSaverLoader.GetInstance().ResetTutorials();
        StarterDecksUnlocker.GetInstance().ResetUnlockedCount();
        ServiceLocator.GetInstance().OptionalTutorialsStateManager.SetAllTutorialsNotDone();
        _cardCollection.DoReset();

        _unlockableTrophiesManager.SetAllTrophiesLocked();
        _gameProgressionUpdater.Value.ResetEverything();

        _gameDifficultyConfig.ResetState();
        
        
        ServiceLocator.GetInstance().RunInfo.SetNewGame(true);
        StartCoroutine(DoStartNewGame());
    }
    
    private IEnumerator DoStartNewGame()
    {
        _newGameAudioSource.Play();
        yield return new WaitForSeconds(0.1f);

        Hide();
        PauseMenu.GetInstance().LeaveOptionsMenu();
        
        ServiceLocator.GetInstance().RunInfo.SetComeFromRun(false);
        SceneLoader.GetInstance().LoadFacilityInstantly();
    }
    
    
    
    private void No_ProceedNewGame()
    {
        ButtonClickedPunch(_noNewGameButton.targetGraphic.rectTransform);
        StartCoroutine(DoGoBack());
    }
    
    private IEnumerator DoGoBack()
    {
        yield return new WaitForSeconds(0.1f);
        _yesNewGameButton.interactable = false;
        _noNewGameButton.interactable = false;
        Hide();
    }
    
    private void ButtonClickedPunch(RectTransform buttonRectTransform)
    {
        GameAudioManager.GetInstance().PlayCardSelected();
    }
}