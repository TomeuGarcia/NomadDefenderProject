using UnityEngine;

#if UNITY_EDITOR
using System;
using UnityEditor;


[CustomEditor(typeof(TurretCardDataModel))]
[CanEditMultipleObjects]
public class EditorTurretCardParts : Editor
{
    private const float width = 96f;
    private const float height = 96f;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TurretCardDataModel cardDataModel = target as TurretCardDataModel;
        TurretCardPartsGroup parts = cardDataModel.SharedPartsGroup;

        
        EditorGUILayout.Space(20);
        DrawAssetName(cardDataModel);
        DrawAttackPreview(parts.Projectile);
        DrawBodyPreview(parts.Body);
        DrawPassiveBasePreview(parts.Passive);     
    }


    private void DrawAssetName(TurretCardDataModel cardDataModel)
    {
        string assetName = "";
        EditorGUILayout.BeginHorizontal();
        {
            assetName = GenerateAssetName(cardDataModel);
            
            EditorGUILayout.LabelField("Asset Name should be:", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
            EditorGUILayout.SelectableLabel(assetName, EditorStyles.textField, 
                GUILayout.Height(EditorGUIUtility.singleLineHeight));

            if (GUILayout.Button("Copy"))
            {
                GUIUtility.systemCopyBuffer = assetName;
            }
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        {
            bool assetHasCorrectName = cardDataModel.name == assetName;
            var style = new GUIStyle();
            style.normal.textColor = assetHasCorrectName ? Color.green : Color.yellow;
            string feedbackText = assetHasCorrectName ? "Asset Name is correct" : "Asset Name doesn't match";
            EditorGUILayout.LabelField(feedbackText,  style);
        }
        EditorGUILayout.EndHorizontal();
    }
    
    private string GenerateAssetName(TurretCardDataModel cardDataModel)
    {
        if (!cardDataModel.SharedPartsGroup.Body ||
            !cardDataModel.SharedPartsGroup.Projectile)
        {
            return "MISSING PARTS";
        }
        
        string name = "";
        
        name += AbilityNameWithCapital(cardDataModel.SharedPartsGroup.Body.partName);
        name += "_" + cardDataModel.CardPlayCost;
        name += "_L" + cardDataModel.CardLevel;
        name += "_" + AbilityNameWithCapital(cardDataModel.SharedPartsGroup.Projectile.abilityName);

        foreach (ATurretPassiveAbilityDataModel passiveAbilityModel in cardDataModel.PassiveAbilityModels)
        {
            if (!passiveAbilityModel)
            {
                name += "_MISSING";
                continue;
            }
            name += "_" + AbilityNameWithCapital(passiveAbilityModel.Name);
        }

        return name;
    }

    private string AbilityNameWithCapital(string abilityName)
    {
        char capital = Char.ToUpper(abilityName[0]);
        if (capital == abilityName[0])
        {
            return abilityName;
        }
        
        return capital + abilityName.Substring(1, abilityName.Length-1);
    }
    
    private void DrawAttackPreview(TurretPartProjectileDataModel turretPartAttack)
    {
        GUILayout.Label("\nAttack Preview:");
        if (turretPartAttack != null)
        {
            GUILayout.Label(" - " + turretPartAttack.abilityName);            
            GUILayout.Box(turretPartAttack.texture,
                          GUILayout.MinHeight(height), GUILayout.MinWidth(width), GUILayout.MaxHeight(height), GUILayout.MaxWidth(width));
        }
        else
        {
            GUILayout.Label("MISSING ATTACK PART", "red");
        }
    }

    private void DrawBodyPreview(TurretPartBody turretPartBody)
    {
        GUILayout.Label("\nBody Preview:");
        if (turretPartBody != null)
        {
            GUILayout.Label(" - " + turretPartBody.partName +
                (" (D" + turretPartBody.DamageStat.ComputeValueByLevel(0)) + 
                (", C" + turretPartBody.ShotsPerSecondStat.ComputeValueByLevel(0)) + 
                (", R" + turretPartBody.RadiusRangeStat.ComputeValueByLevel(0)) +
                ")");
            GUILayout.Box(turretPartBody.materialTextureMap,
                          GUILayout.MinHeight(height), GUILayout.MinWidth(width), GUILayout.MaxHeight(height), GUILayout.MaxWidth(width));
            
            GUILayout.Box(turretPartBody.BasePartPrimitive.MaterialTexture,
                GUILayout.MinHeight(height), GUILayout.MinWidth(width), GUILayout.MaxHeight(height), GUILayout.MaxWidth(width));
        }
        else
        {
            GUILayout.Label("MISSING BODY PART", "red");
        }
    }
    
    private void DrawPassiveBasePreview(TurretPassiveBase turretPassiveBase)
    {
        GUILayout.Label("\nPassive Base Preview:");
        if (turretPassiveBase != null)
        {
            GUILayout.Label(" - " + turretPassiveBase.passive.abilityName);
            GUILayout.Box(turretPassiveBase.visualInformation.spriteAsTexture,
                          GUILayout.MinHeight(height), GUILayout.MinWidth(width), GUILayout.MaxHeight(height), GUILayout.MaxWidth(width));
        }
        else
        {
            GUILayout.Label("MISSING PASSIVE BASE PART", "red");
        }
    }

}

#endif
