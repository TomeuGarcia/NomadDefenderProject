using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;


[CustomEditor(typeof(SupportCardDataModel))]
[CanEditMultipleObjects]
public class EditorSupportCardParts : Editor
{
    private const float width = 96f;
    private const float height = 96f;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SupportCardDataModel cardDataModel = target as SupportCardDataModel;
        SupportCardPartsGroup parts = cardDataModel.SharedPartsGroup;

        DrawBasePreview(parts.Base);
    }


    private void DrawBasePreview(SupportPartBase turretPartBase)
    {
        GUILayout.Label("\nBase Preview:");
        if (turretPartBase != null)
        {
            GUILayout.Label(" - " + turretPartBase.abilityName + (" (R" + turretPartBase.RadiusRangeStat.ComputeValueByLevel(0)) + ")");
            GUILayout.Box(turretPartBase.BasePartPrimitive.MaterialTexture,
                          GUILayout.MinHeight(height), GUILayout.MinWidth(width), GUILayout.MaxHeight(height), GUILayout.MaxWidth(width));
        }
        else
        {
            GUILayout.Label("MISSING BASE PART", "red");
        }
    }


}

#endif