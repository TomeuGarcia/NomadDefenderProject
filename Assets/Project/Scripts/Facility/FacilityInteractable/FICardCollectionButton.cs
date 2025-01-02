using System.Collections;
using TMPro;
using UnityEngine;


public class FICardCollectionButton : AFacilityInteractable
{
    [SerializeField] private PointAndClickClickableObject _caller;
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private Color _textColorHovered = Color.cyan;
    private Color _textColorUnhovered;
    
    private FacilityManager _facilityManager;


    public void Init(bool enabled, FacilityManager facilityManager)
    {
        _facilityManager = facilityManager;
        gameObject.SetActive(enabled);

        _textColorUnhovered = _text.color;
    }
    
    protected override IEnumerator DoInteract()
    {
        Destroy(_caller);
        _facilityManager.TransitionToCardCollection();
        
        GameAudioManager.GetInstance().PlayCardSelected();
        
        yield return null;
    }

    public override void Hovered()
    {
        _text.color = _textColorHovered;
        GameAudioManager.GetInstance().PlayCardInfoShown();
    }

    public override void Unhovered()
    {
        _text.color = _textColorUnhovered;
        GameAudioManager.GetInstance().PlayCardInfoHidden();
    }
}