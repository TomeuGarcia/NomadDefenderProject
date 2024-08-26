

using System;
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
        public readonly Sprite IconSprite;
        public readonly Color IconColor;

        public bool HasIcon => IconSprite != null;

        private Content(string name, string description, Sprite iconSprite, Color iconColor)
        {
            Name = name;
            Description = description;
            IconSprite = iconSprite;
            IconColor = iconColor;
        }

        public static Content MakeForPassiveAbility(string abilityName, string abilityDescription,
            Sprite abilityIconSprite, Color abilityIconColor)
        {
            return new Content(abilityName, abilityDescription, abilityIconSprite, abilityIconColor);
        }

        public static Content MakeForAbilityKeyword(string keywordName, string keywordDescription)
        {
            return new Content(keywordName, keywordDescription, null, Color.black);
        }
    }
    
    [Header("COMPONENTS")]
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Image _iconImage;

    private Transform _originalParent;

    private void Awake()
    {
        _originalParent = transform.parent;
    }

    public void ParentToContainer(Transform container)
    {
        transform.SetParent(container);
    }

    public void SetContent(Content content)
    {
        _nameText.gameObject.SetActive(!string.IsNullOrEmpty(content.Name));
        _nameText.text = content.Name;
        
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
    }
    
    internal override void RecycledInit() { }

    internal override void RecycledReleased()
    {
        transform.SetParent(_originalParent);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        Recycle();
    }
}