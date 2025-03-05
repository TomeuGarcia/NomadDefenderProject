using System;
using System.Collections;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FIExitDoor : AFacilityInteractable
{
    [System.Serializable]
    private class DoorState
    {
        [SerializeField] private TweenConfig _rotationTween;
        [SerializeField] private AudioClip _sound;
        [SerializeField, Min(0)] private float _soundVolume;
        [SerializeField, MinMaxSlider(0f, 2f)] private Vector2 _soundPitch;

        public Vector3 LocalRotation => _rotationTween.Value;

        public IEnumerator PlayAnimation(Transform rotationPivot, AudioSource audioSource)
        {
            rotationPivot.DOKill();
            rotationPivot.LocalRotate(_rotationTween);
            audioSource.clip = _sound;
            audioSource.volume = _soundVolume;
            audioSource.pitch = Random.Range(_soundPitch.x, _soundPitch.y);
            audioSource.Play();

            yield return new WaitForSeconds(_rotationTween.Duration);
        }
    }
    
    
    [Header("COMPONENTS")] 
    [SerializeField] private Transform _doorPivot;
    [SerializeField] private AudioSource _doorSoundsSource;
    [SerializeField] private GameObject _quitUIHolder;
    [SerializeField] private TextDecoder[] _quitUITexts;
    [SerializeField] private Button _quitGameButton;
    [SerializeField, Min(0)] private float _showButtonDelay = 0.4f;

    [Header("STATES")] 
    [SerializeField] private DoorState _unhoveredState;
    [SerializeField] private DoorState _hoveredState;

    private bool _exiting = false;
    private bool _buttonAvailable = false;

    
    protected override void DoAwake()
    {
        _exiting = false;
        _buttonAvailable = false;
        _quitUIHolder.SetActive(false);
        _quitGameButton.onClick.AddListener(OnQuitGameButtonPressed);

        _doorPivot.localEulerAngles = _unhoveredState.LocalRotation;
    }

    private void OnDestroy()
    {
        _quitGameButton.onClick.RemoveAllListeners();
    }

    protected override IEnumerator DoInteract()
    {
        if (!_buttonAvailable)
        {
            yield break;
        }
        
        _quitGameButton.OnPointerClick(new PointerEventData(EventSystem.current));
    }


    public override void Hovered()
    {
        StopAllCoroutines();
        StartCoroutine(DoHovered());
    }

    public override void Unhovered()
    {
        StopHoverButton();
        StopAllCoroutines();
        StartCoroutine(DoUnhovered());
    }
    

    private IEnumerator DoHovered()
    {
        StartCoroutine(_hoveredState.PlayAnimation(_doorPivot, _doorSoundsSource));
        yield return new WaitForSeconds(_showButtonDelay);
        yield return DoShowExitUI();
        StartHoverButton();
    }

    private IEnumerator DoShowExitUI()
    {
        _quitUIHolder.SetActive(true);
        foreach (TextDecoder quitUIText in _quitUITexts)
        {
            quitUIText.Activate();
            yield return new WaitUntil(() => quitUIText.FinishedLine);
        }
    }

    
    private IEnumerator DoUnhovered()
    {
        StartCoroutine(_unhoveredState.PlayAnimation(_doorPivot, _doorSoundsSource));
        yield return new WaitForSeconds(0.01f);
        HideExitUI();
    }

    private void HideExitUI()
    {
        _quitUIHolder.SetActive(false);
        foreach (TextDecoder quitUIText in _quitUITexts)
        {
            quitUIText.ClearAndStop();
        }
    }


    private void StartHoverButton()
    {
        _buttonAvailable = true;
        _quitGameButton.targetGraphic.color = _quitGameButton.colors.highlightedColor;
    }
    private void StopHoverButton()
    {
        _buttonAvailable = false;
        _quitGameButton.targetGraphic.color = _quitGameButton.colors.normalColor;
    }


    private void OnQuitGameButtonPressed()
    {
        //_exiting = true;
        //_manager.DisableInteractions();
        Application.Quit();
    }

}