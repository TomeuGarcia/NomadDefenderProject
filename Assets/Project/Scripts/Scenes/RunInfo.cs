using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunInfo : MonoBehaviour
{
    private bool _isNewGame = false;
    public bool IsNewGame => _isNewGame;

    private bool _comeFromRun = false;
    public bool ComeFromRun => _comeFromRun;

    private bool _wonRun = false;
    public bool WonRun => _wonRun;

    private void OnEnable()
    {
        ServiceLocator.GetInstance().RunInfo = this;
    }

    public void SetNewGame(bool isNewGame)
    {
        _isNewGame = isNewGame;
    }

    public void SetComeFromRun(bool comeFromRun)
    {
        _comeFromRun = comeFromRun;
    }

    public void SetWonRun(bool wonRun)
    {
        _wonRun = wonRun;
    }
}
