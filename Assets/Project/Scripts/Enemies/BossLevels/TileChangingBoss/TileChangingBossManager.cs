using System;
using System.Collections;
using Project.Scripts.Enemies.BossLevels;
using UnityEngine;
using UnityEngine.Serialization;

public class TileChangingBossManager : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private SpeedUpButton _speedUpButton;
    [SerializeField] private EnemyWaveManager _enemyWaveManager;
    [SerializeField] private TDGameManager _tdGameManager;
    [SerializeField] private ConsoleDialogSystem _bossDialogueSystem;
    [SerializeField] private ScreenGlitcher _screenGlitcher;
    
    [Header("EVENTS")]
    [SerializeField] private TileChangingBossEvent[] _levelEvents;
    [SerializeField] private TileChangingBossVictoryEvent _victoryEvent;
    private TileChangingBossEvent _currentLevelEvent;
    private int _currentWaveIndex = 0;
    private bool _playingVictory;
    private bool _firstEventHappened;
    
    private void Awake()
    {
        _currentLevelEvent = null;
        _currentWaveIndex = 0;
        _playingVictory = false;
        _firstEventHappened = false;
        
        foreach (TileChangingBossEvent levelTileChange in _levelEvents)
        {
            levelTileChange.Init(_bossDialogueSystem);
        }
        
        EnemyWaveManager.OnAllWavesFinished += PlayVictory;
    }

    private void Start()
    {
        _speedUpButton.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        GameTime.SetTimeScale(1);

        if (!_playingVictory)
        {
            EnemyWaveManager.OnAllWavesFinished -= PlayVictory;
        }

        GameAudioManager.GetInstance().SetMusicPitch(1);
    }

    private void OnEnable()
    {
        EnemyWaveManager.OnStartNewWaves += OnStartNewWaves;
    }
    private void OnDisable()
    {
        EnemyWaveManager.OnStartNewWaves -= OnStartNewWaves;
    }


    private void OnStartNewWaves()
    {
        ++_currentWaveIndex;
        
        if (!GetNextLevelEvent(out TileChangingBossEvent nextLevelEvent))
        {
            return;
        }
        
        StartCoroutine(StartNewWaveEvent(_currentLevelEvent, nextLevelEvent));
        _currentLevelEvent = nextLevelEvent;
    }

    private bool GetNextLevelEvent(out TileChangingBossEvent nextLevelTileChange)
    {
        foreach (TileChangingBossEvent levelTileChange in _levelEvents)
        {
            if (levelTileChange.WaveIndex == _currentWaveIndex)
            {
                nextLevelTileChange = levelTileChange;
                return true;
            }
        }

        nextLevelTileChange = null;
        return false;
    }

    private IEnumerator StartNewWaveEvent(TileChangingBossEvent oldLevelEvent, TileChangingBossEvent nextLevelEvent)
    {
        if (!_speedUpButton.IsTimePaused)
        {
            _speedUpButton.SetDefaultTimeSpeed();
        }
        
        _enemyWaveManager.WaveStartPaused = true;
        GameTime.SetTimeScale(0);

        
        yield return StartCoroutine(nextLevelEvent.Dialogue.PlayBeforeAnimationLines());

        bool isFirstEvent = !_firstEventHappened;
        if (isFirstEvent)
        {
            GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.BOSS_BATTLE, 0.1f);
            _firstEventHappened = true;            
        }

        StartCoroutine(ShowNext(0.2f, oldLevelEvent, nextLevelEvent));
        yield return StartCoroutine(PlayShowNextAnimation(!isFirstEvent));
        yield return StartCoroutine(nextLevelEvent.Dialogue.PlayAfterAnimationLines());

        
        _enemyWaveManager.WaveStartPaused = false;
        GameTime.SetTimeScale(1);
    }
    

    private IEnumerator ShowNext(float delay, TileChangingBossEvent oldLevelEvent, TileChangingBossEvent nextLevelEvent)
    {
        yield return new WaitForSeconds(delay);
        oldLevelEvent?.Hide();
        nextLevelEvent.Show();
    }

    private IEnumerator PlayShowNextAnimation(bool withMusicPitchVariation)
    {
        yield return StartCoroutine(
            _screenGlitcher.PlayGlitch(0f, 0.2f, 0.7f, _currentWaveIndex, withMusicPitchVariation)
        );
        GameAudioManager.GetInstance().SetMusicPitch(1);
    }



    private void PlayVictory()
    {
        EnemyWaveManager.OnAllWavesFinished -= PlayVictory;
        _playingVictory = true;
        StartCoroutine(DoPlayVictory());
    }

    private IEnumerator DoPlayVictory()
    {
        _tdGameManager.VictoryPaused = true;
        
        yield return StartCoroutine(_victoryEvent.PlayVictory(_bossDialogueSystem));
        
        _tdGameManager.VictoryPaused = false;
    }
}