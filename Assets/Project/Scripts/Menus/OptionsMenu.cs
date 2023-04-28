using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private TextManager textManager;
    [SerializeField] private AudioMixersController audioMixersController;


    public void Show()
    {
        textManager.ResetTexts();
        StartCoroutine(textManager.DecodeTexts());
    }
    public void Hide()
    {
        textManager.ResetTexts();
    }


    public void SetMasterMixerVolume(float sliderValue)
    {
        audioMixersController.SetMasterMixerVolume(sliderValue);
    }
    public void SetMusicMixerVolume(float sliderValue)
    {
        audioMixersController.SetMusicMixerVolume(sliderValue);
    }
    public void SetSFXMixerVolume(float sliderValue)
    {
        audioMixersController.SetSFXMixerVolume(sliderValue);
    }


}
