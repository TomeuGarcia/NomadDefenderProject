using UnityEngine;

namespace Project.Scripts.Utilities.Canvas
{
    public class HealthHUDResizeConfig
    {
        private readonly Vector2 _widthSizes = new Vector2(1200,1920);
        private readonly Vector2Int _healthThresholds = new Vector2Int(30,350);


        public void Apply(HealthSystem healthSystem, RectTransform canvasTransform)
        {
            float health = healthSystem.GetMaxHealth();
            float minHealth = _healthThresholds.x;
            float maxHealth = _healthThresholds.y;

            float healthThresholdT = Mathf.Clamp01((health - minHealth) / (maxHealth - minHealth));
            
            float width = Mathf.LerpUnclamped(_widthSizes.x, _widthSizes.y, healthThresholdT);
            
            
            Vector2 size = canvasTransform.sizeDelta;
            size.x = width;
            
            canvasTransform.sizeDelta = size;

        }
    }
}