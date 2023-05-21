using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static RandomSoundsCollection;

public class PlayOnParticleBurst : MonoBehaviour
{
    [SerializeField] ParticleSystem particleSystem;
    //[SerializeField] AudioSource audioSource;
    //[SerializeField] List<RandomSoundsCollection.RandomSound> soundPool = new List<RandomSound>();

    [SerializeField] private FMODUnity.StudioEventEmitter soundEmitter;

    private bool allParticlesDied = true;

    public static bool canPlaySound = true;



    void Update()
    {
        if(allParticlesDied && particleSystem.particleCount > 0)
        {
            allParticlesDied = false;

            //int soundIndex = Random.Range(0, soundPool.Count);
            //audioSource.clip = soundPool[soundIndex].audioClip;
            //audioSource.volume = Random.Range(soundPool[soundIndex].minVolume, soundPool[soundIndex].maxVolume);
            //audioSource.pitch = Random.Range(soundPool[soundIndex].minPitch, soundPool[soundIndex].maxPitch);
            //audioSource.Play();

            if (canPlaySound)
            {
                soundEmitter.Play();
            }
        }
        else if(!allParticlesDied && particleSystem.particleCount == 0)
        {
            allParticlesDied = true;
        }
    }
}
