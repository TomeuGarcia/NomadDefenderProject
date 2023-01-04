using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Decorator Utils")]
public class decoratorUtils : ScriptableObject
{

    public List<Texture> upgradeNodeTextures;
    public List<Texture> battleNodeTextures;
    public List<Texture> emptyNodeTextures;


    public Texture GetBattleNodeTexture(NodeEnums.BattleType battleType) 
    {
        return battleNodeTextures[(int)battleType];
    }
    
    public Texture GetUpgradeNodeTexture(NodeEnums.UpgradeType upgradeType) 
    {
        return upgradeNodeTextures[(int)upgradeType];
    }
    
    public Texture GetEmptyNodeTexture(NodeEnums.EmptyType emptyType) 
    {
        return emptyNodeTextures[(int)emptyType];
    }
}
