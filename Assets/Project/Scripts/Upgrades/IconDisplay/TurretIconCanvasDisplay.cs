using UnityEngine;
using UnityEngine.UI;

public class TurretIconCanvasDisplay : MonoBehaviour
{
    public class ConfigData
    {
        public readonly Sprite ImageSprite;
        public readonly Color ImageColor;

        public ConfigData(Sprite imageSprite, Color imageColor)
        {
            ImageSprite = imageSprite;
            ImageColor = imageColor;
        }
    }
    
    
    [SerializeField] private GameObject _holder;
    [SerializeField] private Image _iconImage;


    public void Init(ConfigData configData)
    {
        _holder.SetActive(true);
        _iconImage.sprite = configData.ImageSprite;
        _iconImage.color = configData.ImageColor;
    }

    public void InitHidden()
    {
        _holder.SetActive(false);
    }
}