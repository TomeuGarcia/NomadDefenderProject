using UnityEngine;


[System.Serializable]
public class GameDifficultySettings
{
    [SerializeField, Min(0f)] private float _enemyHealthMultiplier = 1f;
    [SerializeField, Min(0f)] private float _extraDelayWaveStart = 1f;
    [SerializeField] private float _extraTimeCurrencyDrop = 0f;
    public float EnemyHealthMultiplier => _enemyHealthMultiplier;
    public float ExtraDelayWaveStart => _extraDelayWaveStart;
    public float ExtraTimeCurrencyDrop => _extraTimeCurrencyDrop;
}