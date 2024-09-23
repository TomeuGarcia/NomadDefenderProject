using System;
using DG.Tweening;
using UnityEngine;

public class TutorialViewUtilities : MonoBehaviour, ITutorialViewUtilities
{
    [Header("WARNING TOP BAR")] 
    [SerializeField] private SpriteRenderer _warningTopBarSprite;
    private Material _warningTopBarMaterial;
    private int _warningTopBarActivatePropertyId;

    private void Awake()
    {
        ServiceLocator.GetInstance().TutorialViewUtilities = this;
        
        _warningTopBarMaterial = _warningTopBarSprite.material;
        _warningTopBarActivatePropertyId = Shader.PropertyToID("_Activate");
        _warningTopBarMaterial.SetFloat(_warningTopBarActivatePropertyId, 0);
    }

    private void OnDestroy()
    {
        ServiceLocator.GetInstance().TutorialViewUtilities = null;
    }

    public void ShowWarningTopBar()
    {
        _warningTopBarMaterial.DOComplete();
        _warningTopBarMaterial.DOFloat(1, _warningTopBarActivatePropertyId, 0.2f);
    }

    public void HideWarningTopBar()
    {
        _warningTopBarMaterial.DOComplete();
        _warningTopBarMaterial.DOFloat(0, _warningTopBarActivatePropertyId, 0.2f);
    }
}