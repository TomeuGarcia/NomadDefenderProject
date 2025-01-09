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

    [SerializeField] private GameObject _screenParent;
    [SerializeField] private Collider _interactableCollider;

    public void Init(bool enabled, FacilityManager facilityManager)
    {
        _facilityManager = facilityManager;
        gameObject.SetActive(enabled);

        _textColorUnhovered = _text.color;
    }
    
    protected override IEnumerator DoInteract()
    {
        Destroy(_caller);
        GameAudioManager.GetInstance().PlayCardSelected();
        yield return new WaitForSeconds(0.3f);
        
        _facilityManager.TransitionToCardCollection();        
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


    public void TurnOn()
    {
        _screenParent.SetActive(true);
        _interactableCollider.enabled = true;
    }
    public void TurnOff()
    {
        _screenParent.SetActive(false);
        _interactableCollider.enabled = false;
    }
}