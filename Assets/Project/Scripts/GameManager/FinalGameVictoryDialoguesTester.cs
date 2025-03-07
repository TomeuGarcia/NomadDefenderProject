using NaughtyAttributes;
using UnityEngine;

public class FinalGameVictoryDialoguesTester : MonoBehaviour
{
    [Header("CONFIG")] 
    [SerializeField] private GameDifficultyType _testGameDifficulty;
    [SerializeField, Min(0)] private int _testVictoryCountHardDifficulty;
    [SerializeField] private bool _testFullPerfectDefense;
    [SerializeField] private bool _everBeatFullPerfectDefenseBefore;
    
    [Header("COMPONENTS")]
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private FinalGameVictoryDialogues _finalGameVictoryDialogues;




    [Button()]
    private void Test()
    {
        _finalGameVictoryDialogues.Setup(_testGameDifficulty, _testVictoryCountHardDifficulty, 
            _testFullPerfectDefense, _everBeatFullPerfectDefenseBefore);
        StartCoroutine(_gameManager.DoStartVictory(_finalGameVictoryDialogues, false));
    }
}