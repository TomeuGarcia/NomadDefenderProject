using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSpawnerHealth : MonoBehaviour
{
    public enum SpawnType { ON_DEATH, REPEATEDLY }
    [SerializeField] AreaFunctionalityArmor.AreaType areaType;
    [SerializeField] SpawnType spawnType;
    [SerializeField] float spawnRate;
    [SerializeField] int healthAmount;
    GameObject currentWave;

    [Header("FEEDBACK")]
    [SerializeField] EnemyFeedback enemyFeedback;

    private void OnEnable()
    {
        InitAreaSpawn();
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
                if (currentWave)
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
                InvokeRepeating("AreaCooldown", spawnRate, spawnRate);
                break;
        }
    }

    public void StopRepeating()
    {
        if (spawnType == SpawnType.REPEATEDLY)
        {
            CancelInvoke("AreaCooldown");
        }
    }

    void AreaCooldown()
    {
        enemyFeedback.AreaOnCooldown(spawnRate);
        SpawnWave();
    }

    void SpawnWave()
    {
        currentWave = AreaStateFactory.GetInstance().GetAreaGameObject(areaType, transform.position + transform.up * 0.25f, Quaternion.identity, null);
        currentWave.SetActive(true);
        currentWave.GetComponent<AreaFunctionalityHealth>().SetHealthToAdd(healthAmount);
        currentWave.GetComponent<AreaFunctionalityHealth>().Follow(transform);
    }
}
