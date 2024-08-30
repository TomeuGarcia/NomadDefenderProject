using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretPartBase", 
    menuName = SOAssetPaths.TURRET_PARTS_BASES + "TurretPartBase")]
public class SupportPartBase : ScriptableObject
{
    [Header("STATS")]
    [SerializeField] private CardStatConfig _radiusRangeStat;
    public CardStatConfig RadiusRangeStat => _radiusRangeStat;

    [Header("PREFAB")]
    [SerializeField] private BasePartPrimitive _basePartPrimitive;
    [SerializeField] private GameObject _previewMeshPrefab;
    public BasePartPrimitive BasePartPrimitive => _basePartPrimitive;
    public GameObject PreviewMeshPrefab => _previewMeshPrefab;

    [Header("VISUALS")]
    [SerializeField] public Sprite abilitySprite;
    [SerializeField] public Color32 spriteColor;

    [Header("ABILITY INFO")]
    [SerializeField] private CardAbilityDescriptionModel _defaultAbilityDescription;
    [SerializeField] private CardAbilityDescriptionModel _lvl1AbilityDescription;
    [SerializeField] private CardAbilityDescriptionModel _lvl2AbilityDescription;
    [SerializeField] private CardAbilityDescriptionModel _lvl3AbilityDescription;
    public string abilityName => _defaultAbilityDescription.AbilityName;
    
    public EditableCardAbilityDescription[] MakeAbilityDescriptions()
    {
        EditableCardAbilityDescription[] abilityDescriptions = new EditableCardAbilityDescription[4];
        abilityDescriptions[0] = _defaultAbilityDescription.MakeEditableDescription();
        abilityDescriptions[1] = _lvl1AbilityDescription.MakeEditableDescription();
        abilityDescriptions[2] = _lvl2AbilityDescription.MakeEditableDescription();
        abilityDescriptions[3] = _lvl3AbilityDescription.MakeEditableDescription();

        return abilityDescriptions;
    }
    
    // Operator Overloads
    public static bool operator ==(SupportPartBase obj1, SupportPartBase obj2)
    {
        if (!obj1 || !obj2) return false;
        return obj1.BasePartPrimitive.Prefab == obj2.BasePartPrimitive.Prefab;
    }

    public static bool operator !=(SupportPartBase obj1, SupportPartBase obj2)
    {
        return !(obj1 == obj2);
    }

}
