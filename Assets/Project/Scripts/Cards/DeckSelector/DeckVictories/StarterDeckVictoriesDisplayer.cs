using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StarterDeckVictoriesDisplayer : MonoBehaviour
{
    [System.Serializable]
    private class WinsCountView
    {
        [SerializeField] private GameObject _holder;
        [SerializeField] private TextMeshPro _winsCountText;

        public void Init(int winsCount)
        {
            if (winsCount < 1)
            {
                _holder.SetActive(false);
                return;
            }

            _winsCountText.text = winsCount.ToString();
        }
    }
    
    [System.Serializable]
    private class BeatenDifficultyView
    {
        [SerializeField] private Material _ledDisabledMaterial;
        [SerializeField] private MeshRenderer[] _ledMeshes;

        public void Init(int winsCount, GameDifficultyType highestDifficultyVictory)
        {
            bool hasEverWon = winsCount > 0;
            int highestDifficultyVictoryIndex = hasEverWon 
                ? (int)highestDifficultyVictory
                : -1;

            for (int i = _ledMeshes.Length - 1; i > highestDifficultyVictoryIndex; --i)
            {
                _ledMeshes[i].material = _ledDisabledMaterial;
            }
        }
    }


    [Header("WINS COUNT")]
    [SerializeField] private WinsCountView _winsCountView;
    [SerializeField] private BeatenDifficultyView _beatenDifficultyView;
    
    
    public void Init(int winsCount, GameDifficultyType highestDifficultyVictory)
    {
        _winsCountView.Init(winsCount);
        _beatenDifficultyView.Init(winsCount, highestDifficultyVictory);
    }
}
