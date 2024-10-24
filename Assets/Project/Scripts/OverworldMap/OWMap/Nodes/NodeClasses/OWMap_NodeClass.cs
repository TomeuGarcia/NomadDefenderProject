using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OWMap_NodeClass
{
    public NodeEnums.NodeType nodeType;
    public Texture nodeSprite;
    protected int nextLevelNodes;
    protected NodeEnums.HealthState healthState; //Can create problems if its not saved as a reference

    public NodeEnums.ProgressionState progressionState;

    public OWMap_NodeClass(NodeEnums.NodeType _nodeType, int _nextLevelNodes, ref NodeEnums.HealthState _healthState, NodeEnums.ProgressionState _progressionState) 
    {
        nextLevelNodes = _nextLevelNodes;
        healthState = _healthState;
        nodeType = _nodeType;

        progressionState = _progressionState;
    }

    public abstract void StartLevel(OverworldMapGameManager overwolrdMapGameManager);
    public void SetIconTexture(Texture mapIconTexture)
    {
        nodeSprite= mapIconTexture;
    }
}
