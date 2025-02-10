using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DemoThanksForPlayingManager : MonoBehaviour
{
    [Header("MAIN TEXTS")] 
    [SerializeField] private Camera _camera;
    
    [Header("MAIN TEXTS")] 
    [SerializeField] private TextDecoder _titleText;
    [SerializeField] private TextDecoder[] _descriptionTexts;

    [Header("CARDS")] 
    [SerializeField] private CardMotionConfig _cardMotionConfig;
    [SerializeField] private TurretCardDataModel _placeholderCardData;
    [SerializeField] private TurretBuildingCard[] _cards;
    [SerializeField] private TweenConfig _cardHiddenDisplacement;

    [Header("BUTTONS")] 
    [SerializeField] private Button _steamWishlistButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private TweenPunchConfig _exitButtonPositionPunch;


    private void Awake()
    {
        _steamWishlistButton.onClick.AddListener(OnSteamWishlistButtonPressed);
        _exitButton.onClick.AddListener(OnExitButtonPressed);
        
        CardTooltipDisplayManager.GetInstance().SetDisplayCamera(_camera);
        ServiceLocator.GetInstance().CameraHelp.SetCardsCamera(_camera);
        _cardMotionConfig.SetResultsScreenDisplayMode();
        
        GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.OWMAP, 0.2f);
    }

    private void Start()
    {
        SetupShowAnimation();
        StartCoroutine(ShowAnimation());
    }

    
    

    private void SetupShowAnimation()
    {
        foreach (TurretBuildingCard card in _cards)
        {
            Vector3 cardPosition = card.transform.position;
            card.InitWithData(new TurretCardData(_placeholderCardData));
            card.ResizeColliderForShowcase();
            card.InitPositions(cardPosition, Vector3.zero, cardPosition);
            card.transform.position -= _cardHiddenDisplacement.Value;
        }
        
        _exitButton.gameObject.SetActive(false);
    }

    private IEnumerator ShowAnimation()
    {
        yield return new WaitForSeconds(1.5f);
        
        _titleText.Activate();
        yield return new WaitUntil(() => _titleText.FinishedLine);
        yield return new WaitForSeconds(0.5f);

        foreach (TextDecoder descriptionText in _descriptionTexts)
        {
            descriptionText.Activate();
            yield return new WaitUntil(() => descriptionText.FinishedLine);
        }
        yield return new WaitForSeconds(1.0f);



        foreach (TurretBuildingCard card in _cards)
        {
            GameAudioManager.GetInstance().PlayCardSelected();
            card.transform.DOBlendableMoveBy(_cardHiddenDisplacement.Value, _cardHiddenDisplacement.Duration)
                .SetEase(_cardHiddenDisplacement.Ease);
            
            yield return new WaitForSeconds(_cardHiddenDisplacement.Duration * 0.5f);
        }

        yield return new WaitForSeconds(_cardHiddenDisplacement.Duration * 0.5f);
        StartCardsInteraction();

        
        yield return new WaitForSeconds(4.5f);
        for (int i = 0; i < 2; ++i)
        {
            GameAudioManager.GetInstance().PlayCardInfoShown();
            _exitButton.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            _exitButton.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }
        _exitButton.gameObject.SetActive(true);
        
    }



    private void OnExitButtonPressed()
    {
        _exitButton.onClick.RemoveAllListeners();
        _exitButton.transform.PunchPosition(_exitButtonPositionPunch);
        GameAudioManager.GetInstance().PlayCardSelected();
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        SceneLoader.GetInstance().StartLoadGameEndCredits();
    }
    
    private void OnSteamWishlistButtonPressed()
    {
        OpenSteamWishlist();
    }
    
    public void OpenSteamWishlist()
    {
        Application.OpenURL(MainMenu.STEAM_WISHLIST_LINK);
    }
    
    
    
    
    private void StartCardsInteraction()
    {
        foreach (BuildingCard card in _cards)
        {
            card.OnCardHovered += SetHoveredCard;
            card.ReenableMouseInteraction();
            card.canDisplayInfoIfNotInteractable = false;
            card.canDisplayInfoIfWhileInteractable = false;
            card.isInteractable = true;
            card.hideInfoWhenSelected = false;
        }
    }

    private void SetHoveredCard(BuildingCard card)
    {
        card.HoveredState(rotate: false);

        foreach (BuildingCard itCard in _cards)
        {
            itCard.OnCardHovered -= SetHoveredCard;
            itCard.OnCardUnhovered += SetStandardCard;
        }
            
        GameAudioManager.GetInstance().PlayCardHovered();
    }

    private void SetStandardCard(BuildingCard card)
    {
        card.StandardState(false);
            
        foreach (BuildingCard itCard in _cards)
        {
            itCard.OnCardHovered += SetHoveredCard;
            itCard.OnCardUnhovered -= SetStandardCard;
        }

    }
    
    

}
