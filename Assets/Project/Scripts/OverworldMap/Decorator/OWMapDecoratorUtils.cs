using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using NodeEnums;
using UnityEngine;

[CreateAssetMenu(fileName = "NewOWMapDecoratorUtils", 
    menuName = SOAssetPaths.MAP_OVERWORLD + "OWMapDecoratorUtils")]
public class OWMapDecoratorUtils : ScriptableObject
{
    [System.Serializable]
    public class AppearChances
    {
        [SerializeField] private bool _sharedByAllProgressions = true;
        [AllowNesting] [SerializeField, Range(0, 100), ShowIf("_sharedByAllProgressions")] private int _chance = 100;
        [AllowNesting] [SerializeField, Range(0, 100), HideIf("_sharedByAllProgressions")] private int _earlyChance = 100;
        [AllowNesting] [SerializeField, Range(0, 100), HideIf("_sharedByAllProgressions")] private int _midChance = 100;
        [AllowNesting] [SerializeField, Range(0, 100), HideIf("_sharedByAllProgressions")] private int _lateChance = 100;
        
        public int GetAppearChance(NodeEnums.ProgressionState progressionState)
        {
            switch (progressionState)
            {
                case ProgressionState.EARLY:
                    return _sharedByAllProgressions ? _chance : _earlyChance;
                case ProgressionState.MID:
                    return _sharedByAllProgressions ? _chance : _midChance;
                case ProgressionState.LATE:
                    return _sharedByAllProgressions ? _chance : _lateChance;
            }

            Debug.LogError("Can't appear in this progression state");
            return 0;
        }
    }
    
    [System.Serializable]
    public class UpgradeTypeApparition
    {
        [SerializeField] private NodeEnums.UpgradeType _upgradeType;
        [SerializeField] private string _titleName;
        [SerializeField] private Texture _texture;
        [AllowNesting] [SerializeField] private AppearChances _appearChances;

        public NodeEnums.UpgradeType UpgradeType => _upgradeType;
        public string TitleName => _titleName;
        public Texture Texture => _texture;

        public int GetAppearChance(NodeEnums.ProgressionState progressionState)
        {
            return _appearChances.GetAppearChance(progressionState);
        }
    }


    [SerializeField] private UpgradeTypeApparition[] _availableUpgrades;
    public UpgradeTypeApparition[] AvailableUpgrades => _availableUpgrades;


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
    
    
    public UpgradeTypeApparition UpgradeTypeApparitionByType(NodeEnums.UpgradeType upgradeType)
    {
        foreach (UpgradeTypeApparition availableUpgrade in _availableUpgrades)
        {
            if (availableUpgrade.UpgradeType == upgradeType)
            {
                return availableUpgrade;
            }
        }

        return null;
    }
    
    
    public Texture GetEmptyNodeTexture(NodeEnums.EmptyType emptyType) 
    {
        return emptyNodeTextures[(int)emptyType];
    }
}
