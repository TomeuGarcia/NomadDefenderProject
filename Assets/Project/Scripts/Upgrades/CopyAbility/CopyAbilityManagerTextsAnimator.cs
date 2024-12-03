using System.Collections;
using UnityEngine;

namespace Project.Scripts.Upgrades.CopyAbility
{
    public class CopyAbilityManagerTextsAnimator : MonoBehaviour
    {
        [SerializeField] private ScriptedSequence _scriptedSequence;


        public IEnumerator PlayInitText()
        {
            yield return new WaitForSeconds(0.5f);
            _scriptedSequence.NextLine();
        }


        public void ClearInitText()
        {
            _scriptedSequence.Clear();
        }
        
        public IEnumerator PlayCompleteText()
        {
            yield return new WaitForSeconds(1.0f);
            _scriptedSequence.NextLine();
        }
    }
        
    
}