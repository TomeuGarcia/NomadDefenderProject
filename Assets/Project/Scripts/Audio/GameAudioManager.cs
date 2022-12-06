using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    private static GameAudioManager instance;

    [Header("MUSIC")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioClip music1;

    [Header("UI")]
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioClip uiButtonPressed;
    [SerializeField] private AudioClip screenShut;
    //[SerializeField] private AudioClip screenOpen;

    [Header("CARDS")]
    [SerializeField] private AudioSource cardsAudioSource;
    [SerializeField] private AudioSource cardsAudioSource2;
    [SerializeField] private AudioClip cardHovered;
    [SerializeField] private AudioClip cardHoverExit;
    [SerializeField] private AudioClip cardPlayed;
    const float cardAudioCooldown = 0.2f;
    bool canPlayCardAudio = true;

    [Header("UPGRADES")]
    [SerializeField] private AudioSource upgradesAudioSource;
    [SerializeField] private AudioSource upgradesAudioSource2;
    [SerializeField] private AudioClip upgradeButtonPressed;
    [SerializeField] private AudioClip upgradeButtonCantBePressed;
    [SerializeField] private AudioClip cardPartSwap;

    [Header("ENEMIES")]
    [SerializeField] private AudioSource[] enemiesAudioSources;
    [SerializeField] private AudioClip enemyTakeDamage;
    [SerializeField] private AudioClip enemyDeath;
    [SerializeField] private AudioClip enemySpawn;

    [Header("CURRENCY")]
    [SerializeField] private AudioSource[] currencyAudioSources;
    [SerializeField] private AudioSource UIcurrencyAudioSource;
    [SerializeField] private AudioClip currencyDropped;
    [SerializeField] private AudioClip currencySpent;

    [Header("PROJECTILES")]
    [SerializeField] private AudioSource[] projectilesAudioSources;
    [SerializeField] private AudioClip[] projectileShots;
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

    public static GameAudioManager GetInstance()
    {
        return instance;
    }


    // Helpers
    private void LoopAudioSources(AudioSource[] audioSources, AudioClip clip, float pitch)
    {
        int i = 0;
        while (i < audioSources.Length)
        {
            if (!audioSources[i].isPlaying)
            {
                audioSources[i].clip = clip;
                audioSources[i].pitch = pitch;

                audioSources[i].Play();
                break;
            }

            ++i;
        }
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
        uiAudioSource.pitch = Random.Range(0.9f, 0.95f);

        uiAudioSource.Play();
    }

    public void PlayScreenOpen()
    {
        //uiAudioSource.clip = screenOpen;
        uiAudioSource.clip = screenShut;
        uiAudioSource.pitch = Random.Range(1.0f, 1.05f);

        uiAudioSource.Play();
    }


    // Cards
    public void PlayCardHovered()
    {
        if (!canPlayCardAudio) return;

        cardsAudioSource.clip = cardHovered;
        cardsAudioSource.pitch = Random.Range(0.9f, 1.1f);

        cardsAudioSource.Play();

        StartCoroutine(CardAudioCooldown());
    }
    private IEnumerator CardAudioCooldown()
    {
        canPlayCardAudio = false;
        yield return new WaitForSeconds(cardAudioCooldown);
        canPlayCardAudio = true;
    }
    public void PlayCardSelected()
    {
        //if (!canPlayCardAudio) return;

        cardsAudioSource.clip = cardHovered;
        cardsAudioSource.pitch = Random.Range(1.3f, 1.4f);

        cardsAudioSource.Play();

        StartCoroutine(CardAudioCooldown());
    }

    public void PlayCardHoverExit()
    {
        if (cardsAudioSource2.isPlaying) return;

        cardsAudioSource2.clip = cardHoverExit;
        cardsAudioSource2.pitch = Random.Range(0.9f, 1.1f);

        cardsAudioSource2.Play();
    }

    public void PlayCardPlayed()
    {
        cardsAudioSource.clip = cardPlayed;
        cardsAudioSource.pitch = Random.Range(0.9f, 1.1f);

        cardsAudioSource.Play();
    }


    // Upgrades
    public void PlayCardPartSwap()
    {
        upgradesAudioSource.clip = cardPartSwap;
        upgradesAudioSource.pitch = Random.Range(0.95f, 1.05f);

        upgradesAudioSource.Play();
    }

    public void PlayUpgradeButtonPressed()
    {
        upgradesAudioSource2.clip = upgradeButtonPressed;
        upgradesAudioSource2.pitch = Random.Range(0.95f, 1.05f);

        upgradesAudioSource2.Play();
    }

    public void PlayUpgradeButtonCantBePressed()
    {
        upgradesAudioSource2.clip = upgradeButtonCantBePressed;
        upgradesAudioSource2.pitch = Random.Range(0.95f, 1.05f);

        upgradesAudioSource2.Play();
    }



    // Enemies
    public void PlayEnemyTakeDamage()
    {
        LoopAudioSources(enemiesAudioSources, enemyTakeDamage, Random.Range(0.9f, 1.1f));
    }

    public void PlayEnemyDeath()
    {
        LoopAudioSources(enemiesAudioSources, enemyDeath, Random.Range(0.9f, 1.1f));
    }

    public void PlayEnemySpawn()
    {
        LoopAudioSources(enemiesAudioSources, enemySpawn, Random.Range(0.9f, 1.1f));
    }


    // Currency
    public void PlayCurrencyDropped()
    {
        LoopAudioSources(currencyAudioSources, currencyDropped, Random.Range(0.85f, 1.15f));
    }

    public void PlayCurrencySpent()
    {
        UIcurrencyAudioSource.clip = currencySpent;
        UIcurrencyAudioSource.pitch = Random.Range(0.9f, 1.1f);

        UIcurrencyAudioSource.Play();
    }


    // Projectiles
    public void PlayProjectileShot(TurretPartBody.BodyType bodyType)
    {
        LoopAudioSources(projectilesAudioSources, projectileShots[(int)bodyType], Random.Range(0.85f, 1.15f));
    }

    public void PlayZapProjectileShot()
    {
        LoopAudioSources(projectilesAudioSources, zapProjectileShot, Random.Range(0.85f, 1.15f));
    }




}
