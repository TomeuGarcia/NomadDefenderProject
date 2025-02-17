using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "CoreConfigurations", 
    menuName = SOAssetPaths.HELPERS + "CoreConfigurations")]
public class CoreConfigurations : ScriptableObject
{
    [Header("DECKS")] 
    [SerializeField, Expandable] private DeckSelectorDebugDeckMap _debugDecks;
    
    [Space(20)]
    [Header("OW MAP")] 
    [SerializeField, Expandable] private OWMapDecoratorSettings _mapDecorationSettings;
    [SerializeField, Expandable] private OWMapDecoratorUtils _mapDecorationUtils;
    [SerializeField, Expandable] private OWMapGenerationSettings _mapGenerationSettings;
    
    
    [Space(20)]
    [Header("DEMO")] 
    [SerializeField, Expandable] private MapScenesLibrary _mapScenesLibrary;
    
    
    [Space(20)]
    [Header("DEMO")] 
    [SerializeField, Expandable] private DemoManagerConfig _demoManagerConfig;
    
    
    [Space(20)]
    [Header("DIFFICULTY")] 
    [SerializeField, Expandable] private GameDifficultyConfig _gameDifficultyConfig;
}
