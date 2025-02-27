public class SBInteractionCredits : AScreenButtonInteraction
{
    public override void DoInteract(FacilityManager facilityManager)
    {
        facilityManager.TransitionToCredits();
    }
}