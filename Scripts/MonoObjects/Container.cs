using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Core.ActionData;
using DG.Tweening;
using Managers;
using UnityEngine;
using Utils;

namespace MonoObjects
{
    public class Container : Cellular
    {
        [SerializeField] private SkinnedMeshRenderer container;
        [SerializeField] private float height;
        public List<Item> items;

        protected override void Awake()
        {
            base.Awake();
            items = new List<Item>();
            ActionManager.Instance.AddAction(ActionIDHolder.ItemHold, ShowCells);
            ActionManager.Instance.AddAction(ActionIDHolder.ItemDrop, HideCells);
        }

        private void HideCells(BaseActionData data)
        {
            foreach (var cell in cells)
            {
                cell.HideCell();
            }
        }

        private void ShowCells(BaseActionData data)
        {
            foreach (var cell in cells)
            {
                cell.ShowCell();
            }
        }

        public override void CreateGrid()
        {
            base.CreateGrid();

            if (container == null) return;

            gameObject.name = $"Container {rows}x{columns}";

            container.SetBlendShapeWeight(0, columns - 1);
            container.SetBlendShapeWeight(1, rows - 1);
        }

        private void OnValidate()
        {
            if (container == null) return;

            container.SetBlendShapeWeight(2, height);
        }

        public void PlaceItem(Item item)
        {
            items.Add(item);

            if (IsContainerFilled())
            {
                StartCoroutine(nameof(AnimateItems));
                Debug.Log("Container is Filled");
            }
        }

        private IEnumerator AnimateItems()
        {
            var itemsOrdered = GetItems();

            yield return Wait.ForSeconds(0.15f);

            
            foreach (var item in itemsOrdered)
            {
                yield return Wait.ForSeconds(0.1f);

                DOTween.Kill(item.transform.GetInstanceID(), true);
                item.transform.DOJump(item.transform.position, 2f, 1, 0.4f).SetId(item.transform.GetInstanceID());
            }

            ObjectPoolManager.Instance.GetFromPool<ContainerParticle>().Activate(rows, columns, transform.position);
            DOTween.To(() => container.GetBlendShapeWeight(2), (value => container.SetBlendShapeWeight(2, value)),
                    2.25f,
                    0.35f)
                .SetEase(Ease.InOutBack)
                .OnComplete(() =>
                {
                    DOTween.To(() => container.GetBlendShapeWeight(2),
                            (value => container.SetBlendShapeWeight(2, value)), 1f,
                            0.35f)
                        .SetEase(Ease.OutExpo);
                });
        }

        private List<Item> GetItems()
        {
            var itemList = new List<Item>();
            for (int i = cells.Count - 1; i >= 0; i--)
            {
                if (!itemList.Contains(cells[i].holdingItem))
                {
                    itemList.Add(cells[i].holdingItem);
                }
            }

            return itemList;
        }

        private bool IsContainerFilled()
        {
            if (cells[0].holdingItem == null)
                return false;

            var itemId = cells[0].holdingItem.id;
            foreach (var cell in cells)
            {
                if (cell.holdingItem == null)
                {
                    return false;
                }

                if (itemId != cell.holdingItem.id)
                {
                    return false;
                }
            }

            return true;
        }

        public override void DeActivate()
        {
            base.DeActivate();
            HideCells(null);

            if (ActionManager.Instance == null) return;

            ActionManager.Instance.RemoveListener(ActionIDHolder.ItemHold, ShowCells);
            ActionManager.Instance.RemoveListener(ActionIDHolder.ItemDrop, HideCells);
        }
    }
}