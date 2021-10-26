using System;
using System.Collections.Generic;
using System.Linq;
using Core.MoveCommands;
using UnityEngine;
using Utils;

namespace MonoObjects
{
    public partial class Item
    {
        protected List<Cell> checkCells;
        
        private bool CheckRoomToMove()
        {
            var items = new List<Item>();

            if (containerCells.Count < rows * columns)
            {
                return false;
            }
            
            foreach (var cell in containerCells.Where(cell => !items.Contains(cell.holdingItem)))
            {
                if (cell.holdingItem != null)
                {
                    items.Add(cell.holdingItem);
                }
            }

            var itemCheckLookup = new Dictionary<Item, List<CheckCommand>>();
            var isPossible = new bool[items.Count];
            foreach (var item in items)
            {
                var calculatedDirection = item.Center() - this.Center();
                var checks = new List<CheckCommand>();
                Debug.Log(calculatedDirection);
                if(calculatedDirection.x != 0)
                    checks.Add(new CheckCommand(calculatedDirection.x > 0f ? Direction.Right : Direction.Left,
                    (int) Mathf.Abs(calculatedDirection.x / cellSize)));

                
                if(calculatedDirection.z != 0)
                    checks.Add(new CheckCommand(calculatedDirection.z > 0f ? Direction.Forward : Direction.Backward,
                    (int) Mathf.Abs(calculatedDirection.x / cellSize)));

                Debug.Log(checks.Count);

                foreach (var check in checks)
                {
                    if (item.CheckIfPossible(check))
                    {
                        isPossible[items.IndexOf(item)] =true;
                    }
                }

                if (!isPossible[items.IndexOf(item)])
                {
                    return false;
                }
                
                itemCheckLookup.Add(item, checks);
            }

            foreach (var item in items)
            {
                item.MovePossiblePlace();
            }
            
            return true;
        }

        private void MovePossiblePlace()
        {
            Debug.Log(checkCells);
            foreach (var cell in containerCells)
            {
                cell.holdingItem = null;
            }
            
            Place(checkCells, playerData.dragSpeed);
            
            foreach (var cell in containerCells)
            {
                checkCells[containerCells.IndexOf(cell)] = cell;
            }
        }

        private bool CheckIfPossible(CheckCommand check)
        {
            var isPossible = true;
            var checkOffsetVector = Vector3.zero;
            switch (check.Direction)
            {
                case Direction.Forward:
                    checkOffsetVector = Vector3.forward * check.RepeatCount;
                    break;
                case Direction.Backward:
                    checkOffsetVector = Vector3.back * check.RepeatCount;
                    break;
                case Direction.Right:
                    checkOffsetVector = Vector3.right * check.RepeatCount;
                    break;
                case Direction.Left:
                    checkOffsetVector = Vector3.left * check.RepeatCount;
                    break;
                default:
                    break;
            }


            checkCells = new List<Cell>();
            
            foreach (var cell in cells)
            {
                if (!TryGetCell(playerData.containerCellLayerMask, cell, checkOffsetVector+ Vector3.up * 2f, out Cell containerCell))
                {
                    continue;
                }

                checkCells.Add(containerCell);
                Debug.Log(checkCells.Count);
                if (containerCell.holdingItem == null || containerCell.holdingItem == this)
                {
                    continue;
                }
                
                if (containerCell.holdingItem.CheckIfPossible(check))
                {
                    continue;
                }

                isPossible = false;
            }

            if (checkCells.Count != cells.Count)
            {
                isPossible = false;
            }

            return isPossible;
        }
    }
}