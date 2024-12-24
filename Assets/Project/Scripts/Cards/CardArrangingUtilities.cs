using UnityEngine;

namespace Project.Scripts.Cards
{
    public static class CardArrangingUtilities
    {
        
        
        public static Vector3[] GetCenteredCards(Transform cardsHolder, int cardsCount, int cardsPerRow,
            float spacingBetweenRows = 0.2f, float spacingBetweenCards = 0.1f, float cardWidth = 1.0f, float cardHeight = 1.5f)
        {
            Vector3[] cardsEndPositions = new Vector3[cardsCount];
        
            int totalRows = Mathf.CeilToInt((float)cardsCount / cardsPerRow);

            int cardsInLastRow = cardsCount % cardsPerRow == 0
                ? cardsPerRow
                : cardsCount - ((cardsCount / cardsPerRow) * cardsPerRow);

            for (int i = 0; i < cardsCount; ++i)
            {
                int rowIndex = i / cardsPerRow;
                bool isLastRow = rowIndex + 1 == totalRows;
            
                int previousCards = rowIndex * cardsPerRow;
                int cardsInRow = isLastRow ? cardsInLastRow : cardsPerRow;

            
                int cardInRowIndex = (i - previousCards) % cardsInRow;
                float totalSideSpacing = ((cardsInRow - 1) * (cardWidth + spacingBetweenCards));
                float totalUpwardsSpacing = ((totalRows - 1) * (cardHeight + spacingBetweenRows));
            
                Vector3 position = cardsHolder.position;
                position += new Vector3(
                    ComputeCenteredCardsSpacing(cardInRowIndex, cardsInRow, totalSideSpacing),
                    0,
                    -ComputeCenteredCardsSpacing(rowIndex, totalRows, totalUpwardsSpacing));

                cardsEndPositions[i] = position;
            }

            return cardsEndPositions;
        }
    
        private static float ComputeCenteredCardsSpacing(int index, int totalCount, float totalSpacing)
        {
            if (totalCount < 2) return 0;
        
            float halfSpace = totalSpacing * 0.5f;

            float t = (float)index / (totalCount - 1);
            float spacing = Mathf.LerpUnclamped(-halfSpace, halfSpace, t);
            return spacing;
        }
    }
}