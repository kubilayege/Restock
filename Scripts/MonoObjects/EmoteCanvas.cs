using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using Core;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace MonoObjects
{
    public enum EmoteType
    {
        Sad,
        Happy,
        Calm
    }
    
    public class EmoteCanvas : MonoPooled
    {
        [SerializeField] private Image emoteImage;
        
        private Person _personToFollow;

        private void Awake()
        {
        }


        public override MonoPooled Init()
        {
            return base.Init();
        }

        public override void ReturnToPool()
        {
            StopCoroutine(nameof(Initialize));
            
            base.ReturnToPool();
        }

        public void Activate(Person person, Sprite emote)
        {
            _personToFollow = person;
            emoteImage.sprite = emote;
            StartCoroutine(nameof(Initialize));
        }

        private IEnumerator Initialize()
        {
            while (true)
            {
                transform.position = _personToFollow.Position;
                transform.forward = PlayerCameraController.Instance.Forward;
                yield return Wait.EndOfFrame;
            }
        }
    }
}