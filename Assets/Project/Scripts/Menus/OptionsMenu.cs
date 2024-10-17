using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private TextManager textManager;
    [SerializeField] private AudioMixersController audioMixersController;

    [SerializeField] private Slider _masterSoundSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;


    public void Init()
    {
        _masterSoundSlider.onValueChanged.AddListener(SetMasterMixerVolume);
        _musicSlider.onValueChanged.AddListener(SetMusicMixerVolume);
        _sfxSlider.onValueChanged.AddListener(SetSFXMixerVolume);

        _masterSoundSlider.value = 1f;
        _musicSlider.value = 0.8f;
        _sfxSlider.value = 0.8f;

        SetMasterMixerVolume(_masterSoundSlider.value);
        SetMusicMixerVolume(_musicSlider.value);
        SetSFXMixerVolume(_sfxSlider.value);
    }

    public void Show()
    {
        textManager.ResetTexts();
        StartCoroutine(textManager.DecodeTextsWithDelay());
    }
    public void Hide()
    {
        textManager.ResetTexts();
    }


    private void SetMasterMixerVolume(float sliderValue)
    {
        audioMixersController.SetMasterMixerVolume(sliderValue);
    }
    private void SetMusicMixerVolume(float sliderValue)
    {
        audioMixersController.SetMusicMixerVolume(sliderValue);
    }
    private void SetSFXMixerVolume(float sliderValue)
    {
        audioMixersController.SetSFXMixerVolume(sliderValue);
    }


}
