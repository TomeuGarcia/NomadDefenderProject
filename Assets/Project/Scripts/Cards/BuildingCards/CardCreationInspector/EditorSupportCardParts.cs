using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;


[CustomEditor(typeof(SupportCardParts))]
[CanEditMultipleObjects]
public class EditorSupportCardParts : Editor
{
    private const float width = 128f;
    private const float height = 128f;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SupportCardParts supportCardParts = target as SupportCardParts;

        DrawBasePreview(supportCardParts.turretPartBase);
    }


    private void DrawBasePreview(TurretPartBase turretPartBase)
    {
        GUILayout.Label("\nBase Preview:");
        if (turretPartBase != null)
        {
            GUILayout.Label(" - " + turretPartBase.abilityName + (" (R" + turretPartBase.rangeLvl) + ")");
            GUILayout.Box(turretPartBase.materialTexture,
                          GUILayout.MinHeight(height), GUILayout.MinWidth(width), GUILayout.MaxHeight(height), GUILayout.MaxWidth(width));
        }
        else
        {
            GUILayout.Label("MISSING BASE PART", "red");
        }
    }


}

#endif