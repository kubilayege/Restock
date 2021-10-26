using System;
using System.Collections;
using Core;
using DG.Tweening;
using Managers;
using UnityEngine;
using Utils;

namespace MonoObjects
{
    public class AudioSourceController : MonoPooled
    {
        [SerializeField] private AudioSource source;

        private AudioClipSettings _settings;
        
        public override MonoPooled Init()
        {
            return base.Init();
        }

        public void PlayOnce(AudioClip clip, AudioClipSettings settings)
        {
            
            source.loop = false;
            source.volume = settings.volume;
            source.pitch = settings.pitch;
            
            StartCoroutine(nameof(PlayClipOnce), clip);
        }

        public void PlayLoop(AudioClip clip, AudioClipSettings settings)
        {
            // if(PlayerPrefs.GetInt("Music") == 0)
            // {
            //     ReturnToPool();
            //     return;
            // }

            _settings = settings;
            source.loop = settings.doLoop;
            source.volume = PlayerPrefs.GetInt("Music") == 0 ? 0 : _settings.volume;
            source.pitch = settings.pitch;
            source.clip = clip;
            source.Play();
        }
        
        public IEnumerator PlayClipOnce(AudioClip clip)
        {
            source.PlayOneShot(clip);
            yield return Wait.ForSeconds(clip.length);
            ReturnToPool();
        }

        public override void ReturnToPool()
        {
            base.ReturnToPool();
        }

        public void ToggleMusic()
        {
            source.volume = PlayerPrefs.GetInt("Music") == 0 ? 0 : _settings.volume;
        }
    }

    [Serializable]
    public struct AudioClipSettings
    {
        public float volume;
        public float pitch ;
        public bool doLoop;

        public AudioClipSettings(float vol = 1f, float pitch = 1f, bool doLoop = false)
        {
            volume = vol;
            this.pitch = pitch;
            this.doLoop = doLoop;
        }
    }
}