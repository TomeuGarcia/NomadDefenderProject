using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static RandomSoundsCollection;

public class PlayOnParticleBurst : MonoBehaviour
{
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] AudioSource audioSource;
    [SerializeField] List<RandomSoundsCollection.RandomSound> soundPool = new List<RandomSound>();

    private OptionsMenu _optionsMenu;

    private bool allParticlesDied = true;

    private void Awake()
    {
        _optionsMenu = PauseMenu.GetInstance().OptionsMenu;
    }

    void Update()
    {
        if(allParticlesDied && particleSystem.particleCount > 0)
        {
            allParticlesDied = false;

            int soundIndex = Random.Range(0, soundPool.Count);
            audioSource.clip = soundPool[soundIndex].audioClip;
            audioSource.volume = Random.Range(soundPool[soundIndex].minVolume, soundPool[soundIndex].maxVolume)
                * _optionsMenu.GetCombinedSFXandMasterVolumeCoef();
            audioSource.pitch = Random.Range(soundPool[soundIndex].minPitch, soundPool[soundIndex].maxPitch);
            audioSource.Play();
        }
        else if(!allParticlesDied && particleSystem.particleCount == 0)
        {
            allParticlesDied = true;
        }
    }
}
