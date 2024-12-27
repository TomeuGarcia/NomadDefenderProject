using System;
using System.Collections.Generic;
using System.IO;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "CardCollectionDataStorage", 
    menuName = SOAssetPaths.CARDS + "CardCollectionDataStorage")]
public class CardCollectionDataStorage : ScriptableObject
{
    private string PathToFile => Application.streamingAssetsPath + "/JSONfiles/Cards/";
    private string FileName => "CardCollection.json";
    
    [SerializeField] private TurretPartProjectileDataModel[] _projectiles;    
    [SerializeField] private ATurretPassiveAbilityDataModel[] _passiveAbilities;


    public TurretPartProjectileDataModel[] Projectiles => _projectiles;    
    public ATurretPassiveAbilityDataModel[] PassiveAbilities => _passiveAbilities;


    private Dictionary<TurretPartProjectileDataModel, bool> _discoveredProjectiles;
    private Dictionary<ATurretPassiveAbilityDataModel, bool> _discoveredPassiveAbilities;


    [System.Serializable]
    private class DataWrapper
    {
        [System.Serializable]
        public class ProjectileData
        {
            [SerializeField] public string name;
            [SerializeField] public bool wasDiscovered;

            public ProjectileData(KeyValuePair<TurretPartProjectileDataModel,bool> discoveredProjectile)
            {
                name = discoveredProjectile.Key.AbilityName;
                wasDiscovered = discoveredProjectile.Value;
            }
        }
        [System.Serializable]
        public class PassiveAbilityData
        {
            [SerializeField] public string name;
            [SerializeField] public bool wasDiscovered;
            
            public PassiveAbilityData(KeyValuePair<ATurretPassiveAbilityDataModel,bool> discoveredProjectile)
            {
                name = discoveredProjectile.Key.Name;
                wasDiscovered = discoveredProjectile.Value;
            }
        }

        public ProjectileData[] projectilesData;
        public PassiveAbilityData[] passiveAbilitiesData;

        public DataWrapper(Dictionary<TurretPartProjectileDataModel, bool> discoveredProjectiles,
            Dictionary<ATurretPassiveAbilityDataModel, bool> discoveredPassiveAbilities)
        {
            projectilesData = new ProjectileData[discoveredProjectiles.Count];
            int i = 0;
            foreach (KeyValuePair<TurretPartProjectileDataModel,bool> discoveredProjectile in discoveredProjectiles)
            {
                projectilesData[i++] = new ProjectileData(discoveredProjectile);
            }

            passiveAbilitiesData = new PassiveAbilityData[discoveredPassiveAbilities.Count];
            i = 0;
            foreach (KeyValuePair<ATurretPassiveAbilityDataModel,bool> discoveredPassiveAbility in discoveredPassiveAbilities)
            {
                passiveAbilitiesData[i++] = new PassiveAbilityData(discoveredPassiveAbility);
            }
        }
    }


    private void OnEnable()
    {
        LoadData();
    }

    private void OnDisable()
    {
        SaveData();
    }



    [Button()]
    private void LoadData()
    {
        CheckFile();

        string storedContent = File.ReadAllText(PathToFile + FileName);
        DataWrapper storedData = JsonUtility.FromJson<DataWrapper>(storedContent);

        _discoveredProjectiles = new Dictionary<TurretPartProjectileDataModel, bool>(_projectiles.Length);
        foreach (DataWrapper.ProjectileData projectileData in storedData.projectilesData)
        {
            TurretPartProjectileDataModel projectile = ProjectileNameToDataModel(projectileData.name);
            _discoveredProjectiles.Add(projectile, projectileData.wasDiscovered);    
        }
        
        _discoveredPassiveAbilities = new Dictionary<ATurretPassiveAbilityDataModel, bool>(_passiveAbilities.Length);
        foreach (DataWrapper.PassiveAbilityData passiveAbilityData in storedData.passiveAbilitiesData)
        {
            ATurretPassiveAbilityDataModel passiveAbility = PassiveAbilityNameToDataModel(passiveAbilityData.name);
            _discoveredPassiveAbilities.Add(passiveAbility, passiveAbilityData.wasDiscovered);    
        }
    }
    
    [Button()]
    private void SaveData()
    {
        DataWrapper dataToStore = new DataWrapper(_discoveredProjectiles, _discoveredPassiveAbilities);
        
        string contentToStore = JsonUtility.ToJson(dataToStore);
        File.WriteAllText(PathToFile + FileName, contentToStore);
    }


    private void CheckFile()
    {
        string directory = PathToFile;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            ResetDiscoveries();
            SaveData();
        }
    }


    [Button()]
    public void DoReset()
    {
        string directory = PathToFile;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        ResetDiscoveries();
        SaveData();
    }
    
    [Button()]
    private void DoDiscoverAll()
    {
        foreach (TurretPartProjectileDataModel projectile in _projectiles)
        {
            Discover(projectile);
        }
        foreach (ATurretPassiveAbilityDataModel passiveAbility in _passiveAbilities)
        {
            Discover(passiveAbility);
        }
    }
    [Button()]
    private void DoDiscoverRandom()
    {
        foreach (TurretPartProjectileDataModel projectile in _projectiles)
        {
            if (Random.Range(0, 2) < 1) Discover(projectile);
        }
        foreach (ATurretPassiveAbilityDataModel passiveAbility in _passiveAbilities)
        {
            if (Random.Range(0, 2) < 1) Discover(passiveAbility);
        }
    }
    
    

    private TurretPartProjectileDataModel ProjectileNameToDataModel(string name)
    {
        foreach (TurretPartProjectileDataModel projectile in _projectiles)
        {
            if (projectile.AbilityName == name)
            {
                return projectile;
            }
        }

        return null;
    }
    
    private ATurretPassiveAbilityDataModel PassiveAbilityNameToDataModel(string name)
    {
        foreach (ATurretPassiveAbilityDataModel passiveAbility in _passiveAbilities)
        {
            if (passiveAbility.Name == name)
            {
                return passiveAbility;
            }
        }

        return null;
    }

    
    
    public bool WasDiscovered(TurretPartProjectileDataModel projectile)
    {
        return _discoveredProjectiles[projectile];
    }
    public bool WasDiscovered(ATurretPassiveAbilityDataModel passiveAbility)
    {
        return _discoveredPassiveAbilities[passiveAbility];
    }

    public void Discover(TurretPartProjectileDataModel projectile)
    {
        _discoveredProjectiles[projectile] = true;
        CheckAllAreDiscovered();
    }
    public void Discover(ATurretPassiveAbilityDataModel passiveAbility)
    {
        _discoveredPassiveAbilities[passiveAbility] = true;
        CheckAllAreDiscovered();
    }

    private void CheckAllAreDiscovered()
    {
        // TODO check & play achievement
    }

    private void ResetDiscoveries()
    {
        _discoveredProjectiles = new Dictionary<TurretPartProjectileDataModel, bool>(_projectiles.Length);
        foreach (TurretPartProjectileDataModel projectile in _projectiles)
        {
            _discoveredProjectiles.Add(projectile, false);    
        }
        
        _discoveredPassiveAbilities = new Dictionary<ATurretPassiveAbilityDataModel, bool>(_passiveAbilities.Length);
        foreach (ATurretPassiveAbilityDataModel passiveAbility in _passiveAbilities)
        {
            _discoveredPassiveAbilities.Add(passiveAbility, false);    
        }
    }


}
