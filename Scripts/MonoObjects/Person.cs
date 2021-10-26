using System;
using Core;
using DG.Tweening;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace MonoObjects
{
    public class Person : MonoBehaviour
    {
        private Animator _animator;
        private EmoteCanvas _emoteCanvas;
        public Transform handL;
        public Transform handR;

        private readonly int Walk = Animator.StringToHash("walk");
        private readonly int Run = Animator.StringToHash("run");
        private readonly int Cry = Animator.StringToHash("cry");
        private readonly int Eat = Animator.StringToHash("eat");
        private readonly int Idle = Animator.StringToHash("idle");
        private int _previousAnim;

        private Bag[] _bags;
        public Vector3 Position => transform.position;

        public void TriggerAnim(int anim)
        {
            _animator.ResetTrigger(_previousAnim);
            _animator.SetTrigger(anim);
            _previousAnim = anim;
        }

        private void Awake()
        {
            _previousAnim = Idle;
            _animator = gameObject.GetComponent<Animator>();
        }

        public Person MoveAlong(Path pathToFollow, float duration, Action onPathComplete, Sprite emote = null,
            Bag[] bags = null)
        {
            transform.position = pathToFollow.GetPath()[0];
            // animator.SetTrigger(Walk);
            
            if(emote != null)
                Emote(emote);

            _bags = bags;
            transform.DOPath(pathToFollow.GetPath(), duration, PathType.CatmullRom).SetEase(Ease.Linear).SetLookAt(0f)
                .OnComplete(() =>
                {
                    
                    if(emote != null)
                        ReturnCanvasToPool();
                    onPathComplete();
                });


            if (bags != null)
            {
                Equip(bags);
            }

            return this;
        }

        private void ReturnCanvasToPool()
        {
            _emoteCanvas.ReturnToPool();
            _emoteCanvas = null;
        }

        public void Wait()
        {
            TriggerAnim(Idle);
            // _bags[0].DropItems();
            // _bags[1].DropItems();
        }

        public void React()
        {
            TriggerAnim(Cry);
        }

        public void WalkFast()
        {
            TriggerAnim(Run);
        }

        public void WalkSlow()
        {
            TriggerAnim(Walk);
        }

        public void EatFromFridge()
        {
            TriggerAnim(Eat);
        }

        public void Emote(Sprite emote)
        {
            if (_emoteCanvas == null)
                _emoteCanvas = ObjectPoolManager.Instance.GetFromPool<EmoteCanvas>();

            _emoteCanvas.Activate(this, emote);
        }

        private void Equip(Bag[] bags)
        {
            bags[0].Asign(handL);
            bags[1].Asign(handR);
        }

        public void Return(Path path, bool doReverse, float duration, Action<Person> takePersonBack, Sprite emote,
            float delay = 0f)
        {
            transform.position = doReverse ? path.GetReversePath()[0] : path.GetPath()[0];
            transform.DOPath(doReverse ? path.GetReversePath() : path.GetPath(), duration, PathType.CatmullRom)
                .SetDelay(delay).SetLookAt(0f)
                .SetEase(Ease.Linear)
                .OnStart(() =>
                {
                    WalkFast();
                    Emote(emote);
                })
                .OnComplete((
                    () =>
                    {
                        ReturnCanvasToPool();

                        takePersonBack(this);
                    }));
        }

        public void ReturnKitchenPerson(Path path, bool doReverse, float duration, Action<Person> takePersonBack,
            Sprite emote, float delay = 0f)
        {
            Emote(emote);

            transform.position = doReverse ? path.GetReversePath()[0] : path.GetPath()[0];
            transform.DOPath(doReverse ? path.GetReversePath() : path.GetPath(), duration, PathType.CatmullRom)
                .SetDelay(delay).SetLookAt(0f)
                .SetEase(Ease.Linear)
                .OnComplete((
                    () =>
                    {
                        ReturnCanvasToPool();
                        takePersonBack(this);
                    }));
        }
    }
}