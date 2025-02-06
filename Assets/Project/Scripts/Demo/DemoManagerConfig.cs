using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DemoManagerConfig", 
    menuName = SOAssetPaths.DEMO + "DemoManagerConfig")]
public class DemoManagerConfig : ScriptableObject
{
    [Header("ENABLED")]
    [SerializeField] private bool _demoEnabled;

    [Header("CONFIG")] 
    [SerializeField, Min(0)] private int _nodeIndex = 8; 
    [SerializeField] private DemoWall _demoWallPrefab;

    
    public bool DemoEnabled => _demoEnabled;
    public int NodeIndex => _nodeIndex;
    public DemoWall DemoWallPrefab => _demoWallPrefab;
}
