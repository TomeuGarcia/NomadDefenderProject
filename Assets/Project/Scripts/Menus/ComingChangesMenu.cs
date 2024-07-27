using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ComingChangesMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup _menuHolderGroup;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _wishlistButton;

    private void Awake()
    {
        _backButton.onClick.AddListener(OnBackButtonPressed);
        _wishlistButton.onClick.AddListener(OnWishlistButtonPressed);

        InstantClose();
    }

    public void Open()
    {
        _menuHolderGroup.gameObject.SetActive(true);
        _menuHolderGroup.DOFade(1f, 0.1f)
            .SetEase(Ease.InOutSine);
    }

    public void Close()
    {
        _menuHolderGroup.DOFade(0f, 0.1f)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => _menuHolderGroup.gameObject.SetActive(false));
    }

    private void InstantClose()
    {
        _menuHolderGroup.alpha = 0;
        _menuHolderGroup.gameObject.SetActive(false);
    }

    private void OnBackButtonPressed()
    {
        Close();
        GameAudioManager.GetInstance().PlayCardInfoMoveHidden();
    }
    private void OnWishlistButtonPressed()
    {
        Application.OpenURL(MainMenu.STEAM_WISHLIST_LINK);
    }
}
