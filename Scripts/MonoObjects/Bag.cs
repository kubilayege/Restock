using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace MonoObjects
{
    public class Bag : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer meshRenderer;
        [SerializeField] private Vector3 rotation;

        private Transform _originalParent;

        private void Awake()
        {
            _originalParent = transform.parent;
        }

        public void Asign(Transform hand)
        {
            gameObject.SetActive(true);
            DOTween.To(() => meshRenderer.GetBlendShapeWeight(0),
                x => meshRenderer.SetBlendShapeWeight(0, x), 0f, 0.1f);
            transform.position = hand.position;
            transform.parent = hand;
            transform.localRotation = Quaternion.Euler(rotation);
        }

        public void DropItems()
        {
            gameObject.SetActive(false);
            transform.parent = _originalParent;
            // DOTween.To(() => meshRenderer.GetBlendShapeWeight(0),
            //     x => meshRenderer.SetBlendShapeWeight(0, x), 100f, 0.3f);
        }
        
        
    }
}