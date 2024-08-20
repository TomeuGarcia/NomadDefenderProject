using UnityEngine;

#if UNITY_EDITOR
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

        DrawAttackPreview(parts.Projectile);
        DrawBodyPreview(parts.Body);
        DrawPassiveBasePreview(parts.Passive);     
    }


    private void DrawAttackPreview(TurretPartAttack turretPartAttack)
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
