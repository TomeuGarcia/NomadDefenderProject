using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSpawner : MonoBehaviour
{
    public enum SpawnType { ON_DEATH, REPEATEDLY}
    [SerializeField] AreaFunctionality.AreaType areaType;
    [SerializeField] SpawnType spawnType;
    [SerializeField] float spawnRate;
    GameObject currentWave;

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
                if(currentWave)
                currentWave.GetComponent<AreaFunctionality>().StopFollowing();
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
                InvokeRepeating("SpawnWave", spawnRate, spawnRate);
                break;
        }
    }

    public void StopRepeating()
    {
        if (spawnType == SpawnType.REPEATEDLY)
        {
            CancelInvoke("SpawnWave");
        }
    }

    

    void SpawnWave()
    {
        currentWave = AreaStateFactory.GetInstance().GetAreaGameObject(areaType, transform.position + transform.up * 0.25f , Quaternion.identity, null);
        currentWave.SetActive(true);
        currentWave.GetComponent<AreaFunctionality>().Follow(transform);
        Debug.Log("spawn Area");
    }
}
