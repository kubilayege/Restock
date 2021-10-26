using System.Collections;
using Core;
using DG.Tweening;
using Managers;
using MonoObjects;
using ScriptableObjects;
using UnityEngine;
using Utils;

namespace Controllers
{
    public class PlayerController : MonoSingleton<PlayerController>
    {
        [SerializeField] private PlayerData _playerData;
        // [Range(0f,1f)]
        // [SerializeField] private float sensitivity;
        // [Range(0f,1f)]
        // [SerializeField] private float minMoveThreshold;
        // [SerializeField] private float minStopThreshold;
        
        public PlayerData PlayerData => _playerData;
        private Item _currentHoldingItem;
        // private Coroutine moveCor;
        // private Vector3 currentDragDirection;
        // private Vector3 dragStartPosition;
        
        public void OnDragStart(Vector3 getWorldPos)
        {
            
            if (_currentHoldingItem == null) return;

            // dragStartPosition = getWorldPos;
            // currentDragDirection = getWorldPos;
            // moveCor = StartCoroutine(nameof(MoveCoroutine));
            _currentHoldingItem.Move(getWorldPos);
        }

        public void OnDrag(Vector3 getWorldPos)
        {
            if (_currentHoldingItem == null) return;

            DOTween.Kill(_currentHoldingItem.transform.GetInstanceID());

            // Debug.Log(getWorldPos);            
            // if (Vector3.Distance(currentDragDirection, getWorldPos) < minStopThreshold * 100f * (1536f / Screen.width))
            // {
            //     dragStartPosition = currentDragDirection;
            // }
            // currentDragDirection = getWorldPos;
            _currentHoldingItem.Move(getWorldPos);
        }

        public void OnRelease(Container container)
        {
            if (_currentHoldingItem)
            {
                if (_currentHoldingItem.TryToPlace(_playerData.dragSpeed))
                {
                    LevelManager.Instance.ItemPlaced(_currentHoldingItem);
                }
                else
                {
                    LevelManager.Instance.ItemRemoved(_currentHoldingItem);
                }
            }

            _currentHoldingItem = null;
            ActionManager.Instance.TriggerAction(ActionIDHolder.ItemDrop);
            // StopAllCoroutines();
            // moveCor = null;
            // currentDragDirection = Vector3.zero;
            // dragStartPosition = Vector3.zero;
        }

        public void Hold(Item currentHoldingItem)
        {
            _currentHoldingItem = currentHoldingItem;
            _currentHoldingItem.Hold(PlayerData);
            ActionManager.Instance.TriggerAction(ActionIDHolder.ItemHold);
        }


        // private IEnumerator MoveCoroutine()
        // {
        //     var yPos = Helper.GetWorldPos(currentDragDirection, PlayerData.movableLayerMask).y;
        //     _currentHoldingItem.MoveSwerve(Vector3.zero, yPos);
        //
        //     while (true)
        //     {
        //         var swerveDirection = Vector3.ClampMagnitude((currentDragDirection - dragStartPosition) / (1f/sensitivity*1000f), 1f);
        //         yPos = Helper.GetWorldPos(currentDragDirection, PlayerData.movableLayerMask).y;
        //         
        //         if(swerveDirection.magnitude > minMoveThreshold / 10f)
        //             _currentHoldingItem.MoveSwerve(swerveDirection, yPos);
        //         yield return Wait.EndOfFrame;
        //     }
        // }
    }
}
