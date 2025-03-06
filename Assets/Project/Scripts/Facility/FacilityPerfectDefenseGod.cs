using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacilityPerfectDefenseGod : MonoBehaviour
{
    [SerializeField] private GameProgressionStatus _gameProgressionStatus;

    private void Awake()
    {
        gameObject.SetActive(_gameProgressionStatus.Game.BeatARunWithFullPerfectDefense);
    }
}
