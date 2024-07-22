using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;


[CustomEditor(typeof(TurretCardParts))]
[CanEditMultipleObjects]
public class EditorTurretCardParts : Editor
{
    private const float width = 96f;
    private const float height = 96f;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TurretCardParts turretCardParts = target as TurretCardParts;

        DrawAttackPreview(turretCardParts.turretPartAttack);
        DrawBodyPreview(turretCardParts.turretPartBody);
        DrawBasePreview(turretCardParts.turretPartBase);
        DrawPassiveBasePreview(turretCardParts.turretPassiveBase);     
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
            GUILayout.Label(" - " + turretPartBody.partName + (" (D" + turretPartBody.BaseDamageText) + (", C" + turretPartBody.BaseShotsPerSecondText) + ")");
            GUILayout.Box(turretPartBody.materialTextureMap,
                          GUILayout.MinHeight(height), GUILayout.MinWidth(width), GUILayout.MaxHeight(height), GUILayout.MaxWidth(width));
        }
        else
        {
            GUILayout.Label("MISSING BODY PART", "red");
        }
    }

    private void DrawBasePreview(TurretPartBase turretPartBase)
    {
        GUILayout.Label("\nBase Preview:");
        if (turretPartBase != null)
        {
            GUILayout.Label(" - " + turretPartBase.abilityName + (" (R" + turretPartBase.BaseRangeText) + ")");
            GUILayout.Box(turretPartBase.materialTexture,
                          GUILayout.MinHeight(height), GUILayout.MinWidth(width), GUILayout.MaxHeight(height), GUILayout.MaxWidth(width));
        }
        else
        {
            GUILayout.Label("MISSING BASE PART", "red");
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
