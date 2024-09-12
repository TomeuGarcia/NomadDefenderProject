using System;
using System.Collections;
using UnityEngine;

public class TutorialCardDrawAnimationPlayer : CardDrawAnimationPlayer
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
            _rangeStat
        };
    }

    [SerializeField] private GameObject _cardLevelText;
    [SerializeField] private GameObject _playCostText;
    [SerializeField] private GameObject _turretPreview;
    [SerializeField] private GameObject _projectileIcon;
    [SerializeField] private GameObject _damageStat;
    [SerializeField] private GameObject _shotsPerSecondStat;
    [SerializeField] private GameObject _rangeStat;
    
    public bool CanStartShowing { get; set; }
    public bool ShowTurret { get; set; }
    public bool ShowPlayCost { get; set; }
    public bool ShowDamageStat { get; set; }
    public bool ShowShotsPerSecondStat { get; set; }
    public bool ShowRangeStat { get; set; }
    
    
    protected override IEnumerator ShowObjects()
    {
        float t1 = 0.065f;
        
        // Show Turret & Projectile
        yield return new WaitUntil(() => CanStartShowing);
        yield return new WaitUntil(() => ShowTurret);
        GameObject[] turretElements = { _turretPreview, _projectileIcon };
        yield return StartCoroutine(ShowObjectsBlinking(turretElements, t1, GameAudioManager.GetInstance().PlayCardInfoMoveShown));
        
        // Show Play Cost
        yield return new WaitUntil(() => ShowPlayCost);
        GameObject[] playCostElements = { _playCostText, _cardLevelText };
        yield return StartCoroutine(ShowObjectsBlinking(playCostElements, t1, GameAudioManager.GetInstance().PlayCardInfoShown));

        // Show Damage Stat
        yield return new WaitUntil(() => ShowDamageStat);
        yield return StartCoroutine(ShowObjectBlinking(_damageStat, t1, GameAudioManager.GetInstance().PlayCardInfoShown));

        // Show Shots per Second Stat
        yield return new WaitUntil(() => ShowShotsPerSecondStat);
        yield return StartCoroutine(ShowObjectBlinking(_shotsPerSecondStat, t1, GameAudioManager.GetInstance().PlayCardInfoShown));

        // Show Range Stat
        yield return new WaitUntil(() => ShowRangeStat);
        yield return StartCoroutine(ShowObjectBlinking(_rangeStat, t1, GameAudioManager.GetInstance().PlayCardInfoShown));
    }
}