using DG.Tweening;
using UnityEngine;

namespace ScriptableObjects
{
    
    [CreateAssetMenu(menuName = "Player/Data")]
    public class PlayerData : ScriptableObject
    {
        public float dragSpeed;
        public float placeSpeed;
        public LayerMask itemLayerMask;
        public LayerMask movableLayerMask;
        public LayerMask freeFormLayerMask;
        public LayerMask containerLayerMask;
        public LayerMask containerCellLayerMask;
        public bool doPhysicsOnPlace;
        public Ease moveEase;
        public float highlightInterval = 0.3f;
        public float sensitivity;
    }
}