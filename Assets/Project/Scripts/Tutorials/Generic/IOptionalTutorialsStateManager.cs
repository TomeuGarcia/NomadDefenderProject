public interface IOptionalTutorialsStateManager
{
    public void SetTutorialAsDone(OptionalTutorialTypes optionalTutorialType);
    public bool IsTutorialDone(OptionalTutorialTypes optionalTutorialType);
    
    public void SetAllTutorialsNotDone();
}