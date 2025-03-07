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
    
    private string PathToFile => Application.streamingAssetsPath + "/JSONfiles/Cards/";
    private string FileName => "GameProgression.json";
    
    
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
        SaveData();
    }


    public void ResetEverything()
    {
        ResetStatus();
    }

    public void IncrementVictoryCount(CardDeckAsset starterDeck)
    {
        Game.IncrementVictoriesCount(starterDeck, _gameDifficultyConfig.CurrentGameDifficulty);
    }
    


    [Button()]
    private void LoadData()
    {
        CheckFile();

        string storedContent = _caesarCipher.Decipher(File.ReadAllText(PathToFile + FileName));

        GameStatusDataWrapper gameWrapper = JsonUtility.FromJson<GameStatusDataWrapper>(storedContent);
        Game = gameWrapper.MakeGameStatus(_decksLibrary.GetAllPossibleStarterDecks());
    }
    
    
    [Button()]
    private void SaveData()
    {
        GameStatusDataWrapper dataToStore = new GameStatusDataWrapper(Game);
        
        string contentToStore = _caesarCipher.Cipher(JsonUtility.ToJson(dataToStore));

        File.WriteAllText(PathToFile + FileName, contentToStore);
    }


    private void CheckFile()
    {
        string directory = PathToFile;
        string directoryWithFile = directory + FileName;
        if (!Directory.Exists(directory) || !File.Exists(directoryWithFile))
        {
            Directory.CreateDirectory(directory);
            FileStream fileStream = File.Create(directoryWithFile);
            fileStream.Close();
            ResetStatus();
            SaveData();
        }
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