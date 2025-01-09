using System;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts.CardCollection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardCollectionManager : MonoBehaviour
{
    [Header("SETUP")] 
    [SerializeField] private CardCollectionDataStorage _cardCollection;
    [SerializeField] private Camera _camera;
    [SerializeField] private CardMotionConfig _cardMotionConfig;
    [SerializeField] private Button _backButton;
    [SerializeField] private TextDecoder _discoveredPercentText;
    [SerializeField] private TextDecoder _discoveriesText;

    [Header("PROJECTILES")]
    [SerializeField] private CardCollectionPositioner _projectileCardsPositioner;
    [SerializeField] private CardPartAttack _cardPartProjectilePrefab;
    private CardCollectionCardPartsGroup _projectilesGroup;
    
    
    [Header("PASSIVE ABILITIES")]
    [SerializeField] private CardCollectionPositioner _passiveAbilityCardsPositioner;
    [SerializeField] private CardPartBase _cardPartBasePrefab;
    private CardCollectionCardPartsGroup _passiveAbilitiesGroup;
    
    
    
    
    private void Start()
    {
        CardTooltipDisplayManager.GetInstance().SetDisplayCamera(_camera);
        ServiceLocator.GetInstance().CameraHelp.SetCardsCamera(_camera);
        _cardMotionConfig.SetCardCollectionDisplayMode();

        InitProjectiles(_cardCollection.Projectiles, out Transform[] projectileCardTransforms, out int discoveredProjectilesCount);
        InitPassiveAbilities(_cardCollection.PassiveAbilities, out Transform[] passiveAbilityCardTransforms, out int discoveredPassivesCount);
        InitDiscoveredPercent(discoveredProjectilesCount, projectileCardTransforms.Length,
            discoveredPassivesCount, passiveAbilityCardTransforms.Length);
        StartCoroutine(PlaySceneStartAnimation(projectileCardTransforms, passiveAbilityCardTransforms));
        
        _backButton.onClick.AddListener(SceneLoader.GetInstance().LoadFacility);
    }
    


    private void InitProjectiles(TurretPartProjectileDataModel[] projectiles,
        out Transform[] projectileCardTransforms, out int discoveredCount)
    {
        CardPart[] projectileCards = new CardPart[projectiles.Length];
        projectileCardTransforms = new Transform[projectiles.Length];
        discoveredCount = 0;

        for (int i = 0; i < projectiles.Length; ++i)
        {
            TurretPartProjectileDataModel projectile = projectiles[i];
            CardPartAttack projectileCard = Instantiate(_cardPartProjectilePrefab, transform);
            projectileCard.Configure(projectile);
            projectileCard.Init();
            if (!_cardCollection.WasDiscovered(projectile))
            {
                projectileCard.SetNotDiscovered();
            }
            else
            {
                ++discoveredCount;
            }
            
            projectileCards[i] = projectileCard;
            projectileCardTransforms[i] = projectileCard.transform;

        }

        _projectilesGroup = new CardCollectionCardPartsGroup(projectileCards);

        _projectileCardsPositioner.SetupCards(projectileCardTransforms);
    }



    private void InitPassiveAbilities(ATurretPassiveAbilityDataModel[] passiveAbilities,
        out Transform[] passiveAbilityCardTransforms, out int discoveredCount)
    {
        CardPart[] passiveAbilityCards = new CardPart[passiveAbilities.Length];
        passiveAbilityCardTransforms = new Transform[passiveAbilities.Length];
        discoveredCount = 0;

        for (int i = 0; i < passiveAbilities.Length; ++i)
        {
            ATurretPassiveAbilityDataModel passiveAbility = passiveAbilities[i];
            CardPartBase passiveAbilityCard = Instantiate(_cardPartBasePrefab, transform);
            passiveAbilityCard.SetTurretPassive(passiveAbility);
            passiveAbilityCard.Init();
            if (!_cardCollection.WasDiscovered(passiveAbility))
            {
                passiveAbilityCard.SetNotDiscovered();
            }
            else
            {
                ++discoveredCount;
            }
            
            passiveAbilityCards[i] = passiveAbilityCard;
            passiveAbilityCardTransforms[i] = passiveAbilityCard.transform;
        }

        _passiveAbilitiesGroup = new CardCollectionCardPartsGroup(passiveAbilityCards);

        _passiveAbilityCardsPositioner.SetupCards(passiveAbilityCardTransforms);
    }


    private void InitDiscoveredPercent(
        int discoveredProjectilesCount, int totalProjectiles,
        int discoveredPassivesCount, int totalPassives)
    {
        float totalDiscovered = Mathf.Max(0, discoveredProjectilesCount + discoveredPassivesCount);
        float totalCount = Mathf.Max(1, totalProjectiles + totalPassives);
        int percent = Mathf.RoundToInt((totalDiscovered / totalCount) * 100);
        
        _discoveredPercentText.ResetDecoder();
        _discoveredPercentText.textStrings.Add(percent.ToString() + '%');
    }
    
    

    private IEnumerator PlaySceneStartAnimation(Transform[] projectileCardTransforms, 
        Transform[] passiveAbilityCardTransforms)
    {
        yield return new WaitForSeconds(1.0f);
        
        yield return StartCoroutine(_projectileCardsPositioner.PositionCards(projectileCardTransforms));
        _projectilesGroup.StartCardsInteraction();

        yield return StartCoroutine(_passiveAbilityCardsPositioner.PositionCards(passiveAbilityCardTransforms));
        _passiveAbilitiesGroup.StartCardsInteraction();
        
        _discoveriesText.Activate();
        yield return new WaitUntil(() => _discoveriesText.FinishedLine);
        _discoveredPercentText.Activate();
    }
    
    
    
    
}
