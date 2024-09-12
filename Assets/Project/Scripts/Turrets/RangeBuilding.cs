using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public abstract class RangeBuilding : Building
{
    protected List<Enemy> enemies = new List<Enemy>();
    public List<Enemy> Enemies => enemies;

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

    public delegate void RangeBuildingAction2();
    public RangeBuildingAction2 OnBuildingUpgraded;
    public RangeBuildingAction2 OnShowRangePlane;
    public RangeBuildingAction2 OnHideRangePlane;


    [SerializeField] protected InBattleBuildingUpgrader upgrader;
    public InBattleBuildingUpgrader Upgrader => upgrader;

    public Vector3 Position => transform.position;

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
        if (!enemy.DiesFromQueuedDamage() && !enemies.Contains(enemy))
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
        enemy.OnEnemyDeactivated += DeleteEnemyFromList;
        enemies.Add(enemy);
        if (OnEnemyEnterRange != null) OnEnemyEnterRange(enemy);
        //SortEnemies();
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

        //SortEnemies();
    }

    private void SortEnemies()
    {
        enemies.Sort(DoSortEnemies);
    }

    public Enemy GetBestEnemyTarget(Enemy currentlyTargetedEnemy)
    {
        SortEnemies();

        if (currentlyTargetedEnemy != null &&
            currentlyTargetedEnemy.CanBeTargeted() &&
            !currentlyTargetedEnemy.DiesFromQueuedDamage() &&
            enemies.Contains(currentlyTargetedEnemy))
        {

            foreach (Enemy enemy in enemies)
            {
                if (enemy.GetTargetPriorityBonus() < currentlyTargetedEnemy.GetTargetPriorityBonus())
                {
                    currentlyTargetedEnemy = enemy;
                }
            }
            
            return currentlyTargetedEnemy;
        }



        int enemyI = 0;
        while (enemyI < enemies.Count && !enemies[enemyI].CanBeTargeted())
        {
            ++enemyI;
        }

        Enemy targetedEnemy = null;
        if (enemyI < enemies.Count)
        {
            targetedEnemy = enemies[enemyI];
        }
        return targetedEnemy;
    }

    private int DoSortEnemies(Enemy e1, Enemy e2)
    {
        return enemySortFunction(e1, e2);
    }

    public static int SortByDistanceLeftToEnd(Enemy e1, Enemy e2)
    {
        float enemy1Value = e1.PathFollower.DistanceLeftToEnd + e1.GetTargetPriorityBonus();
        float enemy2Value = e2.PathFollower.DistanceLeftToEnd + e2.GetTargetPriorityBonus();
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

    public Enemy[] GetEnemiesInRange()
    {
        return enemies.ToArray();
    }

    public override void ShowRangePlane()
    {        
        basePart.baseCollider.ShowRange();
        if (OnShowRangePlane != null) OnShowRangePlane();
    }
    public override void HideRangePlane()
    {
        basePart.baseCollider.HideRange();
        if (OnHideRangePlane != null) OnHideRangePlane();
    }
    protected abstract void UpdateRange();


    public void ShowUpgrades()
    {
        if (Upgrader.CanOpenWindow()) Upgrader.OpenWindow();
    }
    public void HideUpgrades()
    {
        Upgrader.CloseWindow();
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


    public void InvokeOnBuildingUpgraded()
    {
        if (OnBuildingUpgraded != null) OnBuildingUpgraded();
    }
}
