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
    [SerializeField] private Image _borderImage;


    public void Init(ConfigData configData)
    {
        _holder.SetActive(true);
        _iconImage.sprite = configData.ImageSprite;
        _iconImage.color = configData.ImageColor;
    }
    private void InitHidden()
    {
        _holder.SetActive(false);
        _iconImage.sprite = null;
        _iconImage.color = new Color(0.1f, 0.1f, 0.1f, 1);
    }

    public void Show()
    {
        _holder.SetActive(true);
    }
    public void Hide()
    {
        _holder.SetActive(false);
    }

    public void SetBorderColor(Color color)
    {
        _borderImage.color = color;
    }
    public void ShowBorder()
    {
        _borderImage.enabled = true;
    }
    public void HideBorder()
    {
        _borderImage.enabled = false; 
    }
    
    public static void InitDisplaysArray(TurretIconCanvasDisplay[] iconDisplays, ConfigData[] iconsDisplayData)
    {
        for (int i = 0; i < iconsDisplayData.Length; ++i)
        {
            iconDisplays[i].Init(iconsDisplayData[i]);
        }
        for (int i = iconsDisplayData.Length; i < iconDisplays.Length; ++i)
        {
            iconDisplays[i].InitHidden();
        }
    }
}