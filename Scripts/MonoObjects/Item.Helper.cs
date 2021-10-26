using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace MonoObjects
{
    public partial class Item
    {
        private static bool TryGetCell(LayerMask cellLayerMask, Cell cell, out Cell containerCell)
        {
            return Helper.TryGetObjectOfType(cell.WorldPosition, cell.Down, cellLayerMask,
                out containerCell);
        }

        private static bool TryGetCell(LayerMask cellLayerMask, Cell cell, Vector3 offset, out Cell containerCell)
        {
            return Helper.TryGetObjectOfType(cell.WorldPosition + offset, cell.Down, cellLayerMask,
                out containerCell);
        }
        
        private static bool TryGetBasket(LayerMask cellLayerMask, Cell cell, out Basket basket)
        {
            return Helper.TryGetObjectOfType(cell.WorldPosition, cell.Down, cellLayerMask,
                out basket);
        }

        private bool IsPlaceFree(List<Cell> cellsUnder, List<Cell> itemHoldingCells)
        {
            Cellular cellular = null;
            if (cellsUnder.Count > 0)
                cellular = cellsUnder[0].cellular;
            foreach (var cell in cellsUnder)
            {
                if (cell.cellular != cellular)
                    return false;
            }
            return (cellsUnder.Count == rows * columns) && (itemHoldingCells.Count == 0);
        }
    }
}