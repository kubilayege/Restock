using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace MonoObjects
{
    [SelectionBase]
    public abstract class Cellular : MonoBehaviour
    {
        [SerializeField] private Cell cellPrefab;
        [FormerlySerializedAs("collider")] [SerializeField] protected Collider cellularCollider;
        
        [SerializeField] private Color cellColor;
        
        [SerializeField] private Transform gridParent;

        [SerializeField] protected float cellSize = 1f;

        [SerializeField] protected int rows;
        [SerializeField] protected int columns;

        protected Cell[,] LocalGridArea;

        [SerializeField] protected List<Cell> cells;

        protected virtual void Awake()
        {
            InitializeGrid();
            DeActivate();
        }
        
        public virtual void CreateGrid()
        {
            
#if UNITY_EDITOR
                LocalGridArea = new Cell[rows, columns];
                cells = new List<Cell>();

                foreach (var objectToDestroy in gridParent.GetComponentsInChildren<Cell>())
                {
                    DestroyImmediate(objectToDestroy.gameObject);
                }

                var rowOffset = (rows * cellSize / 2f) - ((cellSize / 2f));
                var columnOffset = (-columns * cellSize / 2f) + ((cellSize / 2f));
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        var offset = new Vector3(columnOffset + j * cellSize, 0f, rowOffset - i * cellSize);
                        LocalGridArea[i, j] =
                            (PrefabUtility.InstantiatePrefab(cellPrefab, gridParent) as Cell)?.Init(
                                new Vector2Int(i, j), offset, cellSize, this);
                        cells.Add(LocalGridArea[i, j]);

                        LocalGridArea[i, j].SetColor(cellColor);
                    }
                }
                
            
#endif

        }
        
        private void InitializeGrid()
        {
            LocalGridArea = new Cell[rows, columns];
            var row = 0;
            var column = 0;
            foreach (var cell in cells)
            {
                LocalGridArea[row, column] = cell;
                column++;

                if (column != columns) continue;

                column = 0;
                row++;
            }
        }

        public virtual void Move(Vector3 worldPos)
        {            
            DOTween.Kill(transform.GetInstanceID(), true);

            transform.position = worldPos;
            
        }
        
        
        public virtual void MoveSwerve(Vector3 swerveDirection, float yPos)
        {
            var nextPosition = (transform.position + new Vector3(swerveDirection.x, 0f, swerveDirection.y)).WithY(yPos);

            Move(nextPosition);
        }

        public virtual void MoveWithTween(Vector3 worldPos, float speed, Ease ease, Action onMoveFinished = null,
            float desiredOnMoveFinishedCallTime = 0f)
        {
            DOTween.Kill(transform.GetInstanceID(), true);
            var duration = (transform.position - worldPos).magnitude / speed;
            var elapsedTime = 0f;
            var eventCalled = false;
            transform.DOMove(worldPos, duration).SetEase(ease).SetId(transform.GetInstanceID()).OnUpdate(() =>
            {
                elapsedTime += Time.deltaTime;
                if (eventCalled || !(duration * desiredOnMoveFinishedCallTime < elapsedTime)) return;

                transform.position = worldPos;
                onMoveFinished?.Invoke();
                eventCalled = true;
            });
        }

        public virtual void DeActivate()
        {
            foreach (var cell in cells)
            {
                cell.gameObject.SetActive(false);
            }
        }

        public virtual void Activate()
        {
            foreach (var cell in cells)
            {
                cell.gameObject.SetActive(true);
            }
        }
    }
}