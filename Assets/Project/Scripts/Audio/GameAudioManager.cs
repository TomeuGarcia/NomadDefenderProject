using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    private static GameAudioManager instance;

    [Header("MUSIC")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private TempMusicClips[] tempMusicClips;
    private Dictionary<MusicType, AudioClip> musicClips = new Dictionary<MusicType, AudioClip>();
    private bool musicPaused = false;
    private float musicDefaultVolume;


    [Header("UI")]
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioClip uiButtonPressed;
    [SerializeField] private AudioClip screenShut;
    [SerializeField] private AudioSource errorSource;
    //[SerializeField] private AudioClip screenOpen;

    [Header("TEXT")]
    [SerializeField] private AudioSource textConsoleTypingSource;
    [SerializeField] private AudioClip[] textConsoleTyping;

    [Header("CARDS")]
    [SerializeField] private AudioSource cardsAudioSource;
    [SerializeField] private AudioSource cardsAudioSource2;
    [SerializeField] private AudioSource cardsAudioSource3;
    [SerializeField] private AudioClip cardSelected;
    [SerializeField] private AudioClip cardHovered;
    [SerializeField] private AudioClip cardHoverExit;
    [SerializeField] private AudioClip cardPlayed;
    const float cardAudioCooldown = 0.2f;
    bool canPlayCardAudio = true;
    [SerializeField] private AudioSource cardsAudioLoopSource;
    [SerializeField] private AudioClip cardRedrawConfirmation;
    [SerializeField] private AudioClip cardRedrawIncreasing;
    private float cardAudioLoopStartVolume;
    private bool playingRedrawsIncrease;

    [Header("CARDS INFO")]
    [SerializeField] private AudioSource cardsInfoAudioSource;
    [SerializeField] private AudioClip cardInfoElement;
    [SerializeField] private AudioClip cardInfoElementMoves;



    [Header("CARDS PLAYED")]
    [SerializeField] private AudioSource cardPlayedAudioSource;
    [SerializeField] private AudioSource watcherCardPlayedAudioSource;



    [Header("IN-BATTLE UPGRADES")]
    [SerializeField] private AudioSource inBattleBuildingUpgradeAudioSource;

    [Header("UPGRADE SCENES")]
    [SerializeField] private AudioSource upgradesAudioSource;
    [SerializeField] private AudioSource upgradesAudioSource2;
    [SerializeField] private AudioSource upgradesAudioSource3;
    [SerializeField] private AudioClip upgradeButtonPressed;
    [SerializeField] private AudioClip upgradeButtonCantBePressed;
    [SerializeField] private AudioClip cardPartSwap;
    [SerializeField] private AudioClip cardPlacedOnUpgradeHolder;
    [SerializeField] private AudioClip cardRetreivedFromUpgradeHolder;
    [SerializeField] private AudioClip cardFinalRetreivedFromUpgrader;

    [Header("ENEMIES")]
    [SerializeField] private AudioSource[] enemiesAudioSources;
    [SerializeField] private AudioClip enemyTakeDamage;
    [SerializeField] private AudioClip enemyDeath;
    [SerializeField] private AudioClip enemySpawn;

    [SerializeField] private AudioSource enemyLastDeathAudioSource;
    [SerializeField] private AudioSource enemyArmorBreakAudioSource;


    [Header("BATTLE SCENES")]
    [SerializeField] private AudioSource battleAudioSource;
    [SerializeField] private AudioClip locationTakeDamage;
    [SerializeField] private AudioSource battleAudioSource2;
    [SerializeField] private AudioSource battleAudioSource3;
    [SerializeField] private AudioClip selfHurtExplosion;
    [SerializeField] private AudioSource battleCursedWiresAudioSource;
    [SerializeField] private AudioClip cursedWiresWave;
    [SerializeField] private AudioClip stageVictory;
    [SerializeField] private AudioClip locationHealed;

    [Header("CURRENCY")]
    [SerializeField] private AudioSource[] currencyAudioSources;
    [SerializeField] private AudioSource UIcurrencyAudioSource;
    [SerializeField] private AudioClip currencyDropped;
    [SerializeField] private AudioClip currencySpent;

    [Header("PROJECTILES")]
    [SerializeField] private AudioSource[] projectilesAudioSources;
    [SerializeField] private AudioClip[] spammerProjectilesAudioSources;
    [SerializeField] private AudioClip[] sentryProjectilesAudioSources;
    [SerializeField] private AudioClip[] blasterProjectilesAudioSources;
    [SerializeField] private AudioClip[] projectileShots;
    [SerializeField] private AudioClip zapProjectileShot;



    [Header("OVERWORLD MAP")]
    [SerializeField] private AudioSource nodeAudioSource;
    [SerializeField] private AudioSource sparkAudioSource;
    [SerializeField] private AudioSource nodeSpawnAudioSource;
    [SerializeField] private AudioSource[] doorAudioSources;


    [Header("GLITCH")]
    [SerializeField] private AudioSource[] glitchAudioSoruces;

    [Header("BACKGROUND")]
    [SerializeField] private AudioSource droneAudioSource;
    private float droneBuildUpInitVolume;
    private IEnumerator droneBuildUp;
    private IEnumerator droneLerpBuildUp;

    [Header("GATHER NEW CARDS")]
    [SerializeField] private AudioSource containerSealAudioSource;
    [SerializeField] private AudioSource containerOpenStartAudioSource;
    [SerializeField] private AudioSource containerOpenEndAudioSource;
    [SerializeField] private AudioSource containerLightOnAudioSource;
    [SerializeField] private AudioSource containerPistonUpAudioSource;

    [Header("OTHER EFFECTS")]
    [SerializeField] private AudioSource effectsAudioSource;
    [SerializeField] private AudioClip smokeBurst;
    [SerializeField] private AudioClip replaceMachineLoad;
    [SerializeField] private AudioClip berserker;


    public enum MusicType {NONE,MENU,OWMAP,BATTLE}
    [System.Serializable]
    struct TempMusicClips
    {
        public MusicType type;
        public AudioClip clip;
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);

            InitVariables();
        }
        else
        {
            Destroy(this);
        }

        InitMusicDictionary();
        musicDefaultVolume = musicAudioSource.volume;
        cardAudioLoopStartVolume = cardsAudioLoopSource.volume;
    }
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.N))
    //    {
    //        NextMusic1();
    //    }
    //    else if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        float volume = Mathf.Clamp01(musicAudioSource.volume + 0.05f);
    //        musicAudioSource.volume = volume;
    //    }
    //    else if (Input.GetKeyDown(KeyCode.O))
    //    {
    //        float volume = Mathf.Clamp01(musicAudioSource.volume - 0.05f);
    //        musicAudioSource.volume = volume;
    //    }
    //}

    public static GameAudioManager GetInstance()
    {
        return instance;
    }


    private void InitVariables()
    {
        droneBuildUpInitVolume = droneAudioSource.volume;
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
    private IEnumerator BuildUp(AudioSource source, float initVol, float endVol, float attackTime, float sustainTime, float releaseTime)
    {
        if(attackTime > 0.0f)
        {
            droneLerpBuildUp = LerpVolume(source, initVol, endVol, attackTime);
            yield return StartCoroutine(droneLerpBuildUp);
        }
        else
        {
            source.volume = endVol;
        }

        if (sustainTime > 0.0f)
        {
            yield return new WaitForSeconds(sustainTime);
        }

        if (releaseTime > 0.0f)
        {
            droneLerpBuildUp = LerpVolume(source, endVol, initVol, releaseTime);
            yield return StartCoroutine(droneLerpBuildUp);
        }
    }

    private IEnumerator LerpVolume(AudioSource source, float initVol, float endVol, float lerpTime)
    {
        float currentTime = 0.0f;
        float tParam;
        float diff = endVol - initVol;

        while (currentTime < lerpTime)
        {
            currentTime += Time.deltaTime;
            tParam = currentTime / lerpTime;
            source.volume = tParam * diff + initVol;
            yield return null;
        }

        source.volume = endVol;
    }


    // Music
    private void InitMusicDictionary()
    {
        foreach(TempMusicClips clips in tempMusicClips)
        {
            musicClips.Add(clips.type, clips.clip);
        }
    }


    public void MusicFadeIn(MusicType type, float duration, float maxVolume)
    {
        AudioClip clip = musicClips[type];
        musicAudioSource.clip = clip;
        musicAudioSource.volume = 0f;
        musicAudioSource.loop = true;
        musicAudioSource.Play();

        musicAudioSource.DOFade(maxVolume, duration);
    }
    public void MusicFadeOut(float duration)
    {
        musicAudioSource.DOFade(0f, duration);
    }
    private void MusicFadeOutThenIn(MusicType type, float duration)
    {
        Sequence fadeSequence = DOTween.Sequence();

        fadeSequence.AppendCallback(() => MusicFadeOut(duration));
        fadeSequence.AppendInterval(duration);
        fadeSequence.AppendCallback(() => MusicFadeIn(type, duration, musicDefaultVolume));
    }


    public void ChangeMusic(MusicType newMusicType, float duration)
    {
        MusicFadeOutThenIn(newMusicType, duration);
    }
    public void PauseMusic1()
    {
        musicAudioSource.Pause();
        musicPaused = true;
    }

    public void ResumeMusic1()
    {
        musicAudioSource.UnPause();
        musicPaused = false;
    }
    public bool isMusicPaused()
    {
        return musicPaused;
    }
    public void PausedMusicPitch()
    {
        musicAudioSource.pitch = 0.85f;
        musicAudioSource.volume = 0.25f;
    }
    public void NormalMusicPitch()
    {
        musicAudioSource.pitch = 1f;
        musicAudioSource.volume = 0.5f;
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

    public void PlayError()
    {
        errorSource.pitch = Random.Range(1.25f, 1.35f);
        errorSource.Play();
    }


    // Text
    public void PlayConsoleTyping(int textType)
    {
        if(textConsoleTyping.Length <= textType)
            textType = 0;

        textConsoleTypingSource.clip = textConsoleTyping[textType];
        textConsoleTypingSource.pitch = Random.Range(0.9f, 1.1f);

        textConsoleTypingSource.Play();
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
        if (!canPlayCardAudio) return;

        cardsAudioSource.clip = cardSelected;
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

    public void PlayRedrawConfirmation()
    {
        //cardsAudioSource3.clip = cardRedrawConfirmation;
        cardsAudioSource3.clip = cardSelected;
        cardsAudioSource3.pitch = Random.Range(1.5f, 1.6f);
        cardsAudioSource3.volume = 0.9f;

        cardsAudioSource3.Play();
    }

    public void PlayRedrawIncreasing(float startPitch, float endPitch, float duration)
    {
        playingRedrawsIncrease = true;

        cardsAudioLoopSource.DOComplete(false);

        cardsAudioLoopSource.clip = cardRedrawIncreasing;
        cardsAudioLoopSource.volume = cardAudioLoopStartVolume;
        cardsAudioLoopSource.pitch = startPitch;
        cardsAudioLoopSource.DOPitch(endPitch, duration);

        cardsAudioLoopSource.Play();
    }
    public void StopRedrawIncreasing()
    {
        playingRedrawsIncrease = false;

        float duration = 0.08f;
        cardsAudioLoopSource.DOFade(0f, duration);
        cardsAudioLoopSource.DOPitch(0.5f, duration).OnComplete(() =>
        {
            if (!playingRedrawsIncrease) cardsAudioLoopSource.Stop();
        });       
    }


    // Cards Info
    public void PlayCardInfoShown()
    {
        cardsInfoAudioSource.clip = cardInfoElement;
        cardsInfoAudioSource.pitch = Random.Range(1.2f, 1.3f);
        cardsInfoAudioSource.volume = 0.25f;

        cardsInfoAudioSource.Play();
    }
    public void PlayCardInfoMoveShown()
    {
        cardsInfoAudioSource.clip = cardInfoElementMoves;
        cardsInfoAudioSource.pitch = Random.Range(1.2f, 1.3f);
        cardsInfoAudioSource.volume = 0.4f;

        cardsInfoAudioSource.Play();
    }

    public void PlayCardInfoHidden()
    {
        cardsInfoAudioSource.clip = cardInfoElement;
        cardsInfoAudioSource.pitch = Random.Range(0.9f, 1.0f);
        cardsInfoAudioSource.volume = 0.25f;

        cardsInfoAudioSource.Play();
    }
    public void PlayCardInfoMoveHidden()
    {
        cardsInfoAudioSource.clip = cardInfoElementMoves;
        cardsInfoAudioSource.pitch = Random.Range(0.9f, 1.0f);
        cardsInfoAudioSource.volume = 0.4f;

        cardsInfoAudioSource.Play();
    }

    public void PlayCardUIInfoShown(float pitch)
    {
        cardsInfoAudioSource.clip = cardInfoElement;
        cardsInfoAudioSource.pitch = pitch;
        cardsInfoAudioSource.volume = 0.25f;

        cardsInfoAudioSource.Play();
    }





    // Cards played

    public void PlayTurretCardPlaced(TurretPartBody.BodyType bodyType)
    {

        float pitch = 1f;
        if (bodyType == TurretPartBody.BodyType.SENTRY)

        {

            pitch = 1f;

        }
        else if (bodyType == TurretPartBody.BodyType.BLASTER)

        {

            pitch = 0.8f;

        }
        else if (bodyType == TurretPartBody.BodyType.SPAMMER)

        {

            pitch = 1.3f;

        }

        cardPlayedAudioSource.pitch = pitch;

        cardPlayedAudioSource.Play();
    }
    public void PlayWatcherCard()
    {
        watcherCardPlayedAudioSource.Play();
    }


    // In-Battle Upgrades
    public void PlayInBattleBuildingUpgrade()
    {

        inBattleBuildingUpgradeAudioSource.pitch = Random.Range(0.9f, 1.1f);

        inBattleBuildingUpgradeAudioSource.Play();

    }


    // Upgrade Scenes
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

    public void PlayCardPlacedOnUpgradeHolder()
    {
        upgradesAudioSource3.clip = cardPlacedOnUpgradeHolder;
        upgradesAudioSource3.pitch = Random.Range(0.9f, 1.1f);
        upgradesAudioSource3.volume = 0.25f;

        upgradesAudioSource3.Play();
    }
    public void PlayCardRetreivedFromUpgradeHolder()
    {
        upgradesAudioSource3.clip = cardRetreivedFromUpgradeHolder;
        upgradesAudioSource3.pitch = Random.Range(0.9f, 1.1f);
        upgradesAudioSource3.volume = 0.25f;

        upgradesAudioSource3.Play();
    }
    public void PlayCardFinalRetreivedFromUpgrader()
    {
        //upgradesAudioSource3.clip = cardFinalRetreivedFromUpgrader;
        //upgradesAudioSource3.pitch = Random.Range(0.9f, 1.1f);
        upgradesAudioSource3.clip = cardHovered;
        upgradesAudioSource3.volume = 1f;
        upgradesAudioSource3.pitch = 0.4f;

        upgradesAudioSource3.Play();
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

    public void PlayEnemyLastDeathHit()

    {
        //StartCoroutine(LerpVolume(enemyLastDeathAudioSource, 0.05f, enemyLastDeathAudioSource.volume, 2.0f));
        enemyLastDeathAudioSource.Play();

    }

    public void PlayEnemyArmorBreak()

    {

        enemyArmorBreakAudioSource.pitch = Random.Range(0.9f, 1.1f);

        enemyArmorBreakAudioSource.Play();

    }


    // Battle
    public void PlayLocationTakeDamage()
    {
        battleAudioSource.clip = locationTakeDamage;
        battleAudioSource.pitch = Random.Range(0.8f, 0.9f);

        battleAudioSource.Play();
    }

    public void PlayLocationDestroyed()
    {
        battleAudioSource.clip = locationTakeDamage;
        battleAudioSource.pitch = Random.Range(1.1f, 1.2f);

        battleAudioSource.Play();
    }
    public void PlayLocationHealed()
    {
        battleAudioSource3.clip = locationHealed;
        battleAudioSource3.pitch = Random.Range(1.1f, 1.2f);

        battleAudioSource3.Play();
    }
    public void PlaySelfHurtExplosion()
    {
        battleAudioSource2.clip = selfHurtExplosion;
        battleAudioSource2.pitch = Random.Range(0.9f, 1.1f);

        battleAudioSource2.Play();
    }

    public void PlayWiresCursedWave()
    {
        battleCursedWiresAudioSource.clip = cursedWiresWave;
        battleCursedWiresAudioSource.pitch = 1.0f;
        battleCursedWiresAudioSource.volume = 1.0f;

        battleCursedWiresAudioSource.Play();
    }

    public void PlayBattleStageVictory()
    {
        battleCursedWiresAudioSource.clip = stageVictory;
        battleCursedWiresAudioSource.pitch = 1.0f;
        battleCursedWiresAudioSource.volume = 1.0f;

        battleCursedWiresAudioSource.Play();
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
        switch (bodyType)
        {
            case TurretPartBody.BodyType.SENTRY:
                LoopAudioSources(projectilesAudioSources, blasterProjectilesAudioSources[(int)Random.Range(0, 3)], Random.Range(0.8f, 1.2f));
                break;

            case TurretPartBody.BodyType.BLASTER:
                LoopAudioSources(projectilesAudioSources, sentryProjectilesAudioSources[(int)Random.Range(0, 3)], Random.Range(0.8f, 1.2f));
                break;
            case TurretPartBody.BodyType.SPAMMER:
                LoopAudioSources(projectilesAudioSources, spammerProjectilesAudioSources[(int)Random.Range(0, 2)], Random.Range(0.8f, 1.2f));
                break;

            default:
                break;
        }
    }

    public void PlayZapProjectileShot()
    {
        LoopAudioSources(projectilesAudioSources, zapProjectileShot, Random.Range(0.8f, 1.2f));
    }







    // Environment

    public void PlaySparksSound()
    {
        sparkAudioSource.pitch = Random.Range(0.8f, 1.2f);
        sparkAudioSource.Play();
    }

    public void PlayNodeSelectedSound()
    {
        nodeAudioSource.pitch = Random.Range(0.9f, 1.0f);
        nodeAudioSource.Play();
    }

    public void PlayDoorSound(int soundIndex)
    {
        doorAudioSources[soundIndex].Play();
    }







    // Glitch

    public void PlayRandomGlitchSound()
    {
        AudioSource randomGlitch = glitchAudioSoruces[Random.Range(0, glitchAudioSoruces.Count())];
        randomGlitch.pitch = Random.Range(0.8f, 1.2f);
        randomGlitch.Play();
    }

    public void PlayGlitchSound(int index)
    {
        glitchAudioSoruces[index].pitch = Random.Range(0.8f, 1.2f);
        glitchAudioSoruces[index].Play();
    }







    // Background

    public void PlayDroneBuildUp(float attackTime, float sustainTime, float releaseTime)
    {
        if(!droneAudioSource.isPlaying)

        {

            droneAudioSource.Play();

        } else {

            droneAudioSource.volume = droneBuildUpInitVolume;

            StopCoroutine(droneBuildUp);

            StopCoroutine(droneLerpBuildUp);

        }



        droneBuildUp = BuildUp(droneAudioSource, 0.0f, droneAudioSource.volume, attackTime, sustainTime, releaseTime);

        StartCoroutine(droneBuildUp);
    }
    
    public void PlayUpgradeNodeSpawnSound()
    {
        nodeSpawnAudioSource.pitch = Random.Range(1.15f, 1.3f);
        nodeSpawnAudioSource.Play();
    }



    public void PlayBattleNodeSpawnSound()
    {
        nodeSpawnAudioSource.pitch = Random.Range(0.6f, 0.75f);
        nodeSpawnAudioSource.Play();
    }



    public void PlayConnectionsNodeSpawnSound()
    {
        nodeSpawnAudioSource.pitch = Random.Range(0.9f, 1.0f);
        nodeSpawnAudioSource.Play();
    }


    // GATHER NEW CARD
    public void PlayCardSlotAppears()
    {
        containerSealAudioSource.Play();
        containerSealAudioSource.pitch = 1.5f;
    }
    public void PlayCardSlotPlacerAppears()
    {
        cardsAudioSource3.clip = cardSelected;
        cardsAudioSource3.pitch = Random.Range(1.5f, 1.6f);
        cardsAudioSource3.volume = 0.5f;

        cardsAudioSource3.Play();
    }

    public void PlayContainerSeal()
    {
        containerSealAudioSource.Play();
        containerSealAudioSource.pitch = 1f;
    }
    public void PlayContainerOpenStart()
    {
        containerOpenStartAudioSource.Play();
    }
    public void PlayContainerOpenEnd()
    {
        //containerOpenEndAudioSource.Play();
    }
    public void PlayContainerLightOn()
    {
        containerLightOnAudioSource.pitch = Random.Range(0.9f, 1.0f);
        containerLightOnAudioSource.Play();
    }
    public void PlayContainerPistonUp()
    {
        containerPistonUpAudioSource.pitch = Random.Range(0.9f, 1.0f);
        containerPistonUpAudioSource.Play();
    }


    // OTHER EFFECTS
    public void PlaySmokeBurst()
    {
        effectsAudioSource.clip = smokeBurst;
        effectsAudioSource.volume = 0.5f;
        effectsAudioSource.pitch = 1.0f;

        effectsAudioSource.Play();
    }
    public void PlayReplaceMachineLoad()
    {
        effectsAudioSource.clip = replaceMachineLoad;
        effectsAudioSource.volume = 1.0f;
        effectsAudioSource.pitch = 1.3f;

        effectsAudioSource.Play();
    }
    public void PlayEnterBerserker()
    {
        effectsAudioSource.clip = berserker;
        effectsAudioSource.volume = 1.0f;
        effectsAudioSource.pitch = 0.85f;

        effectsAudioSource.Play();
    }
    public void PlayExitBerserker()
    {
        effectsAudioSource.clip = berserker;
        effectsAudioSource.volume = 1.0f;
        effectsAudioSource.pitch = 1.5f;

        effectsAudioSource.Play();
    }

}
