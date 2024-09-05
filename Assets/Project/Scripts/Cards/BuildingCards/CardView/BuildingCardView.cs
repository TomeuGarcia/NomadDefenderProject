using System;
using TMPro;
using UnityEngine;

public class BuildingCardView : MonoBehaviour
{
    [Header("CONFIG")] 
    [SerializeField] private BuildingCardViewConfig _config;
    
    [Header("MESHES")]
    [SerializeField] private MeshRenderer _borderMesh;
    [SerializeField] private MeshRenderer _backgroundMesh;
    [SerializeField] private TMP_Text _levelText;

    [Header("DRAW ANIMATION")]
    [SerializeField] private CardDrawAnimationPlayer _drawAnimationPlayer;

    private Material _borderMaterial;
    private Material _backgroundMaterial;
    private int _canBePlayedPropertyId;
    private int _timeStartCanNotBePlayedPropertyId;
    
    public CardDrawAnimationPlayer DrawAnimationPlayer => _drawAnimationPlayer;

    private bool _alreadyConfigured = false;

    public void Configure()
    {
        if (_alreadyConfigured)
        {
            return;
        }
        _alreadyConfigured = true;
        
        _borderMaterial = _borderMesh.material;
        _backgroundMaterial = _backgroundMesh.material;
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
    
    
    public void UpdateCardLevelView(int cardLevel)
    {
        _backgroundMaterial.SetFloat("_Level", cardLevel);
        _levelText.fontMaterial = cardLevel == TurretCardDataModel.MAX_CARD_LEVEL 
            ? _config.MaxCardLevelTextMaterial 
            : _config.DefaultCardLevelTextMaterial;
    }
}