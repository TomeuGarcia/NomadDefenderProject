using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUMAnimator : MonoBehaviour
{
    [Header("PARAMETERS")]
    [SerializeField, Min(0f)] private float _uniqueAmbientIntensity;
    [SerializeField, Min(0f)] private float _shutDownAmbientIntensity;
    private float _ambientIntensity;

    [Header("REFERENCES")]
    [SerializeField] private Animator _animator;
    [SerializeField] private GUMCableFillCheckpoint[] _leftCableFillStarts;
    [SerializeField] private GUMCableFillCheckpoint[] _rightCableFillStarts;
    [SerializeField] private GUMSideIndicator _leftIndicator;
    [SerializeField] private GUMSideIndicator _rightIndicator;

    private void Awake()
    {
        _ambientIntensity = RenderSettings.ambientIntensity;
        RenderSettings.ambientIntensity = _uniqueAmbientIntensity;
        Enter();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Enter();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LeftReady();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            LeftUnReady();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            RightReady();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            RightUnReady();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Ready();
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            UnReady();
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Upgrading();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void Enter()
    {
        _animator.SetTrigger("Enter");
    }

    public void LeftReady()
    {
        _leftIndicator.Ready();

        foreach (GUMCableFillCheckpoint checkpoint in _leftCableFillStarts)
        {
            checkpoint.StartReadyFill();
        }
    }
    public void LeftUnReady()
    {
        _leftIndicator.UnReady();

        foreach (GUMCableFillCheckpoint checkpoint in _leftCableFillStarts)
        {
            checkpoint.StartUnReadyFill();
        }
    }

    public void RightReady()
    {
        _rightIndicator.Ready();

        foreach (GUMCableFillCheckpoint checkpoint in _rightCableFillStarts)
        {
            checkpoint.StartReadyFill();
        }
    }
    public void RightUnReady()
    {
        _rightIndicator.UnReady();

        foreach (GUMCableFillCheckpoint checkpoint in _rightCableFillStarts)
        {
            checkpoint.StartUnReadyFill();
        }
    }

    public void Ready()
    {

    }
    public void UnReady()
    {

    }

    public void Upgrading()
    {
        _animator.SetTrigger("Upgrading");
        _leftIndicator.Upgrading();
        _rightIndicator.Upgrading();

        foreach (GUMCableFillCheckpoint checkpoint in _leftCableFillStarts)
        {
            checkpoint.StartUpgradingFill();
        }
        foreach (GUMCableFillCheckpoint checkpoint in _rightCableFillStarts)
        {
            checkpoint.StartUpgradingFill();
        }
    }

    public void ShutDown()
    {
        foreach (GUMCableFillCheckpoint checkpoint in _leftCableFillStarts)
        {
            checkpoint.ShutDown();
        }
        foreach (GUMCableFillCheckpoint checkpoint in _rightCableFillStarts)
        {
            checkpoint.ShutDown();
        }

        RenderSettings.ambientIntensity = _shutDownAmbientIntensity;
    }

    private void OnDestroy()
    {
        RenderSettings.ambientIntensity = _ambientIntensity;
    }
}
