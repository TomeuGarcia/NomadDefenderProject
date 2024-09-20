using System;
using System.Collections;
using UnityEngine;

public class CardDrawAnimationPlayer_TutorialCardOverview : CardDrawAnimationPlayer
{
    private void Awake()
    {
        _objectsToShow = new[]
        {
            _cardLevelText,
            _playCostText,
            _turretPreview,
            _projectileIcon,
            _damageStat,
            _shotsPerSecondStat,
            _rangeStat,
            
            _tutorial_Projectile,
            _tutorial_PlayCost,
            _tutorial_Damage,
            _tutorial_ShotsPerSecond,
            _tutorial_Range,
        };
    }

    [Header("CARD OBJECTS")]
    [SerializeField] private GameObject _cardLevelText;
    [SerializeField] private GameObject _playCostText;
    [SerializeField] private GameObject _turretPreview;
    [SerializeField] private GameObject _projectileIcon;
    [SerializeField] private GameObject _damageStat;
    [SerializeField] private GameObject _shotsPerSecondStat;
    [SerializeField] private GameObject _rangeStat;

    [Header("TUTORIAL OBJECTS")] 
    [SerializeField] private GameObject _tutorial_Projectile;
    [SerializeField] private GameObject _tutorial_PlayCost;
    [SerializeField] private GameObject _tutorial_Damage;
    [SerializeField] private GameObject _tutorial_ShotsPerSecond;
    [SerializeField] private GameObject _tutorial_Range;
    [SerializeField] private TextDecoder _tutorialText_Projectile;
    [SerializeField] private TextDecoder _tutorialText_PlayCost;
    [SerializeField] private TextDecoder _tutorialText_Damage;
    [SerializeField] private TextDecoder _tutorialText_ShotsPerSecond;
    [SerializeField] private TextDecoder _tutorialText_Range;
    
    public bool CanStartShowing { get; set; }
    public bool ShowTurret { get; set; }
    public bool ShowProjectile { get; set; }
    public bool ShowPlayCost { get; set; }
    public bool ShowDamageStat { get; set; }
    public bool ShowShotsPerSecondStat { get; set; }
    public bool ShowRangeStat { get; set; }
    public bool Finish { get; set; }

    
    private GameObject _currentTutorialObject;
    
    
    protected override IEnumerator ShowObjects()
    {
        float t1 = 0.065f;
        
        // Show Turret & Projectile
        yield return new WaitUntil(() => CanStartShowing);
        yield return new WaitUntil(() => ShowTurret);
        yield return StartCoroutine(ShowObjectBlinking(_turretPreview, t1, GameAudioManager.GetInstance().PlayCardInfoMoveShown));
        
        
        yield return new WaitUntil(() => ShowProjectile);
        StartCoroutine(PlayTutorial(_tutorial_Projectile, _tutorialText_Projectile));
        yield return StartCoroutine(ShowObjectBlinking(_projectileIcon, t1, GameAudioManager.GetInstance().PlayCardInfoMoveShown));
        
        
        // Show Damage Stat
        yield return new WaitUntil(() => ShowDamageStat);
        yield return StartCoroutine(PlayNextTutorial(_tutorial_Damage, _tutorialText_Damage));
        yield return StartCoroutine(ShowObjectBlinking(_damageStat, t1, GameAudioManager.GetInstance().PlayCardInfoShown));

        
        
        // Show Shots per Second Stat
        yield return new WaitUntil(() => ShowShotsPerSecondStat);
        yield return StartCoroutine(PlayNextTutorial(_tutorial_ShotsPerSecond, _tutorialText_ShotsPerSecond));
        yield return StartCoroutine(ShowObjectBlinking(_shotsPerSecondStat, t1, GameAudioManager.GetInstance().PlayCardInfoShown));

        
        
        // Show Range Stat
        yield return new WaitUntil(() => ShowRangeStat);
        yield return StartCoroutine(PlayNextTutorial(_tutorial_Range, _tutorialText_Range));
        yield return StartCoroutine(ShowObjectBlinking(_rangeStat, t1, GameAudioManager.GetInstance().PlayCardInfoShown));
        
        
        
        // Show Play Cost
        yield return new WaitUntil(() => ShowPlayCost);
        GameObject[] playCostElements = { _playCostText, _cardLevelText };
        yield return StartCoroutine(PlayNextTutorial(_tutorial_PlayCost, _tutorialText_PlayCost));
        yield return StartCoroutine(ShowObjectsBlinking(playCostElements, t1, GameAudioManager.GetInstance().PlayCardInfoShown));

        
        yield return new WaitUntil(() => Finish);
        FinishTutorial();
    }


    private IEnumerator PlayTutorial(GameObject nextTutorialObject, TextDecoder textDecoder, float delay = 0.5f)
    {
        yield return new WaitForSeconds(delay);
        _currentTutorialObject?.SetActive(false);
        
        if (Finish) yield break;
        
        _currentTutorialObject = nextTutorialObject;
        _currentTutorialObject.SetActive(true);
        
        textDecoder.Activate();
    }

    private IEnumerator PlayNextTutorial(GameObject nextTutorialObject, TextDecoder textDecoder, float transitionDelay = 0.5f)
    {
        _currentTutorialObject?.SetActive(false);

        yield return new WaitForSeconds(transitionDelay);
        
        if (Finish) yield break;
        
        StartCoroutine(PlayTutorial(nextTutorialObject, textDecoder));
    }

    private void FinishTutorial()
    {
        _currentTutorialObject.SetActive(false);
        
        _tutorial_Projectile.SetActive(false);
        _tutorial_PlayCost.SetActive(false);
        _tutorial_Damage.SetActive(false);
        _tutorial_ShotsPerSecond.SetActive(false);
        _tutorial_Range.SetActive(false);
        
        _currentTutorialObject = null;
    }
    
}