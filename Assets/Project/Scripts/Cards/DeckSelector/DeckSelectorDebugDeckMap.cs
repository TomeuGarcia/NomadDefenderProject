using System;
using System.Collections;
using System.Collections.Generic;
using NodeEnums;
using UnityEngine;

[CreateAssetMenu(fileName = "DeckSelectorDebugDeckMap", 
    menuName = SOAssetPaths.CARDS_DECKS + "DeckSelectorDebugDeckMap")]
public class DeckSelectorDebugDeckMap : ScriptableObject
{
    [System.Serializable]
    private class DeckMap
    {
        [SerializeField] private CardDeckAsset _deckToRemap;
        [SerializeField] private List<DeckByProgression> _deckByProgressions;

        public CardDeckAsset DeckToRemap => _deckToRemap;

        public void Validate()
        {
            int currentCount = _deckByProgressions.Count;
            Debug.Log(currentCount);
            int progressionStateCount = Enum.GetValues(typeof(ProgressionState)).Length;
            for (int i = currentCount; i < progressionStateCount; ++i)
            {
                _deckByProgressions.Add(new DeckByProgression((ProgressionState)i));
            }

            if (currentCount > progressionStateCount)
            {
                _deckByProgressions.RemoveRange(progressionStateCount, currentCount - progressionStateCount);
            }
        }

        public CardDeckAsset GetDeck(ProgressionState progressionState)
        {
            foreach (DeckByProgression deckByProgression in _deckByProgressions)
            {
                if (deckByProgression.ProgressionState == progressionState)
                {
                    return deckByProgression.Deck;
                }
            }

            return null;
        }
    }
    
    [System.Serializable]
    private class DeckByProgression
    {
        [SerializeField] private ProgressionState _progressionState;
        [SerializeField] private CardDeckAsset _deck;
        
        public ProgressionState ProgressionState => _progressionState;
        public CardDeckAsset Deck => _deck;

        public DeckByProgression(ProgressionState progressionState)
        {
            _progressionState = progressionState;
        }
    }
    
    
    
    [Header("OPTIONS")]
    [SerializeField] private bool _enabled;
    [SerializeField] private ProgressionState _progressionState = ProgressionState.EARLY;

    [Header("DECKS MAP")] 
    [SerializeField] private DeckMap[] _deckMaps;


    private void OnValidate()
    {
        foreach (DeckMap deckMap in _deckMaps)
        {
            deckMap.Validate();
        }
    }

    public CardDeckAsset RemapDeck(CardDeckAsset deck)
    {
        if (!_enabled)
        {
            return deck;
        }

        foreach (DeckMap deckMap in _deckMaps)
        {
            if (deckMap.DeckToRemap == deck)
            {
                return deckMap.GetDeck(_progressionState);
            }
        }

        return null;
    }
    
}
