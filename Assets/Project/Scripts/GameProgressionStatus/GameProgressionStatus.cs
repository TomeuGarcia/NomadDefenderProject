using System.IO;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "GameProgressionStatus", 
    menuName = SOAssetPaths.GAME_PROGRESSION + "GameProgressionStatus")]
public class GameProgressionStatus : ScriptableObject, IGameProgressionStatus, IGameProgressionUpdater
{
    [SerializeField, Expandable] private DecksLibrary _decksLibrary;
    [SerializeField, Expandable] private GameDifficultyConfig _gameDifficultyConfig;

    [Header("DEBUGGING")] 
    [SerializeField] private CardDeckAsset _debugAddDeckVictory;
    
    
    private string PathToFile_StreamingAssets => Application.streamingAssetsPath + "/JSONfiles/Cards/";
    private string PathToFile_Persistent => Application.persistentDataPath + "/Data/";
    private const string FILE_NAME = "GameProgression.json";
    
    
    
    private readonly CaesarCipher _caesarCipher = new (735);

    public IGameProgressionStatus.GameStatus Game { get; private set; }


    [System.Serializable]
    private class GameStatusDataWrapper
    {
        [SerializeField] public int victoriesCount;
        [SerializeField] public int victoriesCountHardDifficulty;
        [SerializeField] public bool beatARunWithFullPerfectDefense;
        [SerializeField] public StarterDecksSaveStatus starterDecksSaveStatus;
        
        public GameStatusDataWrapper(IGameProgressionStatus.GameStatus gameStatus)
        {
            victoriesCount = gameStatus.VictoriesCount;
            victoriesCountHardDifficulty = gameStatus.VictoriesCountHardDifficulty;
            beatARunWithFullPerfectDefense = gameStatus.BeatARunWithFullPerfectDefense;
            starterDecksSaveStatus = gameStatus.StarterDecksSaveStatus;
        }

        public IGameProgressionStatus.GameStatus MakeGameStatus(CardDeckAsset[] possibleStarterDecks)
        {
            IGameProgressionStatus.GameStatus gameStatus =
                new IGameProgressionStatus.GameStatus(victoriesCount, victoriesCountHardDifficulty,
                    beatARunWithFullPerfectDefense, starterDecksSaveStatus);
            gameStatus.ValidateCorrectLoading(possibleStarterDecks);
            return gameStatus;
        }
        
        
    }


    private void OnEnable()
    {
        LoadData();
    }

    private void OnDisable()
    {
        if(Game == null)
        {
            ResetStatus();

        }
        SaveData(true);
    }


    public void ResetEverything()
    {
        ResetStatus();
        SaveData(true);
        SaveData(false);
    }

    public void IncrementVictoryCount(CardDeckAsset starterDeck)
    {
        Game.IncrementVictoriesCount(starterDeck, _gameDifficultyConfig.CurrentGameDifficulty);
    }
    


    [Button()]
    private void LoadData()
    {
        CheckFile();

        string storedContent = _caesarCipher.Decipher(File.ReadAllText(PathToFile_Persistent + FILE_NAME));

        GameStatusDataWrapper gameWrapper = JsonUtility.FromJson<GameStatusDataWrapper>(storedContent);
        Game = gameWrapper.MakeGameStatus(_decksLibrary.GetAllPossibleStarterDecks());
    }
    
    
    [Button()]
    private void DebugSaveData()
    {
        SaveData(true);
    }
    public void SaveData(bool toPersistent)
    {
        GameStatusDataWrapper dataToStore = new GameStatusDataWrapper(Game);
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
            ResetStatus();
            SaveData();
        }
        */
        
        if (Directory.Exists(PathToFile_Persistent) && File.Exists(PathToFile_Persistent + FILE_NAME))
        {
            Debug.Log("Already exists " + PathToFile_Persistent + FILE_NAME);
            return;
        }
        
        ResetStatus();
        if (!Directory.Exists(PathToFile_StreamingAssets) || !File.Exists(PathToFile_StreamingAssets + FILE_NAME))
        {
            Debug.Log("Created Streaming");
            Directory.CreateDirectory(PathToFile_StreamingAssets);
            SaveData(false);
        }
        
        Debug.Log("Created Persistent " + PathToFile_Persistent);
        Debug.Log("Copy from " + PathToFile_StreamingAssets + FILE_NAME);
        Directory.CreateDirectory(PathToFile_Persistent);
        File.Copy(PathToFile_StreamingAssets + FILE_NAME, PathToFile_Persistent + FILE_NAME);
        SaveData(true);
    }


    private void ResetStatus()
    {
        Game = new IGameProgressionStatus.GameStatus(_decksLibrary.GetAllPossibleStarterDecks());
    }


    [Button()]
    private void Debug_AddDeckVictory()
    {
        Game.StarterDecksSaveStatus.IncrementDeckVictory(_debugAddDeckVictory, _gameDifficultyConfig.CurrentGameDifficulty);
    }
    
    [Button()]
    private void Debug_ResetAllDeckVictories()
    {
        Game.StarterDecksSaveStatus.ResetAll();
    }
}