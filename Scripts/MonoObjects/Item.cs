using System;
using System.Collections.Generic;
using System.Linq;
using Core.MoveCommands;
using DG.Tweening;
using Managers;
using ScriptableObjects;
using UnityEngine;
using Utils;

public enum ColliderType
{
    BoxCollider,
    CapsuleCollider,
    SphereCollider,
    MeshCollider
}

namespace MonoObjects
{
    public partial class Item : Cellular
    {
        [SerializeField] public string id;
        [Header("Sound")]
        [SerializeField] private AudioClip soundEffect;

        [Range(0f,1f)]
        [SerializeField] private float placementDelay;

        [SerializeField] private AudioClipSettings soundEffectSettings;

        public Ease placeEase;

        public float placeSpeed;

        private List<Cell> containerCells;

        private Vector3[] directions =
        {
            new Vector3(-1f, 0f, 0f),  //L
            new Vector3( 1f, 0f, 0f), //R
            new Vector3( 0f,0f, 1f), //F
            new Vector3( 0f, 0f, -1f), //B
            new Vector3(-1f, 0f, -1f), //LB
            new Vector3(-1f, 0f, 1f), //LF
            new Vector3( 1f, 0f, 1f), //RF
            new Vector3( 1f, 0f, -1f), //RB
        };

        private List<Cell> itemHoldingCells;
        
        [SerializeField] private ColliderType typeOfCollider;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Transform modelParent;

        private List<Cell> oldContainerCells;
        private Vector3 oldPlace;

        private PlayerData playerData;
        private Basket basket;
        [SerializeField] private bool doSmartPlace = false;
        private Container container;

        private Vector3 startingPoint;

        protected override void Awake()
        {
            base.Awake();
            startingPoint = transform.position;
        }

        public override void CreateGrid()
        {
            base.CreateGrid();
            if (modelParent == null || modelParent.childCount == 0)
            {
                return;
            }

            var oldCollider = cellularCollider;

            var modelObject = modelParent.GetChild(0).gameObject;
            if (oldCollider == null)
            {
                modelObject.TryGetComponent(out oldCollider);
                if (modelObject.TryGetComponent(out ItemHandle itemHandle))
                {
                    DestroyImmediate(itemHandle);
                }
            }

            switch (typeOfCollider)
            {
                case ColliderType.BoxCollider:
                    cellularCollider = (Collider) modelObject.AddComponent(typeof(BoxCollider));
                    var oldColliderY = ((BoxCollider) cellularCollider).size.y;

                    if (oldCollider != null)
                        oldColliderY = ((BoxCollider) oldCollider).size.y;

                    ((BoxCollider) cellularCollider).size = new Vector3(columns * cellSize, oldColliderY, rows * cellSize);
                    ((BoxCollider) cellularCollider).center = new Vector3(0, oldColliderY / 2f, 0);

                    break;
                case ColliderType.CapsuleCollider:

                    cellularCollider = (Collider) modelObject.AddComponent(typeof(CapsuleCollider));
                    if (rows > columns)
                    {
                        ((CapsuleCollider) cellularCollider).direction = 2;
                        ((CapsuleCollider) cellularCollider).height = rows;
                    }
                    else
                    {
                        ((CapsuleCollider) cellularCollider).direction = 0;
                        ((CapsuleCollider) cellularCollider).height = columns;
                    }

                    break;
                case ColliderType.SphereCollider:

                    cellularCollider = (Collider) modelObject.AddComponent(typeof(SphereCollider));
                    ((SphereCollider) cellularCollider).radius = Mathf.Min(rows, columns) / 2f;
                    ((SphereCollider) cellularCollider).center = Vector3.up * ((Mathf.Min(rows, columns) / 2f) - cellSize / 2f);

                    break;
                case ColliderType.MeshCollider:

                    cellularCollider = (Collider) modelObject.AddComponent(typeof(MeshCollider));
                    ((MeshCollider) cellularCollider).sharedMesh =
                        modelParent.GetChild(0).GetComponent<MeshFilter>().sharedMesh;
                    ((MeshCollider) cellularCollider).convex = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ((ItemHandle) modelObject.AddComponent(typeof(ItemHandle))).Init(this);

            if (oldCollider != null)
            {
                DestroyImmediate(oldCollider);
            }
        }

        public bool TryToPlace(float speed)
        {
            StopCoroutine(nameof(HighlightCor));
            
            bool isPlaced = false;
            itemHoldingCells = new List<Cell>();
            containerCells = new List<Cell>();
            foreach (var cell in cells)
            {
                if (basket == null && TryGetBasket(playerData.freeFormLayerMask, cell, out basket))
                {
                }

                if (!TryGetCell(playerData.containerCellLayerMask, cell, out Cell containerCell)) continue;


                containerCells.Add(containerCell);

                if (containerCell.holdingItem != null)
                {
                    itemHoldingCells.Add(containerCell);
                }
            }

            
            
            if (basket != null)
            {
                ReturnToBasket(basket, 0.1f);
            }
            else
            {
                if (IsPlaceFree(containerCells, itemHoldingCells))
                {
                    if (playerData.doPhysicsOnPlace)
                    {
                        hasPlaceToFit = true;
                        Drop();
                    }
                    else
                    {
                        Place(containerCells, placeSpeed);
                    }

                    isPlaced = true;
                }
                else
                {
                    var placeItem = false;
                    var originalPosition = transform.position;

                    for (int i = 0; i < directions.Length; i++)
                    {
                        itemHoldingCells = new List<Cell>();
                        containerCells = new List<Cell>();
                        var currentOffset = directions[i] * cellSize;
                        transform.position += currentOffset;
                        foreach (var cell in cells)
                        {
                            if (basket == null && TryGetBasket(playerData.freeFormLayerMask, cell, out basket))
                            {
                            }

                            if (!TryGetCell(playerData.containerCellLayerMask, cell, out Cell containerCell)) continue;


                            containerCells.Add(containerCell);

                            if (containerCell.holdingItem != null)
                            {
                                itemHoldingCells.Add(containerCell);
                            }
                        }

                        transform.position = originalPosition;
                        if (!IsPlaceFree(containerCells, itemHoldingCells)) continue;
                        
                        placeItem = true;
                        
                        break;
                    }
                    
                    if (placeItem)
                    {
                        Place(containerCells, placeSpeed);
                        isPlaced = true;
                    }
                    else
                    {
                        isPlaced = Cancel(speed); 
                    }
                }
            }


            foreach (var highlightedCell in highlightedCells)
            {
                highlightedCell.RemoveHighlight();
            }

            return isPlaced;
        }

        [SerializeField] private bool hasPlaceToFit;

        private void Place(List<Cell> placeCells, float speed)
        {
            var zippedCells = cells.Zip(placeCells,
                (iC, cC)
                    => new {ItemCell = iC, ContainerCell = cC});

            foreach (var cell in zippedCells)
            {
                cell.ItemCell.containerCell = cell.ContainerCell;
                cell.ContainerCell.holdingItem = this;
            }

            container = (Container) cells[0].containerCell.cellular;

            transform.parent = container.transform;
            container.PlaceItem(this);


            var position = placeCells[0].WorldPosition - cells[0].localPosition;

            
            MoveWithTween(position, speed, placeEase,
                () =>
                {
                    ObjectPoolManager.Instance.GetFromPool<AudioSourceController>()?.PlayOnce(soundEffect, soundEffectSettings);
                    
                },placementDelay);
        }


        public override void MoveWithTween(Vector3 worldPos, float speed, Ease ease, Action onMoveFinished = null,
            float desiredOnMoveFinishedCallTime = 0f)
        {
            base.MoveWithTween(worldPos, speed, ease, onMoveFinished, desiredOnMoveFinishedCallTime);
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if (hasPlaceToFit)
            {
                _rigidbody.isKinematic = true;
                transform.rotation = Quaternion.identity;
                Place(containerCells, placeSpeed);
                hasPlaceToFit = false;
            }
        }

        private void Drop()
        {
            _rigidbody.isKinematic = false;
        }


        private bool Cancel(float speed)
        {
            if (oldContainerCells != null && oldContainerCells.Count != 0)
            {
                foreach (var cell in cells)
                {
                    cell.containerCell.holdingItem = this;
                }

                ((Container) cells[0].containerCell.cellular).PlaceItem(this);
                Move(oldPlace);
                return true;
            }
            else
            {
                Debug.Log("Cancel");
                _rigidbody.isKinematic = false;
                transform.rotation = Quaternion.identity;

                containerCells = new List<Cell>();

                Move(oldPlace);
                return false;
            }
        }


        public void Hold(PlayerData _playerData)
        {
            DOTween.Kill(transform.GetInstanceID());
            oldPlace = transform.position;
            playerData = _playerData;
            _rigidbody.isKinematic = true;
            transform.rotation = Quaternion.identity;

            if (highlightedCells != null)
            {
                foreach (var cell in highlightedCells)
                {
                    cell.RemoveHighlight();
                }
            }

            oldContainerCells = new List<Cell>();
            highlightedCells = new List<Cell>();
            currentHighlightedCells = new List<Cell>();

            foreach (var cell in cells)
            {
                if (cell.containerCell != null)
                {
                    oldContainerCells.Add(cell);
                    cell.containerCell.holdingItem = null;
                    ((Container) cell.containerCell.cellular).items.Remove(this);
                }
            }
            
            // ObjectPoolManager.Instance.GetFromPool<AudioSourceController>()?.PlayOnce(soundEffect, soundEffectSettings);

            
            StartCoroutine(nameof(HighlightCor));
        }

        public void ReturnToBasket(Basket basket, float duration)
        {
            foreach (var cell in cells)
            {
                if (cell.containerCell != null)
                {
                    cell.containerCell.holdingItem = null;
                    ((Container) cell.containerCell.cellular).items.Remove(this);
                }
            }

            transform.parent = basket.transform;
            Debug.Log("Basket");
            basket.Take(this, duration);
            this.basket = null;
        }

        public void BackToBasket()
        {
            _rigidbody.isKinematic = false;
        }

        public Vector3 Center()
        {
            Vector3 center = Vector3.zero;
            foreach (var cell in currentHighlightedCells)
            {
                center += cell.WorldPosition;
            }

            Debug.Log(center / currentHighlightedCells.Count);
            return center / currentHighlightedCells.Count;
        }

        public override void DeActivate()
        {
            base.DeActivate();
            cellularCollider.enabled = false;
        }

        public override void Activate()
        {
            base.Activate();

            cellularCollider.enabled = true;
        }

        public void ResetItem()
        {
            transform.position = startingPoint;
        }

        public void InitialItem()
        {
            _rigidbody.isKinematic = true;
            DeActivate();
        }
    }
}