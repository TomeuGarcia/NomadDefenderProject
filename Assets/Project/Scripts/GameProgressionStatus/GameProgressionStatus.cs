using System.IO;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "GameProgressionStatus", 
    menuName = SOAssetPaths.GAME_PROGRESSION + "GameProgressionStatus")]
public class GameProgressionStatus : ScriptableObject, IGameProgressionStatus, IGameProgressionUpdater
{
    private string PathToFile => Application.streamingAssetsPath + "/JSONfiles/Cards/";
    private string FileName => "GameProgression.json";
    
    
    private readonly CaesarCipher _caesarCipher = new (735);

    public IGameProgressionStatus.GameStatus Game { get; private set; }


    [System.Serializable]
    private class GameStatusDataWrapper
    {
        [SerializeField] public int victoriesCount;
        
        public GameStatusDataWrapper(IGameProgressionStatus.GameStatus gameStatus)
        {
            victoriesCount = gameStatus.VictoriesCount;
        }

        public IGameProgressionStatus.GameStatus MakeGameStatus()
        {
            return new IGameProgressionStatus.GameStatus(victoriesCount);
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

    public void IncrementVictoryCount()
    {
        Game = new IGameProgressionStatus.GameStatus(Game.VictoriesCount + 1);
    }
    


    [Button()]
    private void LoadData()
    {
        CheckFile();

        string storedContent = _caesarCipher.Decipher(File.ReadAllText(PathToFile + FileName));
        Game = JsonUtility.FromJson<GameStatusDataWrapper>(storedContent).MakeGameStatus();
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
        if (!Directory.Exists(directory) || !File.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            FileStream fileStream = File.Create(directory + FileName);
            fileStream.Close();
            ResetStatus();
            SaveData();
        }
    }


    private void ResetStatus()
    {
        Game = new IGameProgressionStatus.GameStatus(0);
    }
}