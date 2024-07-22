using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewOWMapDecoratorUtils", menuName = "Map/OWMapDecoratorUtils")]
public class OWMapDecoratorUtils : ScriptableObject
{

    public List<Texture> upgradeNodeTextures;
    public List<Texture> battleNodeTextures;
    public List<Texture> emptyNodeTextures;

    [SerializeField, ColorUsage(true, true)] private Color darkGreyColor = new Color(106f / 255f, 106f / 255f, 106f / 255f);
    [SerializeField, ColorUsage(true, true)] private Color lightGreyColor = new Color(.9f, .9f, .9f);
    [SerializeField, ColorUsage(true, true)] private Color blueColor = new Color(38f / 255f, 142f / 255f, 138f / 255f);
    [SerializeField, ColorUsage(true, true)] private Color blueColor2 = new Color(38f / 255f, 142f / 255f, 138f / 255f);

    [SerializeField, ColorUsage(true, true)] private Color yellowColor = new Color(190f / 255f, 190f / 255f, 50f / 255f);
    [SerializeField, ColorUsage(true, true)] private Color orangeColor = new Color(190f / 255f, 80f / 255f, 0f / 255f);
    [SerializeField, ColorUsage(true, true)] private Color redColor = new Color(140f / 255f, 7f / 255f, 36f / 255f);
    [SerializeField, ColorUsage(true, true)] private Color redColor2 = new Color(140f / 255f, 7f / 255f, 36f / 255f);



    public static Color s_darkGreyColor;
    public static Color s_lightGreyColor;
    public static Color s_blueColor;
    public static Color s_blueColor2;

    public static Color s_yellowColor;
    public static Color s_orangeColor;
    public static Color s_redColor;
    public static Color s_redColor2;


    private void Awake()
    {
        SetupStaticColors();
    }
    private void OnValidate()
    {
        SetupStaticColors();
    }

    private void SetupStaticColors()
    {
        s_darkGreyColor = darkGreyColor;
        s_lightGreyColor = lightGreyColor;
        s_blueColor = blueColor;
        s_blueColor2 = blueColor2;
        s_yellowColor = yellowColor;
        s_orangeColor = orangeColor;
        s_redColor = redColor;
        s_redColor2 = redColor2;
    }

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
