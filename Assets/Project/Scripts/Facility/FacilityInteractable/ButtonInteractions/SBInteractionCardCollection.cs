public class SBInteractionCardCollection : AScreenButtonInteraction
{
    public override void DoInteract(FacilityManager facilityManager)
    {
        facilityManager.TransitionToCardCollection();
    }
    
}