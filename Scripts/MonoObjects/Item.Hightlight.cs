using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Utils;

namespace MonoObjects
{
    public partial class Item
    {
        private List<Cell> highlightedCells;
        private List<Cell> currentHighlightedCells;
        private List<Cell> itemHoldingCellsForHighlight;

        private IEnumerator HighlightCor()
        {
            while (true)
            {
                foreach (var highlightedCell in highlightedCells)
                {
                    highlightedCell.RemoveHighlight();
                }

                itemHoldingCellsForHighlight = new List<Cell>();
                currentHighlightedCells = new List<Cell>();

                foreach (var cell in cells)
                {
                    if (!TryGetCell(playerData.containerCellLayerMask, cell, out Cell containerCell)) continue;

                    if (!highlightedCells.Contains(containerCell))
                    {
                        highlightedCells.Add(containerCell);
                    }

                    currentHighlightedCells.Add(containerCell);

                    if (containerCell.holdingItem != null)
                    {
                        itemHoldingCellsForHighlight.Add(containerCell);
                    }
                }

                Highlight(IsPlaceFree(currentHighlightedCells, itemHoldingCellsForHighlight));

                yield return Wait.ForSeconds(playerData.highlightInterval);
            }
        }

        private void Highlight(bool isPlaceFree)
        {
            foreach (var cell in highlightedCells)
            {
                cell.RemoveHighlight();
            }

            foreach (var cell in currentHighlightedCells)
            {
                cell.DoHighlight(isPlaceFree);
            }
        }
    }
}