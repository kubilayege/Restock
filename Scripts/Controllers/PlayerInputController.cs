using System;
using Core;
using MonoObjects;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Controllers
{
    public class PlayerInputController : MonoSingleton<PlayerInputController>
    {
        [SerializeField] private EventController _inGameEventController;
        [SerializeField] private PlayerController _playerController;
                
        
        private Type currentHoldType;
        private LayerMask currentHoldLayer;
            
        private Vector3 mouseDownStart;
        private Vector3 currentHoldDirection;
        private Vector3 lastPos;
        private float ScreenWidth;
        private float ScreenHeight;
        
        
        
        private void Start()
        {
            ScreenWidth = Screen.width;
            ScreenWidth = Screen.height;

            SwitchHoldType(typeof(Item), _playerController.PlayerData.itemLayerMask);
        }

        private void SwitchHoldType(Type type, LayerMask layerMask)
        {
            currentHoldType = type;
            currentHoldLayer = layerMask;
        }

        private void AddEvents()
        {
            _inGameEventController.dragStarted.AddListener(OnDragStart);
            _inGameEventController.dragged.AddListener(OnDragged);
            _inGameEventController.dragEnded.AddListener(OnDragEnd);
        }

        private void RemoveEvents()
        {
            _inGameEventController.dragStarted.RemoveListener(OnDragStart);
            _inGameEventController.dragged.RemoveListener(OnDragged);
            _inGameEventController.dragEnded.RemoveListener(OnDragEnd);
        }

        public void SetInputPanel(EventController inputPanel)
        {
            _inGameEventController = inputPanel;
            
            AddEvents();
        }

        private void OnDragStart(PointerEventData pointerEvent)
        {
            if (Helper.TryGetObjectOfType(pointerEvent.position, currentHoldLayer, out ItemHandle currentHoldingHandle))
            {
                mouseDownStart = pointerEvent.position;
                lastPos = mouseDownStart;
                _playerController.Hold(currentHoldingHandle.item);
                Helper.TryGetWorldPos(pointerEvent.position, out var worldPos, _playerController.PlayerData.movableLayerMask);
                _playerController.OnDragStart(worldPos);
                // _playerController.OnDragStart(pointerEvent.position);
            }
        }
        
        private void OnDragged(PointerEventData pointerEvent)
        {
            currentHoldDirection = ((Vector3) pointerEvent.position - mouseDownStart);
            lastPos = (Vector2) lastPos +
                      ((Vector2) currentHoldDirection *
                       _playerController.PlayerData.sensitivity);
            var isPosValid = Helper.TryGetWorldPos(lastPos, out var worldPos,
                _playerController.PlayerData.movableLayerMask);
            
            if(isPosValid)
            {
                _playerController.OnDrag(worldPos);
            }
            else
            {
                lastPos -= currentHoldDirection *
                           _playerController.PlayerData.sensitivity;
            }
            // _playerController.OnDrag(pointerEvent.position);
            
            mouseDownStart = pointerEvent.position;
        }
        
        private void OnDragEnd(PointerEventData pointerEvent)
        {
            if(Helper.TryGetObjectOfType(pointerEvent.position, _playerController.PlayerData.containerLayerMask, out Container container))
            {
            }
            
            _playerController.OnRelease(container);
        }
    }
}