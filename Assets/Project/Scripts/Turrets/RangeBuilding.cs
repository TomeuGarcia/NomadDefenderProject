using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public abstract class RangeBuilding : Building
{
    protected List<Enemy> enemies = new List<Enemy>();

    [Header("COMPONENTS")]
    [SerializeField] private MouseOverNotifier meshMouseNotifier;
    //[SerializeField] protected GameObject rangePlaneMeshObject;
    //protected Material rangePlaneMaterial;

    //[Header("COLLIDER")]
    //[SerializeField] protected CapsuleCollider rangeCollider;

    private TriggerNotifier triggerNotifier;

    protected TurretPartBase_Prefab basePart; // Consider all ranged buildings have a BasePart
    


    public delegate int EnemySortFunction(Enemy e1, Enemy e2);
    private EnemySortFunction enemySortFunction;

    public delegate void RangeBuildingAction(Enemy enemy);
    public RangeBuildingAction OnEnemyEnterRange;
    public RangeBuildingAction OnEnemyExitRange;


    [SerializeField] protected InBattleBuildingUpgrader upgrader;


    private void OnEnable()
    {
        if (triggerNotifier != null) { SubscribeToTriggerNotifier(); }
    }
    private void OnDisable()
    {
        if (triggerNotifier != null) { UnsubscribeToTriggerNotifier(); }
    }


    private void Awake()
    {
        AwakeInit();
    }
    protected override void AwakeInit()
    {
        ResetEnemySortFunction();
    }

    public TurretPartBase_Prefab GetBasePart()
    {
        return basePart;
    }

    private void OnTriggerEnterNotif(Collider other)
    {
        if (other.tag == "Enemy")
        {
            OnEnemyEnters(other.gameObject.GetComponent<Enemy>());
        }
    }

    private void OnTriggerExitNotif(Collider other)
    {
        if (other.tag == "Enemy")
        {
            OnEnemyExits(other.gameObject.GetComponent<Enemy>());
        }
    }

    public void SetUpTriggerNotifier(TriggerNotifier triggerNotifier)
    {
        this.triggerNotifier = triggerNotifier;
        SubscribeToTriggerNotifier();
    }

    private void SubscribeToTriggerNotifier()
    {
        triggerNotifier.OnEnter += OnTriggerEnterNotif;
        triggerNotifier.OnExit += OnTriggerExitNotif;
    }
    private void UnsubscribeToTriggerNotifier()
    {
        triggerNotifier.OnEnter -= OnTriggerEnterNotif;
        triggerNotifier.OnExit -= OnTriggerExitNotif;
    }


    private void OnEnemyEnters(Enemy enemy)
    {
        if (!enemy.DiesFromQueuedDamage())
        {
            AddEnemy(enemy);
        }
    }
    private void OnEnemyExits(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            RemoveEnemy(enemy);
        }            
    }


    private void AddEnemy(Enemy enemy)
    {
        if (OnEnemyEnterRange != null) OnEnemyEnterRange(enemy);

        enemy.OnEnemyDeactivated += DeleteEnemyFromList;
        enemies.Add(enemy);
        enemies.Sort(SortEnemies);
    }

    protected void RemoveEnemy(Enemy enemy)
    {
        enemy.OnEnemyDeactivated -= DeleteEnemyFromList;
        DeleteEnemyFromList(enemy);
    }

    private void DeleteEnemyFromList(Enemy enemyToDelete)
    {
        if (OnEnemyExitRange != null) OnEnemyExitRange(enemyToDelete);

        enemies.Remove(enemyToDelete);

        enemies.Sort(SortEnemies);
    }

    private int SortEnemies(Enemy e1, Enemy e2)
    {
        return enemySortFunction(e1, e2);
    }

    public static int SortByDistanceLeftToEnd(Enemy e1, Enemy e2)
    {
        float enemy1Value = e1.pathFollower.DistanceLeftToEnd + e1.GetTargetNegativePriorityBonus();
        float enemy2Value = e2.pathFollower.DistanceLeftToEnd + e2.GetTargetNegativePriorityBonus();
        return enemy1Value.CompareTo(enemy2Value);
    }

    public void SetEnemySortFunction(EnemySortFunction newEnemySortFunction)
    {
        enemySortFunction = newEnemySortFunction;
    }
    public void ResetEnemySortFunction()
    {
        enemySortFunction = SortByDistanceLeftToEnd;
    }


    /// <summary>
    /// Returns a List with size up to "amount", assumes there is at least 1 enemy in the turret's queue
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public Enemy[] GetNearestEnemies(int amount, float thresholdDistance)
    {
        List<Enemy> enemyList = new List<Enemy>();

        enemyList.Add(enemies[0]);

        int currentEnemyI = 1;
        while (currentEnemyI < enemies.Count && enemyList.Count < amount)
        {
            if (!enemyList.Contains(enemies[currentEnemyI]) && !enemies[currentEnemyI].DiesFromQueuedDamage())
            {
                float distance = Vector3.Distance(enemyList[enemyList.Count - 1].transformToMove.position, enemies[currentEnemyI].transformToMove.position);
                if (distance < thresholdDistance)
                {
                    enemyList.Add(enemies[currentEnemyI]);
                    currentEnemyI = 0;
                }
            }

            ++currentEnemyI;
        }

        return enemyList.ToArray();
    }

    public override void ShowRangePlane()
    {
        basePart.baseCollider.ShowRange();
    }
    public override void HideRangePlane()
    {
        basePart.baseCollider.HideRange();
    }
    protected abstract void UpdateRange();


    public void ShowUpgrades()
    {
        if (upgrader.CanOpenWindow()) upgrader.OpenWindow();
    }
    public void HideUpgrades()
    {
        upgrader.CloseWindow();
    }


    protected override void DisableFunctionality()
    {
        base.DisableFunctionality();

        meshMouseNotifier.gameObject.SetActive(false);
    }
    protected override void EnableFunctionality()
    {
        base.EnableFunctionality();

        meshMouseNotifier.gameObject.SetActive(true);
    }

    public override void EnablePlayerInteraction()
    {
        meshMouseNotifier.OnMouseEntered += ShowRangePlane;
        meshMouseNotifier.OnMouseExited += HideRangePlane;

        meshMouseNotifier.OnMousePressed += ShowUpgrades;
        //TODO : HideUpgrades

        meshMouseNotifier.OnMouseEntered += ShowQuickLevelUI;
        meshMouseNotifier.OnMouseExited += HideQuickLevelUI;
    }
    public override void DisablePlayerInteraction()
    {
        meshMouseNotifier.OnMouseEntered -= ShowRangePlane;
        meshMouseNotifier.OnMouseExited -= HideRangePlane;

        meshMouseNotifier.OnMousePressed -= ShowUpgrades;

        meshMouseNotifier.OnMouseEntered -= ShowQuickLevelUI;
        meshMouseNotifier.OnMouseExited -= HideQuickLevelUI;
    }


    public abstract void Upgrade(TurretUpgradeType upgradeType, int newStatLevel);
}
