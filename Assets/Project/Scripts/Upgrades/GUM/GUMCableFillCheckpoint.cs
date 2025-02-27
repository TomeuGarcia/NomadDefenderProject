using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GUMCableFillCheckpoint : MonoBehaviour
{
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

    public void Fill()
    {
        //_materials.D
    }

    private void FillCompleted()
    {
        foreach (var next in _nextCheckpoints)
        {
            next.Fill();
        }
    }
}
