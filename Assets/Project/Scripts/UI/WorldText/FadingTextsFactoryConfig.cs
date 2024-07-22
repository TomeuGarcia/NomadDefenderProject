using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.ObjectPooling;
using NaughtyAttributes;


[CreateAssetMenu(fileName = "FadingTextFactoryConfig_NAME", 
    menuName = SOAssetPaths.UI_FADING_TEXTS + "FadingTextFactoryConfig")]
public class FadingTextsFactoryConfig : ScriptableObject
{
    [Header("POOLS")]
    [SerializeField] private ObjectPoolInitData<FadingText> _fadingTextPoolData;
    [SerializeField] private ObjectPoolInitData<FadingTextCharacter> _fadingTextCharacterPoolData;

    [Header("TEXTS CONFIGS")]
    [Expandable] [SerializeField] private FadingTextConfig _fadingTextConfig;


    public ObjectPoolInitData<FadingText> FadingTextPoolData => _fadingTextPoolData;
    public ObjectPoolInitData<FadingTextCharacter> FadingTextCharacterPoolData => _fadingTextCharacterPoolData;
    public FadingTextConfig FadingTextConfig => _fadingTextConfig;
}
