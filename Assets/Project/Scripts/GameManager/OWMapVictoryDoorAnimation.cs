using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Scripts.GameManager
{
    public class OWMapVictoryDoorAnimation : MonoBehaviour
    {
        [Header("DOOR")]
        [SerializeField] private Transform _doorHolder;
        [SerializeField] private Vector3 _doorOpenPosition= new Vector3(0f, 0.45f, 0f);
        [SerializeField, Min(0)] private float _doorOpenMoveDuration = 5f;
        [SerializeField] private AnimationCurve _doorOpenMoveEase;
        private Vector3 _doorStartPosition;


        [Header("AUDIOS")] 
        [SerializeField] private AudioSource _openDoorAudio;
        [SerializeField] private AudioSource[] _openDoorCursedAudios;


        [Header("ROOMS")] 
        [SerializeField] private GameObject[] _rooms;
        private bool _playingBlinkingRooms;

        private void Awake()
        {
            _doorStartPosition = _doorHolder.localPosition;
        }



        public void PlayVictoryStartAnimation()
        {
            StartCoroutine(PlayDoorOpen());
            StartCoroutine(PlayCursedAudios());
            StartCoroutine(PlayBlinkingRooms());
        }
        public void PlayVictoryEndAnimation()
        {
            PlayDoorClose();
            StopCursedAudios();
            StopBlinkingRooms();
        }
        

        private IEnumerator PlayDoorOpen()
        {
            _openDoorAudio.Play();
            
            Timer doorOpenTimer = new Timer(_doorOpenMoveDuration);
            while (!doorOpenTimer.HasFinished())
            {
                float t = _doorOpenMoveEase.Evaluate(doorOpenTimer.Ratio01);
                _doorHolder.localPosition = Vector3.LerpUnclamped(_doorStartPosition, _doorOpenPosition, t);

                float audioVolumes = Mathf.Pow(doorOpenTimer.Ratio01, 2) * 0.65f;
                foreach (AudioSource openDoorCursedAudio in _openDoorCursedAudios)
                {
                    openDoorCursedAudio.volume = audioVolumes;
                }
                
                
                doorOpenTimer.Update(Time.deltaTime);
                yield return null;
            }
            
            _doorHolder.localPosition = _doorOpenPosition;
        }


        private void PlayDoorClose()
        {
            _doorHolder.localPosition = _doorStartPosition;
        }


        private IEnumerator PlayCursedAudios()
        {
            foreach (AudioSource openDoorCursedAudio in _openDoorCursedAudios)
            {
                openDoorCursedAudio.Play();
                yield return new WaitForSeconds(Random.Range(0.4f, 0.8f));
            }
        }

        private void StopCursedAudios()
        {
            foreach (AudioSource openDoorCursedAudio in _openDoorCursedAudios)
            {
                openDoorCursedAudio.Stop();
            }
        }


        private IEnumerator PlayBlinkingRooms()
        {
            _playingBlinkingRooms = true;

            while (_playingBlinkingRooms)
            {
                StartCoroutine(BlinkingRoom(Random.Range(0, _rooms.Length)));
                yield return new WaitForSeconds(Random.Range(0.2f, 0.3f));
            }
        }

        private IEnumerator BlinkingRoom(int roomIndex)
        {
            _rooms[roomIndex].SetActive(false);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
            _rooms[roomIndex].SetActive(true);
        }


        private void StopBlinkingRooms()
        {
            _playingBlinkingRooms = false;
        }
    }
}