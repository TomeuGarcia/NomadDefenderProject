using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSpawnerArmor : MonoBehaviour
{
    public enum SpawnType { ON_DEATH, REPEATEDLY}
    [SerializeField] AreaFunctionalityArmor.AreaType areaType;
    [SerializeField] SpawnType spawnType;
    [SerializeField] float spawnRate;
    [SerializeField] int armorAmount;
    GameObject currentWave;

    private Coroutine repeatedAreaSpawn = null;

    [Header("FEEDBACK")]
    [SerializeField] EnemyFeedback enemyFeedback;

    private void OnEnable()
    {
        if (gameObject.activeInHierarchy) InitAreaSpawn();
    }
    private void OnDisable()
    {
        DisableArea();
    }
    public void DisableArea()
    {
        switch (spawnType)
        {
            case SpawnType.ON_DEATH:
                SpawnWave();
                break;

            case SpawnType.REPEATEDLY:
                StopRepeating();
                if(currentWave)
                currentWave.GetComponent<AreaFunctionalityArmor>().StopFollowing();
                break;
        }
    }
    public void InitAreaSpawn()
    {
        switch (spawnType)
        {
            case SpawnType.ON_DEATH:
                break;

            case SpawnType.REPEATEDLY:
                //should I add StopRepeating() to a OnDeath event?
                enemyFeedback.AreaOnCooldown(spawnRate);
                //InvokeRepeating("AreaCooldown", spawnRate, spawnRate);
                repeatedAreaSpawn = StartCoroutine(SpawnAreaRepeatedly());
                break;
        }
    }

    public void StopRepeating()
    {
        if (spawnType == SpawnType.REPEATEDLY)
        {
            //CancelInvoke("AreaCooldown");
            if (repeatedAreaSpawn != null)
            {
                StopCoroutine(repeatedAreaSpawn);
            }
            repeatedAreaSpawn = null;
        }
    }

    private IEnumerator SpawnAreaRepeatedly()
    {
        Timer delayTimer = new Timer(spawnRate);
        
        while (gameObject.activeInHierarchy)
        {
            while (!delayTimer.HasFinished())
            {
                delayTimer.Update(GameTime.DeltaTime);
                yield return null;
            }
            
            delayTimer.Reset();
            AreaCooldown();
        }
    }


    void AreaCooldown()
    {
        enemyFeedback.AreaOnCooldown(spawnRate);
        SpawnWave();
    }

    void SpawnWave()
    {
        currentWave = AreaStateFactory.GetInstance().GetAreaGameObject(areaType, transform.position + transform.up * 0.25f , Quaternion.identity, null);
        currentWave.SetActive(true);
        currentWave.GetComponent<AreaFunctionalityArmor>().SetArmorToAdd(armorAmount);
        currentWave.GetComponent<AreaFunctionalityArmor>().Follow(transform);
    }
}
