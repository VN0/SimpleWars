using UnityEngine;
using DG.DeAudio;

namespace SimpleWars
{
    public class UISoundPlayer : MonoBehaviour
    {
        public AudioClip mClick;
        public AudioClip mEnter;
        public AudioClip mExit;

        public void PlayMouseClick (float volume = 1)
        {
            DeAudioManager.Play(mClick, volume);
        }

        public void PlayMouseEnter(float volume = 1)
        {
            DeAudioManager.Play(mEnter, volume);
        }

        public void PlayMousExit(float volume = 1)
        {
            DeAudioManager.Play(mExit, volume);
        }
    }
}