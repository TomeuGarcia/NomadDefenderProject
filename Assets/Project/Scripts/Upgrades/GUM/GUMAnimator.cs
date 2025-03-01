using UnityEngine;

public class GUMAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private GUMCableFillCheckpoint[] _cableFillStarts;
    [SerializeField] private GUMSideIndicator _leftIndicator;
    [SerializeField] private GUMSideIndicator _rightIndicator;

    private void Awake()
    {
        
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
    }

    public void Enter()
    {

    }

    public void LeftReady()
    {
        _leftIndicator.Ready();
    }
    public void LeftUnReady()
    {
        _leftIndicator.UnReady();
    }

    public void RightReady()
    {
        _rightIndicator.Ready();
    }
    public void RightUnReady()
    {
        _rightIndicator.UnReady();
    }

    public void Ready()
    {
        foreach (GUMCableFillCheckpoint checkpoint in _cableFillStarts)
        {
            checkpoint.StartReadyFill();
        }
    }
    public void UnReady()
    {
        foreach (GUMCableFillCheckpoint checkpoint in _cableFillStarts)
        {
            checkpoint.StartUnReadyFill();
        }
    }

    public void Upgrading()
    {
        _leftIndicator.Upgrading();
        _rightIndicator.Upgrading();

        foreach (GUMCableFillCheckpoint checkpoint in _cableFillStarts)
        {
            checkpoint.StartUpgradingFill();
        }
    }
}
