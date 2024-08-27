using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial2CardDrawer : CardDrawer
{

    [SerializeField] TurretPartProjectileDataModel turretPartAttackToSortFirst;

    protected override void SetupDeck()
    {
        base.SetupDeck();

        //Sort
        SortFirstCardWithWantedTurretPartAttack();
    }
    private void SortFirstCardWithWantedTurretPartAttack()
    {
        int i = 0;

        
        while (i < deck.Cards.Count)
        {
            TurretBuildingCard tempCard = deck.Cards[i] as TurretBuildingCard;

            if (tempCard != null && tempCard.CardParts.Projectile == turretPartAttackToSortFirst)
            {
                //Sorts actual card to first place in deck
                deck.Cards[i] = deck.Cards[0];
                deck.Cards[0] = tempCard;
                return;
            }

            i++;
        }

    }
}
