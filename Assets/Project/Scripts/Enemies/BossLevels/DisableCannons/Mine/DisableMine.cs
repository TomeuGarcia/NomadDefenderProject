using System;
using System.Collections;
using Scripts.ObjectPooling;
using UnityEngine;

public class DisableMine : RecyclableObject
{
    [System.Serializable]
    public class LogicConfig
    {
        [SerializeField, Min(0f)] private float _totalLifetimeDuration = 3f;
        [SerializeField, Range(0f, 1f)] private float _startLifetimeRatio = 0.5f;
        [SerializeField, Min(0f)] private float _lifetimeRemovePerClick = 0.75f;
        [SerializeField] private BuildingDisableWaveConfig _disableWaveConfig;
        
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

    private IDisableMineDisappearListener _disappearListener;
    public Tile OccupiedTile { get; private set; }

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
    }

    public void Appear()
    {
        gameObject.SetActive(true);
        _view.Init();
        _view.PlayAppearAnimation();
        
        _lifetimeTimer.Reset();
        _lifetimeTimer.Update(Mathf.Max(0.01f, _logicConfig.StartLifetimeTime));

        _update = true;
        StartCoroutine(UpdateLoop());
    }
    

    private IEnumerator UpdateLoop()
    {
        bool wasCleared = false;
        while (_update)
        {
            wasCleared = _lifetimeTimer.CurrentTime.AlmostZero();
            if (wasCleared)
            {
                _update = false;
            }

            bool lifetimeFinished = _lifetimeTimer.HasFinished();
            if (lifetimeFinished)
            {
                _update = false;
            }
        
            _lifetimeTimer.Update(GameTime.DeltaTime);
            _view.UpdateTimer(_lifetimeTimer.Ratio01);

            yield return null;
        }

        _view.HideHovered();
        
        if (wasCleared)
        {
            yield return StartCoroutine(_view.PlayClearedDestroy());
        }
        else
        {
            yield return StartCoroutine(_view.PlayLifetimeEndDestroy());
            BuildingDisableWaveFactory.Instance.Create(_logicConfig.DisableWaveConfig, 
                transform.position, Quaternion.identity);
        }

        Recycle();
    }


    private void OnMousePressed()
    {
        if (_update && !PauseMenu.GameIsPaused)
        {
            _lifetimeTimer.Update(-_logicConfig.LifetimeRemovePerClick);
            _view.PlayTakeDamageAnimation();
        }
    }

    private void OnMouseEntered()
    {
        if (_update)
        {
            _view.ShowHovered();
        }
    }
    private void OnMouseExited()
    {
        if (_update)
        {
            _view.HideHovered();
        }
    }

}