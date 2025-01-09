using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplicateSelectedCardIndicator : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> _indicatorMeshes = new();
    [SerializeField] private Material _offMaterial;
    [SerializeField] private Material _regularMaterial;
    [SerializeField] private Material _selectedMaterial;

    private int _materialIndex = 2;

    private void Awake()
    {
        //TurnOff();
    }

    public void TurnOn(int cardAmount = 5)
    {
        for (int i = 0; i < cardAmount; i++)
        {
            ChangeMaterial(i, _regularMaterial);
        }
    }

    public void SelectCard(int cardIndex)
    {
        ChangeMaterial(cardIndex, _selectedMaterial);
    }

    public void UnselectCard(int cardIndex)
    {
        ChangeMaterial(cardIndex, _regularMaterial);
    }

    public void TurnOff()
    {
        for (int i = 0; i < _indicatorMeshes.Count; i++)
        {
            _indicatorMeshes[i].materials[_materialIndex] = _offMaterial;
        }
    }

    private void ChangeMaterial(int index, Material mat)
    {
        Material[] materials = _indicatorMeshes[index].materials;
        materials[_materialIndex] = mat;
        _indicatorMeshes[index].materials = materials;
    }
}
