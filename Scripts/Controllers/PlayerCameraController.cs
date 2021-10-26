using System.Collections;
using Core;
using DG.Tweening;
using UnityEngine;
using Utils;

namespace Controllers
{
    public class PlayerCameraController : MonoSingleton<PlayerCameraController>
    {

        [SerializeField] private Transform cameraParent;
        [SerializeField] private float duration;
        
        private Transform popUpStar;
        public Vector3 Forward => cameraParent.forward;

        public void MoveTo(Transform targetTransform, float _duration = 0f, bool doRotate = true,
            Ease ease = Ease.Linear)
        {
            StopCoroutine(nameof(Look));
            DOTween.Kill(cameraParent.GetInstanceID());

            if (popUpStar != null)
            {
                popUpStar.gameObject.SetActive(false);
                popUpStar = null;
            }

        cameraParent.DOMove(targetTransform.position, _duration != 0 ? _duration : duration)
                .SetId(cameraParent.GetInstanceID()).SetEase(ease);
            
            if (!doRotate)
                return;
            
            cameraParent.DORotate(targetTransform.rotation.eulerAngles, _duration != 0 ? _duration : duration)
                .SetId(cameraParent.GetInstanceID()).SetEase(ease);
        }

        public void LookAt(Transform target, float waitTimeBeforeFollowing)
        {
            // var offset = Vector3.up * 5f;
            // cameraParent.DOLookAt(target.position, 0.01f).SetLoops(-1, LoopType.Restart).SetId(cameraParent.GetInstanceID());
            
            StartCoroutine(nameof(Look), new LookData(target, waitTimeBeforeFollowing));
        }

        private IEnumerator Look(LookData lookData)
        {
            yield return Wait.ForSeconds(lookData.WaitTimeBeforeFollowing);
            while (true)
            {
                cameraParent.DOLookAt(lookData.Target.position, 0.01f, AxisConstraint.None);
                yield return Wait.ForSeconds(0.01f);
            }
        }

        private class LookData
        {
            public Transform Target { get; }
            public float WaitTimeBeforeFollowing { get; }

            public LookData(Transform target, float waitTimeBeforeFollowing)
            {
                Target = target;
                WaitTimeBeforeFollowing = waitTimeBeforeFollowing;
            }
        }

        public void PopUpStar(Transform popUpStarCanvas, Vector3 offset)
        {
            popUpStarCanvas.gameObject.SetActive(true);
            popUpStar = popUpStarCanvas;
            var forward = cameraParent.forward;
            popUpStar.position = cameraParent.position + (forward * offset.z) + (cameraParent.up * offset.y);
            popUpStarCanvas.forward = forward;
        }
    }
}