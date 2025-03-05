public class SBInteractionOptions : AScreenButtonInteraction
{
    public override void DoInteract(FacilityManager facilityManager)
    {
        facilityManager.TransitionToOptions();
    }
}