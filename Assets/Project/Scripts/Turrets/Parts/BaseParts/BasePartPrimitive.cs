using UnityEngine;

[System.Serializable]
public class BasePartPrimitive
{
    [Header("PREFAB")]
    [SerializeField] private GameObject _prefab;

    [Header("VISUALS")]
    [SerializeField] private Texture2D _materialTexture;
    [SerializeField] private Color _materialColor;
    
    
    public GameObject Prefab => _prefab;
    public Texture2D MaterialTexture => _materialTexture;
    public Color MaterialColor => _materialColor;
}