using UnityEngine;
using EazyTools.SoundManager;

namespace SimpleWars
{
    public class UISoundPlayer : MonoBehaviour
    {
        public AudioClip mClick;
        public AudioClip mEnter;
        public AudioClip mExit;

        public void PlayMouseClick (float volume = 1)
        {
            SoundManager.PlayUISound(mClick, volume);
        }

        public void PlayMouseEnter(float volume = 1)
        {
            SoundManager.PlayUISound(mEnter, volume);
        }

        public void PlayMousExit(float volume = 1)
        {
            SoundManager.PlayUISound(mExit, volume);
        }
    }
}