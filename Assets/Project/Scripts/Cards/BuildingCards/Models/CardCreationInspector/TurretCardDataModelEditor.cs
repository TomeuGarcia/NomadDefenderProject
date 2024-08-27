using UnityEngine;

#if UNITY_EDITOR
using System;
using UnityEditor;


[CustomEditor(typeof(TurretCardDataModel))]
[CanEditMultipleObjects]
public class TurretCardDataModelEditor : Editor
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
        EditorGUILayout.Space(20);
        DrawBodyAndProjectilePreview(parts.Body, parts.Projectile);
        DrawPassiveBasePreview(cardDataModel.PassiveAbilityModels);     
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
        name += "_" + AbilityNameWithCapital(cardDataModel.SharedPartsGroup.Projectile.AbilityName);

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
    
    private void DrawBodyAndProjectilePreview(TurretPartBody turretPartBody, TurretPartProjectileDataModel turretPartProjectile)
    {
        EditorGUILayout.BeginHorizontal();

        
        EditorGUILayout.BeginVertical();
        if (turretPartBody != null)
        {
            GUILayout.Label("BODY: " + AbilityNameWithCapital(turretPartBody.partName));            
            GUILayout.Box(turretPartBody.materialTextureMap,
                          GUILayout.MinHeight(height), GUILayout.MinWidth(width), GUILayout.MaxHeight(height), GUILayout.MaxWidth(width));
        }
        else
        {
            GUILayout.Label("Missing BODY part", "red");
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.BeginVertical();
        if (turretPartProjectile != null)
        {
            GUILayout.Label("PROJECTILE: " + AbilityNameWithCapital(turretPartProjectile.AbilityName));            
            GUILayout.Box(turretPartProjectile.texture,
                          GUILayout.MinHeight(height), GUILayout.MinWidth(width), GUILayout.MaxHeight(height), GUILayout.MaxWidth(width));
            EditorGUILayout.ColorField(turretPartProjectile.materialColor, 
                GUILayout.MinHeight(16), GUILayout.MinWidth(width), GUILayout.MaxHeight(16), GUILayout.MaxWidth(width));
        }
        else
        {
            GUILayout.Label("Missing PROJECTILE part", "red");
        }
        EditorGUILayout.EndVertical();
        
        
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawPassiveBasePreview(ATurretPassiveAbilityDataModel[] passiveAbilities)
    {
        GUILayout.Label("\nPASSIVES:");

        if (passiveAbilities.Length == 0)
        {
            GUILayout.Label("(no passives)");
            return;
        }
        
        
        EditorGUILayout.BeginHorizontal();
        
        foreach (ATurretPassiveAbilityDataModel passiveAbilityDataModel in passiveAbilities)
        {
            if (passiveAbilityDataModel != null)
            {
                EditorGUILayout.BeginVertical();
                GUILayout.Label(AbilityNameWithCapital(passiveAbilityDataModel.Name));
                GUILayout.Box(passiveAbilityDataModel.View.SpriteAsTexture, 
                    GUILayout.MinHeight(height), GUILayout.MinWidth(width), GUILayout.MaxHeight(height), GUILayout.MaxWidth(width));
                EditorGUILayout.ColorField(passiveAbilityDataModel.View.Color, 
                    GUILayout.MinHeight(16), GUILayout.MinWidth(width), GUILayout.MaxHeight(16), GUILayout.MaxWidth(width));
                EditorGUILayout.EndVertical();
            }
            else
            {
                GUILayout.Label("MISSING PASSIVE BASE PART", "red");
            }
        }
        
        EditorGUILayout.EndHorizontal();
        
        

    }

}

#endif
