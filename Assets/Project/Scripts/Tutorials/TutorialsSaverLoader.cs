using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialsSaverLoader
{

    /// <summary>
    /// Given a Tutorials type returns if it has been done or not by the user
    /// </summary>
    /// <param name="tutorial"></param>
    /// <returns>bool (0 or 1)</returns>
    public bool IsTutorialDone(Tutorials tutorial)
    {
        int tutorialsSaved = PlayerPrefs.GetInt("Tutorials", 0);

        if ((tutorialsSaved >> (int)tutorial) % 2 == 0)
            return false;

        return true;
    }

    /// <summary>
    /// Given a Tutorials type it will save in PlayerPrefs this tutorial as done
    /// </summary>
    /// <param name="tutorial"></param>
    public void SetTutorialDone(Tutorials tutorial)
    {
        int tutorialsSaved = PlayerPrefs.GetInt("Tutorials", 0);

        tutorialsSaved += (int)Mathf.Pow(2, (int)tutorial);
        PlayerPrefs.SetInt("Tutorials", tutorialsSaved);
    }
}
