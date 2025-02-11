using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Cards.BuildingCards.TurretAbilities.Model
{
    [CreateAssetMenu(fileName = "AbilityDescriptionPattern_NAME", 
        menuName = SOAssetPaths.CARDS + "AbilityDescriptionPattern")]
    public class AbilityDescriptionPattern : ScriptableObject
    {
        private const char DEFAULT_CHAR = '-';
        
        [Header("PATTERN")]
        [SerializeField] private char _prefix = DEFAULT_CHAR;
        [SerializeField] private char _suffix = DEFAULT_CHAR;

        [Header("MODIFICATIONS")] 
        [SerializeField] private bool _removePrefixAndSuffix = false;
        [SerializeField] private bool _makeBold = true;
        [SerializeField] private Color _color = Color.cyan;

        private void OnValidate()
        {
            if (_prefix == ' ')
            {
                _prefix = DEFAULT_CHAR;
            }
            if (_suffix == ' ')
            {
                _suffix = DEFAULT_CHAR;
            }
        }


        public void ApplyModificationsToDescription(EditableCardAbilityDescription abilityDescription)
        {
            IdentifyPatterns(abilityDescription.Description, out Dictionary<string, string> patternToModifiedPattern);
            abilityDescription.ApplyDescriptionModifications(patternToModifiedPattern);
        }


        private void IdentifyPatterns(string description, out Dictionary<string, string> patternToModifiedPattern)
        {
            patternToModifiedPattern = new Dictionary<string, string>();
            bool lookingForPrefix = true;
            bool lookingForSuffix = false;
            string patternInProcess = "";

            for (int i = 0; i < description.Length; ++i)
            {
                char currentCharacter = description[i];
                bool patternBroke = currentCharacter == ' ';
                if (patternBroke)
                {
                    lookingForPrefix = true;
                    lookingForSuffix = false;
                    patternInProcess = "";
                    continue;
                }

                if (lookingForPrefix)
                {
                    bool foundPrefix = currentCharacter == _prefix;

                    if (foundPrefix)
                    {
                        lookingForPrefix = false;
                        lookingForSuffix = true;
                        patternInProcess += currentCharacter;
                    }
                    continue;
                }
                
                patternInProcess += currentCharacter;

                if (lookingForSuffix)
                {
                    bool foundSuffix = currentCharacter == _suffix;

                    if (foundSuffix)
                    {
                        lookingForPrefix = true;
                        lookingForSuffix = false;

                        string patternToModify = _removePrefixAndSuffix 
                            ? patternInProcess.Substring(1, patternInProcess.Length - 2)
                            : patternInProcess;
                        AddPattern(patternToModifiedPattern, patternInProcess, patternToModify);
                    }

                    continue;
                }
            }
        }

        private void AddPattern(Dictionary<string, string> patternToModifiedPattern, string pattern, string patternToModify)
        {
            if (patternToModifiedPattern.ContainsKey(pattern))
            {
                return;
            }
            
            patternToModifiedPattern.Add(pattern, MakeChangesToPattern(patternToModify));
        }
        

        private string MakeChangesToPattern(string pattern)
        {
            string prefix = "<color=#" + ColorUtility.ToHtmlStringRGB(_color) + ">";
            string suffix = "</color>";

            if (_makeBold)
            {
                prefix = prefix + "<b>";
                suffix = "</b>" + suffix;
            }            
            
            return prefix + pattern + suffix;
        }    
        
        
    }
}