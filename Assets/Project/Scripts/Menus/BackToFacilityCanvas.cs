using System;
using UnityEngine;
using UnityEngine.UI;

public class BackToFacilityCanvas : MonoBehaviour
{
    [SerializeField] private Button _backButton;
    private bool _interactionEnabled = true;

    private void Awake()
    {
        _backButton.onClick.AddListener(SceneLoader.GetInstance().LoadFacility);
        PauseMenu.GetInstance().GameCanBePaused = false;
        PauseMenu.GetInstance().CanPauseNormally = false;
    }

    private void OnDestroy()
    {
        PauseMenu.GetInstance().GameCanBePaused = true;
        PauseMenu.GetInstance().CanPauseNormally = true;
        ClearButtons();
    }

    public void OnBackButtonHover()
    {
        if (_interactionEnabled)
        {
            GameAudioManager.GetInstance().PlayCardInfoMoveShown();
        }
    }

    private void ClearButtons()
    {
        _backButton.onClick.RemoveAllListeners();
    }
    
    public void DisableButtonsInteraction()
    {
        ClearButtons();
        _interactionEnabled = false;
        _backButton.interactable = false;
    }
}