using DG.Tweening;
using UnityEngine;

public class GUMCableFillCheckpoint : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _timeToFill;
    [SerializeField] private Ease _ease;

    [SerializeField] private MeshRenderer _checkpointMesh;
    [SerializeField] private GUMCableFillCheckpoint[] _nextCheckpoints;
    [SerializeField] private MeshRenderer[] _meshes;
    private Material[] _materials;

    private void Awake()
    {
        _materials = new Material[_meshes.Length];

        for(int i = 0; i < _meshes.Length; i++)
        {
            _materials[i] = _meshes[i].material;
        }
    }

    public void StartReadyFill()
    {
        Fill(1.0f, "_ReadyCoef", _timeToFill, _ease);
    }
    public void StartUnReadyFill()
    {
        Fill(0.0f, "_ReadyCoef", _timeToFill, _ease);
    }
    public void StartUpgradingFill()
    {
        Fill(1.0f, "_UpgradingCoef", _timeToFill * 2.0f, _ease);
    }

    public void ShutDown()
    {
        foreach (Material mat in _materials)
        {
            mat.SetFloat("_ReadyCoef", 0.0f);
            mat.SetFloat("_UpgradingCoef", 0.0f);
        }
        if (_checkpointMesh != null)
        {
            _checkpointMesh.materials[2].SetFloat("_ReadyCoef", 0.0f);
            _checkpointMesh.materials[2].SetFloat("_UpgradingCoef", 0.0f);
        }
        foreach (var next in _nextCheckpoints)
        {
            next.ShutDown();
        }
    }

    public void Fill(float endValue, string propertyName, float duration, Ease ease)
    {
        Sequence sequence = DOTween.Sequence();

        foreach (Material mat in _materials)
        {
            sequence.Join(mat.DOFloat(endValue, propertyName, duration * Random.Range(0.1f, 0.9f))).SetEase(ease);
        }
        
        sequence.OnComplete(() => {
            FillCompleted(endValue, propertyName, duration, ease);
        });
    }

    private void FillCompleted(float endValue, string propertyName, float duration, Ease ease)
    {
        if(_checkpointMesh != null)
        {
            _checkpointMesh.materials[2].SetFloat(propertyName, endValue);
        }
        foreach (var next in _nextCheckpoints)
        {
            next.Fill(endValue, propertyName, duration, ease);
        }
    }
}
