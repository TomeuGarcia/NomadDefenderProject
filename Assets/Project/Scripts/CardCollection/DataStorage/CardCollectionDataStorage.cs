using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "CardCollectionDataStorage", 
    menuName = SOAssetPaths.CARDS + "CardCollectionDataStorage")]
public class CardCollectionDataStorage : ScriptableObject
{
    private string PathToFile_StreamingAssets => Application.streamingAssetsPath + "/JSONfiles/Cards/";
    private string PathToFile_Persistent => Application.persistentDataPath + "/Data/";
    private const string FILE_NAME = "CardCollection.json";
    
    
    
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
        SaveData(false);
    }

    
    private readonly CaesarCipher _caesarCipher = new (912);

    [Button()]
    private void LoadData()
    {
        CheckFile();

        string storedContent = _caesarCipher.Decipher(File.ReadAllText(PathToFile_Persistent + FILE_NAME));

        DataWrapper storedData = JsonUtility.FromJson<DataWrapper>(storedContent);

        _discoveredProjectiles = new Dictionary<TurretPartProjectileDataModel, bool>(_projectiles.Length);
        foreach (DataWrapper.ProjectileData projectileData in storedData.projectilesData)
        {
            TurretPartProjectileDataModel projectile = ProjectileNameToDataModel(projectileData.name);
            if (projectile != null)
            {
                _discoveredProjectiles.Add(projectile, projectileData.wasDiscovered);
            }
        }
        
        _discoveredPassiveAbilities = new Dictionary<ATurretPassiveAbilityDataModel, bool>(_passiveAbilities.Length);
        foreach (DataWrapper.PassiveAbilityData passiveAbilityData in storedData.passiveAbilitiesData)
        {
            ATurretPassiveAbilityDataModel passiveAbility = PassiveAbilityNameToDataModel(passiveAbilityData.name);
            if (passiveAbility != null)
            {
                _discoveredPassiveAbilities.Add(passiveAbility, passiveAbilityData.wasDiscovered);
            }
        }

        UpdateMissingData();
    }
    private void UpdateMissingData()
    {
        foreach (TurretPartProjectileDataModel projectile in _projectiles)
        {
            _discoveredProjectiles.TryAdd(projectile, false);
        }
        foreach (ATurretPassiveAbilityDataModel passiveAbility in _passiveAbilities)
        {
            _discoveredPassiveAbilities.TryAdd(passiveAbility, false);
        }
    }
    
    
    
    [Button()]
    private void DebugSaveData()
    {
        SaveData(true);
    }
    
    public void SaveData(bool toPersistent)
    {
        DataWrapper dataToStore = new DataWrapper(_discoveredProjectiles, _discoveredPassiveAbilities);        
        string contentToStore = _caesarCipher.Cipher(JsonUtility.ToJson(dataToStore));

        string path = (toPersistent ? PathToFile_Persistent : PathToFile_StreamingAssets) + FILE_NAME;
        File.WriteAllText(path, contentToStore);
    }


    private void CheckFile()
    {
        /*
        string directory = PathToFile_StreamingAssets;
        string directoryWithFile = directory + FILE_NAME;
        if (!Directory.Exists(directory) || !File.Exists(directoryWithFile))
        {
            Directory.CreateDirectory(directory);
            FileStream fileStream = File.Create(directoryWithFile);
            fileStream.Close();
            ResetDiscoveries();
            SaveData();
        }
        */
        
        
        if (Directory.Exists(PathToFile_Persistent) && File.Exists(PathToFile_Persistent + FILE_NAME))
        {
            return;
        }
        
        if (!Directory.Exists(PathToFile_StreamingAssets) || !File.Exists(PathToFile_StreamingAssets + FILE_NAME))
        {
            ResetDiscoveries();
            Directory.CreateDirectory(PathToFile_StreamingAssets);
            SaveData(false);
        }
        
        Directory.CreateDirectory(PathToFile_Persistent);
        File.Copy(PathToFile_StreamingAssets + FILE_NAME, PathToFile_Persistent + FILE_NAME);
        SaveData(true);
    }


    [Button()]
    public void DoReset()
    {
        CheckFile();
        ResetDiscoveries();
        //DiscoverFirsts();
        SaveData(false);
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

    private void DiscoverFirsts()
    {
        Discover(_projectiles[0]);
        Discover(_passiveAbilities[0]);
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
        AchievementDefinitions.DiscoverAllProjectilesAndAbilities.Check(this);
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


    public bool AllProjectilesDiscovered()
    {
        foreach (KeyValuePair<TurretPartProjectileDataModel,bool> discoveredProjectile in _discoveredProjectiles)
        {
            if (!discoveredProjectile.Value)
            {
                return false;
            }
        }
        return true;
    }
    public bool AllPassiveAbilitiesDiscovered()
    {
        foreach (KeyValuePair<ATurretPassiveAbilityDataModel,bool> discoveredPassiveAbility in _discoveredPassiveAbilities)
        {
            if (!discoveredPassiveAbility.Value)
            {
                return false;
            }
        }
        return true;
    }
}
