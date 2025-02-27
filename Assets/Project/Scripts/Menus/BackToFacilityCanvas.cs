using System;
using UnityEngine;
using UnityEngine.UI;

public class BackToFacilityCanvas : MonoBehaviour
{
    [SerializeField] private Button _backButton;

    private void Awake()
    {
        _backButton.onClick.AddListener(SceneLoader.GetInstance().LoadFacility);
        PauseMenu.GetInstance().GameCanBePaused = false;
    }

    private void OnDestroy()
    {
        PauseMenu.GetInstance().GameCanBePaused = true;
    }

    public void OnBackButtonHover()
    {
        GameAudioManager.GetInstance().PlayCardInfoMoveShown();
    }
}