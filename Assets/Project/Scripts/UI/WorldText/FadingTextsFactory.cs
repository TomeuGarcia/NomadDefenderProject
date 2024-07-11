using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.ObjectPooling;

public class FadingTextsFactory : MonoBehaviour, IFadingTextsFactory, IFadingTextCharactersFactory
{
    [Header("CONFIGURATION")]
    [SerializeField] private FadingTextsFactoryConfig _config;

    [Header("COMPONENTS")]
    [SerializeField] private Transform _textsParent;
    [SerializeField] private RectTransform _textsWaitParent;

    private ObjectPool _fadingTextsPool;
    private ObjectPool _fadingTextCharactersPool;
    private CircularBuffer<IFadingTextsFactory.TextSpawnData> _textSpawnDataBuffer;


    private void Awake()
    {
        _fadingTextsPool = _config.FadingTextPoolData.MakeInitializedObjectPool(_textsParent);
        _fadingTextCharactersPool = _config.FadingTextCharacterPoolData.MakeInitializedObjectPool(_textsWaitParent);

        _textSpawnDataBuffer = new CircularBuffer<IFadingTextsFactory.TextSpawnData>(new IFadingTextsFactory.TextSpawnData[]
        {
            new IFadingTextsFactory.TextSpawnData(_config.FadingTextConfig.DefaultImageBackgroundColor),
            new IFadingTextsFactory.TextSpawnData(_config.FadingTextConfig.DefaultImageBackgroundColor),
            new IFadingTextsFactory.TextSpawnData(_config.FadingTextConfig.DefaultImageBackgroundColor)
        });
    }



    public IFadingTextsFactory.TextSpawnData GetTextSpawnData()
    {
        return _textSpawnDataBuffer.GetNext();
    }

    public void SpawnFadingText(IFadingTextsFactory.TextSpawnData textSpawnData)
    {
        FadingText fadingText = _fadingTextsPool.Spawn<FadingText>(Vector3.zero, Quaternion.identity);
        fadingText.Init(textSpawnData, this, _config.FadingTextConfig);
    }

    public FadingTextCharacter SpawnFadingTextCharacter(Transform fadingTextParent, char character)
    {
        FadingTextCharacter fadingTextCharacter = _fadingTextCharactersPool.Spawn<FadingTextCharacter>(Vector3.zero, Quaternion.identity);
        fadingTextCharacter.Init(fadingTextParent, character);
        return fadingTextCharacter;
    }
}