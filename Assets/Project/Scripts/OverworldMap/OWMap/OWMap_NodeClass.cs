using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OWMap_NodeClass
{
    public NodeEnums.NodeType nodeType;
    public Sprite nodeSprite;
    protected int nextLevelNodes;
    protected NodeEnums.HealthState healthState; //Can create problems if its not saved as a reference

    public OWMap_NodeClass(int _nextLevelNodes, ref NodeEnums.HealthState _healthState) 
    {
        nextLevelNodes = _nextLevelNodes;
        healthState = _healthState;
    }
    public abstract void StartLevel(OverworldMapGameManager overwolrdMapGameManager);
}
