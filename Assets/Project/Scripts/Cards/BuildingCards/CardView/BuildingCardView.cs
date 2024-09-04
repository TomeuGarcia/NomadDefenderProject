using System;
using UnityEngine;

public class BuildingCardView : MonoBehaviour
{
    [SerializeField] private MeshRenderer _borderMesh;
    [SerializeField] private CardDrawAnimationPlayer _drawAnimationPlayer;

    private Material _borderMaterial;
    private int _canBePlayedPropertyId;
    private int _timeStartCanNotBePlayedPropertyId;
    
    public CardDrawAnimationPlayer DrawAnimationPlayer => _drawAnimationPlayer;
    

    public void Configure()
    {
        _borderMaterial = _borderMesh.material;
        _drawAnimationPlayer.Configure(_borderMaterial);
        
        _canBePlayedPropertyId = Shader.PropertyToID("_CanBePlayed");
        _timeStartCanNotBePlayedPropertyId = Shader.PropertyToID("_TimeStartCanNotBePlayed");
    }


    public void SetCanBePlayed(bool canBePlayed)
    {
        _borderMaterial.SetFloat(_canBePlayedPropertyId, canBePlayed ? 1 : 0);
    }

    public void PlayCanNotBePlayedAnimation()
    {
        _borderMaterial.SetFloat(_timeStartCanNotBePlayedPropertyId, Time.time);
    }
    
}