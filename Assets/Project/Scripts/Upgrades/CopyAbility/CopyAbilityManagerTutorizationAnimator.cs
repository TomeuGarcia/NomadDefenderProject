using System;
using System.Collections;
using UnityEngine;

namespace Project.Scripts.Upgrades.CopyAbility
{
    public class CopyAbilityManagerTutorizationAnimator : MonoBehaviour
    {
        [SerializeField] private TextDecoder _notValidCardText;

        private void Start()
        {
            _notValidCardText.ClearDecoder();
        }

        public IEnumerator PlayNotValidCopyFromCardWasPlaced()
        {
            yield return new WaitForSeconds(0.25f);
            
            _notValidCardText.ClearDecoder();
            _notValidCardText.Activate();

            yield return new WaitUntil(() => _notValidCardText.FinishedLine);
            yield return new WaitForSeconds(0.25f);

            for (int i = 0; i < 2; i++)
            {
                _notValidCardText.gameObject.SetActive(false);
                yield return new WaitForSeconds(0.1f);
                
                GameAudioManager.GetInstance().PlayCardInfoMoveHidden();
                _notValidCardText.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.1f);
            }
            
            yield return new WaitForSeconds(0.75f);
            
            _notValidCardText.ClearDecoder();
        }

        
    }
}