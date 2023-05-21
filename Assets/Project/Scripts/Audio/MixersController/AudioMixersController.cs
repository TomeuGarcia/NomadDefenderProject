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
    
    private FMOD.Studio.Bus masterBus;
    private FMOD.Studio.Bus musicBus;
    private FMOD.Studio.Bus sfxBus;

    private void Awake()
    {
        masterBus = FMODUnity.RuntimeManager.GetBus("bus:/Master");
        musicBus = FMODUnity.RuntimeManager.GetBus("bus:/Master/Music");
        sfxBus = FMODUnity.RuntimeManager.GetBus("bus:/Master/SFX");
    }


    public void SetMasterMixerVolume(float sliderValue)
    {
        //SetMixerVolume(masterAudioMixer, sliderValue);
        SetBusVolume(masterBus, sliderValue);
    }
    public void SetMusicMixerVolume(float sliderValue)
    {
        //SetMixerVolume(musicAudioMixer, sliderValue);
        SetBusVolume(musicBus, sliderValue);
    }
    public void SetSFXMixerVolume(float sliderValue)
    {
        //SetMixerVolume(sfxAudioMixer, sliderValue);
        SetBusVolume(sfxBus, sliderValue);
    }


    private void SetMixerVolume(AudioMixerAndParameters maixerAndParams, float value01)
    {
        maixerAndParams.audioMixer.SetFloat(maixerAndParams.volumeParameterName, Mathf.Log10(value01) * 20f);
    }
    
    private void SetBusVolume(FMOD.Studio.Bus bus, float value01)
    {
        bus.setVolume(value01);        
    }

}
