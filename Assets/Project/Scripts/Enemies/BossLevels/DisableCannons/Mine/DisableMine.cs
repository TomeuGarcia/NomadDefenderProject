using System;
using System.Collections;
using Scripts.ObjectPooling;
using UnityEngine;

public class DisableMine : RecyclableObject
{
    [System.Serializable]
    public class LogicConfig
    {
        [SerializeField, Min(0f)] private float _takeDamageCooldown = 0.2f;
        [SerializeField, Min(0f)] private float _totalLifetimeDuration = 3f;
        [SerializeField, Range(0f, 1f)] private float _startLifetimeRatio = 0.5f;
        [SerializeField, Min(0f)] private float _lifetimeRemovePerClick = 0.75f;
        [SerializeField] private BuildingDisableWaveConfig _disableWaveConfig;
        
        public float TakeDamageCooldown => _takeDamageCooldown;
        public float TotalLifetimeDuration => _totalLifetimeDuration;
        public float StartLifetimeTime => _startLifetimeRatio * _totalLifetimeDuration;
        public float LifetimeRemovePerClick => _lifetimeRemovePerClick;
        public BuildingDisableWaveConfig DisableWaveConfig => _disableWaveConfig;
    }
    
    
    [SerializeField] private DisableMineConfig _config;
    [SerializeField] private MouseOverlapNotifier _mouseOverlapNotifier;
    [SerializeField] private DisableMineView _view;


    private LogicConfig _logicConfig;
    private Timer _lifetimeTimer;
    private bool _update;
    private bool _isOnDamageCooldown = false;
    private bool _queueTakeDamage;

    private IDisableMineDisappearListener _disappearListener;
    public Tile OccupiedTile { get; private set; }

    public static bool AnyMineWasCleared { get; set; } = false;

    private void Awake()
    {
        _logicConfig = _config.LogicConfig;
        _lifetimeTimer = new Timer(_logicConfig.TotalLifetimeDuration);
        _view.Configure(_config.ViewConfig);
    }

    private void OnEnable()
    {
        _mouseOverlapNotifier.OnMousePressed += OnMousePressed;
        _mouseOverlapNotifier.OnMouseEntered += OnMouseEntered;
        _mouseOverlapNotifier.OnMouseExited += OnMouseExited;
    }
    private void OnDisable()
    {
        _mouseOverlapNotifier.OnMousePressed -= OnMousePressed;
        _mouseOverlapNotifier.OnMouseEntered -= OnMouseEntered;
        _mouseOverlapNotifier.OnMouseExited -= OnMouseExited;        
    }
    
    internal override void RecycledInit() { }

    internal override void RecycledReleased()
    {
        _disappearListener.OnDisableMineDisappeared(this);
        OccupiedTile = null;
    }

    public void Prepare(Tile occupiedTile, IDisableMineDisappearListener disappearListener)
    {
        gameObject.SetActive(false);
        OccupiedTile = occupiedTile;
        _disappearListener = disappearListener;
        _isOnDamageCooldown = false;
        _update = false;
    }

    public void Appear()
    {
        gameObject.SetActive(true);
        _view.Init();
        _view.PlayAppearAnimation();
        if (AnyMineWasCleared)
        {
            _view.HideClick();
        }
        
        _lifetimeTimer.Reset();
        _lifetimeTimer.Update(Mathf.Max(0.01f, _logicConfig.StartLifetimeTime));

        _update = true;
        StartCoroutine(TakeDamageCooldown());
        StartCoroutine(UpdateLoop());
    }
    

    private IEnumerator UpdateLoop()
    {
        bool wasCleared = false;
        bool lifetimeFinished = false;
        bool doSomethingAfterUpdate = false;
        
        while (_update)
        {
            if (_queueTakeDamage)
            {
                _lifetimeTimer.Update(-_logicConfig.LifetimeRemovePerClick);
                _queueTakeDamage = false;
            }
            
            wasCleared = _lifetimeTimer.CurrentTime.AlmostZero();
            if (wasCleared)
            {
                _update = false;
                doSomethingAfterUpdate = true;
            }

            lifetimeFinished = _lifetimeTimer.HasFinished();
            if (lifetimeFinished)
            {
                _update = false;
                doSomethingAfterUpdate = true;
            }

            _lifetimeTimer.Update(GameTime.DeltaTime);
            _view.UpdateTimer(_lifetimeTimer.Ratio01);

            yield return null;
        }

        if (!doSomethingAfterUpdate) // idk wtf is happening - if I don't do this, it is bugged
        {
            lifetimeFinished = false;
            wasCleared = true;
        }
        
        
        _view.HideHovered();
        if (lifetimeFinished)
        {
            GameAudioManager.GetInstance().PlayCannonMineExplodes();
            yield return StartCoroutine(_view.PlayLifetimeEndDestroy());
            BuildingDisableWaveFactory.Instance.Create(_logicConfig.DisableWaveConfig, 
                transform.position, Quaternion.identity);
        }
        else
        {
            AnyMineWasCleared = true;
            GameAudioManager.GetInstance().PlayCannonMineCleared();
            yield return StartCoroutine(_view.PlayClearedDestroy());
        }

        FinishLifetime();
    }

    

    private void OnMousePressed()
    {
        if (_update && !_isOnDamageCooldown && !PauseMenu.GameIsPaused && !SpeedUpButton.Instance.IsTimePaused)
        {
            _queueTakeDamage = true;
            _view.PlayTakeDamageAnimation();
            GameAudioManager.GetInstance().PlayCannonMineDamaged();
            StartCoroutine(TakeDamageCooldown());
        }
    }

    private IEnumerator TakeDamageCooldown()
    {
        _isOnDamageCooldown = true;
        yield return new WaitForSeconds(_config.LogicConfig.TakeDamageCooldown);
        _isOnDamageCooldown = false;
    }

    private void OnMouseEntered()
    {
        if (_update)
        {
            _view.ShowHovered();
            GameAudioManager.GetInstance().PlayCardInfoMoveHidden();
        }
    }
    private void OnMouseExited()
    {
        if (_update)
        {
            _view.HideHovered();
        }
    }


    private void FinishLifetime()
    {
        Recycle();
        StopAllCoroutines();
    }

}