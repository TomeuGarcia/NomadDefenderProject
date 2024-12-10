using UnityEditor;
using UnityEngine;

namespace Project.Scripts.Turrets.Parts.TurretStatsUpgrades.Editor
{
    [CustomEditor(typeof(TurretStatsUpgradeModel))]
    public class TurretStatsUpgradeModelEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TurretStatsUpgradeModel turretStatsUpgradeModel = target as TurretStatsUpgradeModel;

            EditorGUILayout.Space(20);
            DrawAssetName(turretStatsUpgradeModel); 
        }


        private void DrawAssetName(TurretStatsUpgradeModel turretStatsUpgradeModel)
        {
            string assetName = "";
            EditorGUILayout.BeginHorizontal();
            {
                assetName = GenerateAssetName(turretStatsUpgradeModel);
                
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
                bool assetHasCorrectName = turretStatsUpgradeModel.name == assetName;
                var style = new GUIStyle();
                style.normal.textColor = assetHasCorrectName ? Color.green : Color.yellow;
                string feedbackText = assetHasCorrectName ? "Asset Name is correct" : "Asset Name doesn't match";
                EditorGUILayout.LabelField(feedbackText,  style);
            }
            EditorGUILayout.EndHorizontal();
        }
        
        private string GenerateAssetName(TurretStatsUpgradeModel turretStatsUpgradeModel)
        {
            bool changesDamage = turretStatsUpgradeModel.DamageMultiplier != 0;
            bool changesShotsPerSecond = turretStatsUpgradeModel.ShotsPerSecondMultiplier != 0;
            bool changesRange = turretStatsUpgradeModel.RadiusRangeMultiplier != 0;
            bool changesLevel = turretStatsUpgradeModel.ExtraLevels != 1;
            bool changesPlayCost = turretStatsUpgradeModel.ExtraPlayCost != 0;
            
            
            if (!changesDamage && !changesShotsPerSecond && !changesRange && !changesLevel && !changesPlayCost)
            {
                return "NO CHANGES";
            }

            string name = "TurretStatsUpgradeModel__";

            name += TypeToString(turretStatsUpgradeModel.Type) + "__";
            name += ProgressionToString(turretStatsUpgradeModel.Progression) + "_";

            if (changesDamage)
            {
                name += '_' + IntToString(turretStatsUpgradeModel.DamageMultiplier) + "D";
            }
            if (changesShotsPerSecond)
            {
                name += '_' + IntToString(turretStatsUpgradeModel.ShotsPerSecondMultiplier) + "SpS";
            }
            if (changesRange)
            {
                name += '_' + IntToString(turretStatsUpgradeModel.RadiusRangeMultiplier) + "R";
            }
            if (changesPlayCost)
            {
                name += '_' + IntToString(turretStatsUpgradeModel.ExtraPlayCost) + "Cost";
            }
            if (changesLevel)
            {
                name += '_' + IntToString(turretStatsUpgradeModel.ExtraLevels, true) + "Lvl";
            }
            
            return name;
        }

        private string TypeToString(CardPartReplaceManager.BonusStatType bonusStatType)
        {
            if (CardPartReplaceManager.BonusStatType.DAMAGE == bonusStatType)
            {
                return "Damage";
            }
            if (CardPartReplaceManager.BonusStatType.SHOTS_PER_SECOND == bonusStatType)
            {
                return "ShotsPS";
            }
            if (CardPartReplaceManager.BonusStatType.RANGE == bonusStatType)
            {
                return "Range";
            }

            return "";
        }
        
        private string ProgressionToString(NodeEnums.ProgressionState progressionState)
        {
            if (NodeEnums.ProgressionState.EARLY == progressionState)
            {
                return "E";
            }
            if (NodeEnums.ProgressionState.MID == progressionState)
            {
                return "M";
            }
            if (NodeEnums.ProgressionState.LATE == progressionState)
            {
                return "L";
            }

            return "";
        }

        private string IntToString(int value, bool zeroIsNegative = false)
        {
            int negativeThreshold = zeroIsNegative ? 1 : 0;
            if (value < negativeThreshold) return '-' + Mathf.Abs(value).ToString();
            return '+' + value.ToString();
        }

        
    }
}