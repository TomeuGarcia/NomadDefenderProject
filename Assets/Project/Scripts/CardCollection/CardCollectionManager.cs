using System;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts.CardCollection;
using UnityEngine;
using UnityEngine.UI;

public class CardCollectionManager : MonoBehaviour
{
    [Header("SETUP")] 
    [SerializeField] private CardCollectionDataStorage _cardCollection;
    [SerializeField] private Camera _camera;
    [SerializeField] private CardMotionConfig _cardMotionConfig;
    [SerializeField] private Button _backButton;

    [Header("ANIMATIONS")] 
    [SerializeField] private CardCollectionAnimations _animations;
    
    [Header("PROJECTILES")]
    [SerializeField] private CardCollectionPositioner _projectileCardsPositioner;
    [SerializeField] private CardPartAttack _cardPartProjectilePrefab;
    private CardCollectionCardPartsGroup _projectilesGroup;
    
    
    [Header("PASSIVE ABILITIES")]
    [SerializeField] private CardCollectionPositioner _passiveAbilityCardsPositioner;
    [SerializeField] private CardPartBase _cardPartBasePrefab;
    private CardCollectionCardPartsGroup _passiveAbilitiesGroup;
    
    
    private void Awake()
    {
        CardTooltipDisplayManager.GetInstance().SetDisplayCamera(_camera);
        ServiceLocator.GetInstance().CameraHelp.SetCardsCamera(_camera);
        _cardMotionConfig.SetCardCollectionDisplayMode();

        InitProjectiles(_cardCollection.Projectiles, out Transform[] projectileCardTransforms);
        InitPassiveAbilities(_cardCollection.PassiveAbilities, out Transform[] passiveAbilityCardTransforms);
        StartCoroutine(PlaySceneStartAnimation(projectileCardTransforms, passiveAbilityCardTransforms));
        
        _backButton.onClick.AddListener(SceneLoader.GetInstance().StartLoadMainMenu);
    }
    


    private void InitProjectiles(TurretPartProjectileDataModel[] projectiles,
        out Transform[] projectileCardTransforms)
    {
        CardPart[] projectileCards = new CardPart[projectiles.Length];
        projectileCardTransforms = new Transform[projectiles.Length];

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
            
            projectileCards[i] = projectileCard;
            projectileCardTransforms[i] = projectileCard.transform;

        }

        _projectilesGroup = new CardCollectionCardPartsGroup(projectileCards);

        _projectileCardsPositioner.SetupCards(projectileCardTransforms);
    }



    private void InitPassiveAbilities(ATurretPassiveAbilityDataModel[] passiveAbilities,
        out Transform[] passiveAbilityCardTransforms)
    {
        CardPart[] passiveAbilityCards = new CardPart[passiveAbilities.Length];
        passiveAbilityCardTransforms = new Transform[passiveAbilities.Length];

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
            
            passiveAbilityCards[i] = passiveAbilityCard;
            passiveAbilityCardTransforms[i] = passiveAbilityCard.transform;
        }

        _passiveAbilitiesGroup = new CardCollectionCardPartsGroup(passiveAbilityCards);

        _passiveAbilityCardsPositioner.SetupCards(passiveAbilityCardTransforms);
    }
    
    

    private IEnumerator PlaySceneStartAnimation(Transform[] projectileCardTransforms, 
        Transform[] passiveAbilityCardTransforms)
    {
        yield return new WaitForSeconds(1.0f);
        _animations.PlayProjectiles();
        
        yield return StartCoroutine(_projectileCardsPositioner.PositionCards(projectileCardTransforms));
        _projectilesGroup.StartCardsInteraction();

        yield return StartCoroutine(_passiveAbilityCardsPositioner.PositionCards(passiveAbilityCardTransforms));
        _passiveAbilitiesGroup.StartCardsInteraction();
    }
    
    
    
    
}
