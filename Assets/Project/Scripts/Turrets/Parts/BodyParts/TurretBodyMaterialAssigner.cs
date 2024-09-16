using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TurretBodyMaterialAssigner
{
    [SerializeField] private MeshRenderer _mesh;
    [SerializeField] private int[] _materialIndices = { 0 };

    private List<Material> _materialsCopy;
    
    public void Init()
    {
        _materialsCopy = new List<Material>(_mesh.materials);
    }
    
    public void Assign(Material material)
    {
        foreach (int materialIndex in _materialIndices)
        {
            _materialsCopy[materialIndex] = material;
        }
        _mesh.SetMaterials(_materialsCopy);
    }

    public void AssignToAll(Material material)
    {
        List<Material> materials = new List<Material>(_mesh.materials.Length);
        for (int i = 0; i < _materialsCopy.Count; ++i)
        {
            materials.Add(material);
        }
        _mesh.SetMaterials(materials);
    }
}