using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OWMap_NodeClass
{
    public NodeEnums.NodeType nodeType;
    public Sprite nodeSprite;

    public abstract void StartLevel();
}
