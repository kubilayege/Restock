using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Utils;

namespace MonoObjects
{
    public class Basket : MonoBehaviour
    {
        [SerializeField] private Transform[] points;
        [SerializeField] private ParticleSystem[] fireworks;
        
        private int lastPointIndex;

        private int GetPointIndex => (++lastPointIndex % points.Length);
        
        public void Take(Item item, float duration)
        {
            // Debug.Log("TakeBasket");
            DOTween.Kill(item.transform.GetInstanceID(),true);
            item.transform.DOMoveZ(transform.position.z, duration).OnComplete(item.BackToBasket).SetId(item.transform.GetInstanceID()).SetEase(Ease.Linear);
        }

        public void Spawn(Item item)
        {
            item.gameObject.SetActive(true);
            item.Activate();
            item.transform.position = points[GetPointIndex].position;
            item.BackToBasket();
        }

        public void Move(Transform point, float duration)
        {
            DOTween.Kill(transform.GetInstanceID());
            transform.DOMove(point.position, duration);
            transform.DORotate(point.rotation.eulerAngles, duration);
        }


        public void Fireworks()
        {
            StartCoroutine(nameof(FireWorksCor));
        }

        private IEnumerator FireWorksCor()
        {
            yield return Wait.ForSeconds(2f);
            foreach (var firework in fireworks)
            {
                firework.Play();
                yield return Wait.ForSeconds(0.5f);
            }
        }
    }
}