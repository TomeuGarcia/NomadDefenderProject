using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OptionalTutorialsStateManager_PlayerPrefs : IOptionalTutorialsStateManager
{
    private const int DONE = 1; 
    private const int NOT_DONE = 0; 
    
    
    public void SetTutorialAsDone(OptionalTutorialTypes optionalTutorialType)
    {
        SetTutorialState(optionalTutorialType, DONE);
        PlayerPrefs.Save();
    }

    public bool IsTutorialDone(OptionalTutorialTypes optionalTutorialType)
    {
        string tutorialKey = TutorialTypeToKey(optionalTutorialType);
        if (!PlayerPrefs.HasKey(tutorialKey))
        {
            SetTutorialState(optionalTutorialType, NOT_DONE);
        }
        
        return PlayerPrefs.GetInt(tutorialKey) == DONE;
    }

    public void SetAllTutorialsNotDone()
    {
        IEnumerable<OptionalTutorialTypes> optionalTutorialTypes = 
            Enum.GetValues(typeof(OptionalTutorialTypes)).Cast<OptionalTutorialTypes>();
        
        foreach (OptionalTutorialTypes optionalTutorialType in optionalTutorialTypes)
        {
            SetTutorialState(optionalTutorialType, NOT_DONE);
        }
        
        PlayerPrefs.Save();
    }

    private string TutorialTypeToKey(OptionalTutorialTypes optionalTutorialType)
    {
        return optionalTutorialType.ToString();
    }

    private void SetTutorialState(OptionalTutorialTypes optionalTutorialType, int doneState)
    {
        PlayerPrefs.SetInt(TutorialTypeToKey(optionalTutorialType), doneState);
    }
}