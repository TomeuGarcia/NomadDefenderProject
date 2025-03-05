using NaughtyAttributes;
using UnityEngine;

public class TileChangingBossEvent : MonoBehaviour
{
    [SerializeField] private bool _skipChangeAnimation = false;
    [SerializeField, Min(0)] private int _waveIndex = 0;
    [Required(), SerializeField] private LevelTileChange _levelTileChange;
    [Required(),SerializeField] private TileChangingBossDialogue _dialogue;
    public TileChangingBossDialogue Dialogue => _dialogue;
    
    public bool SkipChangeAnimation => _skipChangeAnimation;
    public int WaveIndex => _waveIndex;


    public void Init(ConsoleDialogSystem dialogueSystem)
    {
        _levelTileChange.Init(_waveIndex == 0);
        _dialogue.Init(dialogueSystem);
    }

    public void Show()
    {
        _levelTileChange.Show();
    }
    
    public void Hide()
    {
        _levelTileChange.Hide();
    }
}