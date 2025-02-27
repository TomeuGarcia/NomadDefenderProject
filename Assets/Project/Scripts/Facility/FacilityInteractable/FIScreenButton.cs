using System.Collections;
using TMPro;
using UnityEngine;


public class FIScreenButton : AFacilityInteractable
{
    [SerializeField] private AScreenButtonInteraction _interaction;
    [SerializeField] private PointAndClickClickableObject _caller;
    [SerializeField] private bool _destroyCallerOnInteract = true;
    [SerializeField, Min(0)] private float _interactionDelay = 0.3f;
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
        if (_destroyCallerOnInteract)
        {
            Destroy(_caller);
        }
        
        GameAudioManager.GetInstance().PlayCardSelected();
        yield return new WaitForSeconds(_interactionDelay);
        
        _interaction.DoInteract(_facilityManager);
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