using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixersController : MonoBehaviour
{
    [System.Serializable]
    private struct AudioMixerAndParameters
    {
        public AudioMixer audioMixer;
        public string volumeParameterName;
    }


    [SerializeField] private AudioMixerAndParameters masterAudioMixer;
    [SerializeField] private AudioMixerAndParameters musicAudioMixer;
    [SerializeField] private AudioMixerAndParameters sfxAudioMixer;


    private const float AUDIO_MULTIPLIER = 3;


    public void SetMasterMixerVolume(float sliderValue)
    {
        SetMixerVolume(masterAudioMixer, sliderValue);
    }
    public void SetMusicMixerVolume(float sliderValue)
    {
        SetMixerVolume(musicAudioMixer, sliderValue);
    }
    public void SetSFXMixerVolume(float sliderValue)
    {
        SetMixerVolume(sfxAudioMixer, sliderValue);
    }


    private void SetMixerVolume(AudioMixerAndParameters maixerAndParams, float value01)
    {
        maixerAndParams.audioMixer.SetFloat(maixerAndParams.volumeParameterName, Mathf.Log10(value01 * AUDIO_MULTIPLIER) * 20f);
    }

}
