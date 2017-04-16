using UnityEngine;
using DG.DeAudio;

namespace SimpleWars
{
    public class UISoundPlayer : Singleton<UISoundPlayer>
    {
        public AudioClip mClick;
        public AudioClip mEnter;
        public AudioClip mExit;

        public void PlayMouseClick (float volume = 1)
        {
            DeAudioManager.Play(DeAudioGroupId.UI, mClick, volume);
        }

        public void PlayMouseEnter(float volume = 1)
        {
            DeAudioManager.Play(DeAudioGroupId.UI, mEnter, volume);
        }

        public void PlayMouseExit(float volume = 1)
        {
            DeAudioManager.Play(DeAudioGroupId.UI, mExit, volume);
        }
    }
}