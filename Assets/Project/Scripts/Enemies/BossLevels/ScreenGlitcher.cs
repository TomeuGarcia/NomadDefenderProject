using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Project.Scripts.Enemies.BossLevels
{
    public class ScreenGlitcher : MonoBehaviour
    {
        [SerializeField] private Volume _globalVolume;
        [SerializeField] private VolumeProfile _initVol;
        [SerializeField] private VolumeProfile _glitchVol;

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        public IEnumerator PlayGlitch(
            float startDelay = 0.0f,  
            float delayBetweenGlitch = 0.2f, 
            float delayBetweenGlitchMultiplier = 1.0f, 
            int times = 1)
        {
            yield return new WaitForSecondsRealtime(startDelay);
            GameAudioManager.GetInstance().PlayGlitchSound(2);

            for (int i = 0; i < times; ++i)
            {
                _globalVolume.profile = _glitchVol;
                yield return new WaitForSecondsRealtime(delayBetweenGlitch);
                _globalVolume.profile = _initVol;
                yield return new WaitForSecondsRealtime(delayBetweenGlitch);

                delayBetweenGlitch *= delayBetweenGlitchMultiplier;
            }
        }
        
        
        
    }
}