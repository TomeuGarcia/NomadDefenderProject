using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    private static GameAudioManager instance;

    // Music
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioClip music1;


    // UI
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioClip uiButtonPressed;
    [SerializeField] private AudioClip screenShut;
    [SerializeField] private AudioClip screenOpen;


    // Enemies
    [SerializeField] private AudioSource[] enemiesAudioSources;
    [SerializeField] private AudioClip enemyTakeDamage;
    [SerializeField] private AudioClip enemyDeath;
    [SerializeField] private AudioClip enemySpawn;

    // Currency
    [SerializeField] private AudioSource[] currencyAudioSources;
    [SerializeField] private AudioClip currencyDropped;
    [SerializeField] private AudioClip currencyCollected;


    // Currency
    [SerializeField] private AudioSource[] projectilesAudioSources;
    [SerializeField] private AudioClip projectileShot;
    [SerializeField] private AudioClip zapProjectileShot;





    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    public GameAudioManager GetInstance()
    {
        return instance;
    }


    // Music
    public void PlayMusic1()
    {
        musicAudioSource.clip = music1;

        musicAudioSource.Play();
    }


    // UI
    public void PlayUiButtonPressed()
    {
        uiAudioSource.clip = uiButtonPressed;
        uiAudioSource.pitch = Random.Range(0.9f, 1.1f);

        uiAudioSource.Play();
    }
    
    public void PlayScreenShut()
    {
        uiAudioSource.clip = screenShut;
        uiAudioSource.pitch = Random.Range(0.9f, 1.1f);

        uiAudioSource.Play();
    }

    public void PlayScreenOpen()
    {
        uiAudioSource.clip = screenOpen;
        uiAudioSource.pitch = Random.Range(0.9f, 1.1f);

        uiAudioSource.Play();
    }


    // Enemies
    public void PlayEnemyTakeDamage()
    {
        int i = 0;
        while (i < enemiesAudioSources.Length)
        {
            if (!enemiesAudioSources[i].isPlaying)
            {
                enemiesAudioSources[i].clip = enemyTakeDamage;
                enemiesAudioSources[i].pitch = Random.Range(0.9f, 1.1f);

                enemiesAudioSources[i].Play();
                break;
            }

            ++i;
        }


    }

    public void PlayEnemyDeath()
    {
        int i = 0;
        while (i < enemiesAudioSources.Length)
        {
            if (!enemiesAudioSources[i].isPlaying)
            {
                enemiesAudioSources[i].clip = enemyTakeDamage;
                enemiesAudioSources[i].pitch = Random.Range(0.9f, 1.1f);

                enemiesAudioSources[i].Play();
                break;
            }

            ++i;
        }

    }



}
