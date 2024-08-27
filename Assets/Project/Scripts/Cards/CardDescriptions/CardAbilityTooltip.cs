

using System;
using DG.Tweening;
using Scripts.ObjectPooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardAbilityTooltip : RecyclableObject
{
    public class Content
    {
        public readonly string Name;
        public readonly string Description;
        public readonly Color NameColor;
        public readonly Sprite IconSprite;
        public readonly Color IconColor;
        public readonly bool FixedWidth;

        public bool HasIcon => IconSprite != null;

        private Content(string name, string description, Color nameColor, Sprite iconSprite, Color iconColor,
            bool fixedWidth)
        {
            Name = name;
            Description = description;
            NameColor = nameColor;
            IconSprite = iconSprite;
            IconColor = iconColor;
            FixedWidth = fixedWidth;
        }

        public static Content MakeForPassiveAbility(CardTooltipDisplayData.Element displayDataElement)
        {
            return new Content(
                displayDataElement.AbilityDescription.NameForDisplay, 
                displayDataElement.AbilityDescription.Description, 
                Color.white,
                displayDataElement.AbilitySprite, displayDataElement.AbilityColor,
                displayDataElement.FixedTooltipWidth);
        }

        public static Content MakeForAbilityKeyword(CardAbilityKeyword keyword)
        {
            return new Content(keyword.Name, keyword.Description, keyword.NameColor,
                null, Color.black, true);
        }
    }
    
    [Header("COMPONENTS")]
    [SerializeField] private RectTransform _contentHolder;
    [SerializeField] private LayoutElement _layoutElement;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Image _iconImage;

    private Transform _originalParent;

    public RectTransform ContentHolder => _contentHolder;
    

    public void SetOriginalParent(Transform originalParent)
    {
        _originalParent = originalParent;
    }

    public void ParentToContainer(Transform container)
    {
        transform.SetParent(container);
    }

    public void SetContent(Content content)
    {
        _nameText.gameObject.SetActive(!string.IsNullOrEmpty(content.Name));
        _nameText.text = content.Name;
        _nameText.color = content.NameColor;
        
        _descriptionText.gameObject.SetActive(!string.IsNullOrEmpty(content.Description));
        _descriptionText.text = content.Description;

        if (content.HasIcon)
        {
            _iconImage.gameObject.SetActive(content.HasIcon);
            _iconImage.sprite = content.IconSprite;
            _iconImage.color = content.IconColor;
        }
        else
        {
            _iconImage?.gameObject.SetActive(false);
        }

        _layoutElement.enabled = content.FixedWidth;
    }
    
    internal override void RecycledInit() { }

    internal override void RecycledReleased()
    {
        transform.SetParent(_originalParent);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        transform.DOPunchScale(Vector3.one * 0.05f, 0.15f);
    }
    public void Hide()
    {
        Recycle();
        transform.DOComplete();
    }
}