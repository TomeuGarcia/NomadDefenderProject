using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace Project.Scripts.Enemies.BossLevels
{
    public class ScreenGlitcher : MonoBehaviour
    {
        [SerializeField] private Volume _globalVolume;
        [SerializeField] private VolumeProfile _initVol;
        [SerializeField] private VolumeProfile _glitchVol;
        [SerializeField] private Vector2 _highMusicPitchRange = new Vector2(1.5f, 2.0f); 
        [SerializeField] private Vector2 _lowMusicPitchRange = new Vector2(0.5f, 0.8f); 

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        public IEnumerator PlayGlitch(
            float startDelay = 0.0f,  
            float delayBetweenGlitch = 0.2f, 
            float delayBetweenGlitchMultiplier = 1.0f, 
            int times = 1,
            bool withPitchVariation = true
            )
        {
            yield return new WaitForSecondsRealtime(startDelay);
            GameAudioManager.GetInstance().PlayGlitchSound(2);

            for (int i = 0; i < times; ++i)
            {
                if (withPitchVariation)
                {
                    GameAudioManager.GetInstance().SetMusicPitch(Random.Range(_lowMusicPitchRange.x, _lowMusicPitchRange.y));
                }
                _globalVolume.profile = _glitchVol;
                yield return new WaitForSecondsRealtime(delayBetweenGlitch);

                if (withPitchVariation)
                {
                    GameAudioManager.GetInstance().SetMusicPitch(Random.Range(_highMusicPitchRange.x, _highMusicPitchRange.y));                    
                }
                _globalVolume.profile = _initVol;
                yield return new WaitForSecondsRealtime(delayBetweenGlitch);

                delayBetweenGlitch *= delayBetweenGlitchMultiplier;
            }
        }
        
        
        
    }
}