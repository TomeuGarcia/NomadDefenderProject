using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CATEGORY_CardsLibraryContent", menuName = "Cards/CardsLibraryContent")]
public class CardsLibraryContent : ScriptableObject
{
    [Header("GROUPED CARD PARTS")]
    [SerializeField] public TurretCardParts[] totalTurretCardParts;
    [SerializeField] public SupportCardParts[] totalSupportCardParts;


}
