using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyWaveManager;

public class RandomSoundsCollection : MonoBehaviour
{
    [System.Serializable]
    public class RandomSound
    {
        [SerializeField, Range(0, 2)] public float minPitch = 0.9f;
        [SerializeField, Range(0, 2)] public float maxPitch = 1.1f;
        [SerializeField, Range(0, 1)] public float minVolume = 0.9f;
        [SerializeField, Range(0, 1)] public float maxVolume = 1.0f;
        [SerializeField] public AudioClip audioClip;
    }


    private delegate void PlayRandomSoundsFunction();

    public delegate void RandomSoundPlayed();
    public event RandomSoundPlayed OnRandomSoundPlayed;

    private enum RandomMode { RANDOM, POOLED_RANDOM }


    [SerializeField] private RandomMode randomMode;
    [SerializeField] public float startDelay;
    [SerializeField] public float minDelayBetweenSounds;
    [SerializeField] public float maxDelayBetweenSounds;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public RandomSound[] randomSounds;
    private List<RandomSound> pooledSounds;

    private bool keepPlaying;


    public void StartPlaying()
    {
        keepPlaying = true;
        if (randomMode == RandomMode.RANDOM)
        {
            StartCoroutine(PlaySoundsLoop(PlayRandomSound));
        }
        else if (randomMode == RandomMode.POOLED_RANDOM)
        {
            StartCoroutine(PlaySoundsLoop(PlayRandomPooledSound));
        }
    }
    public void StopPlaying()
    {
        keepPlaying = false;
    }
    public void StopSound()
    {
        keepPlaying = false;
        audioSource.Stop();
    }

    private IEnumerator PlaySoundsLoop(PlayRandomSoundsFunction playRandomSoundsFunction)
    {
        yield return new WaitForSeconds(startDelay);

        while (keepPlaying)
        {            
            playRandomSoundsFunction();
            if(OnRandomSoundPlayed != null) { OnRandomSoundPlayed(); } 

            yield return new WaitForSeconds(Random.Range(minDelayBetweenSounds, maxDelayBetweenSounds));
        }
    }

    public void PlayRandomSound()
    {
        RandomSound randomSound = randomSounds[Random.Range(0, randomSounds.Length)];
        PlayRandomSound(randomSound);
    }

    public void PlayRandomSoundWithVolumeCoef(float volumeCoef)
    {
        RandomSound randomSound = randomSounds[Random.Range(0, randomSounds.Length)];
        PlayRandomSound(randomSound, volumeCoef);
    }

    private void PlayRandomPooledSound()
    {
        if (pooledSounds == null) pooledSounds = new List<RandomSound>();

        if (pooledSounds.Count == 0)
        {
            pooledSounds.Capacity = randomSounds.Length;

            for (int i = 0; i < randomSounds.Length; ++i)
            {
                pooledSounds.Add(randomSounds[i]);
            }
        }

        int randomI = Random.Range(0, pooledSounds.Count);
        RandomSound randomSound = pooledSounds[randomI];
        pooledSounds.RemoveAt(randomI);

        PlayRandomSound(randomSound);
    }

    private void PlayRandomSound(RandomSound randomSound, float volumeCoef = 1.0f)
    {
        audioSource.clip = randomSound.audioClip;
        audioSource.volume = Random.Range(randomSound.minVolume, randomSound.maxVolume) * volumeCoef;
        audioSource.pitch = Random.Range(randomSound.minPitch, randomSound.maxPitch);
        audioSource.Play();
    }
}
