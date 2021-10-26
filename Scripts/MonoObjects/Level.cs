using System.Collections;
using System.Collections.Generic;
using Controllers;
using Core;
using Core.ActionData;
using DG.Tweening;
using Managers;
using UnityEngine;
using Utils;

namespace MonoObjects
{
    public class Level : MonoBehaviour
    {
        [SerializeField] private float itemEatInterval = 0.05f;
        [SerializeField] private int itemEatRate = 5;

        [SerializeField] private ParticleSystem[] _confetti;
        private Transform itemsParent;
        private Transform containersParent;
        [SerializeField] private Transform moveArea;
        [SerializeField] private Basket basket;
        [SerializeField] private Transform basketStartPoint;
        [SerializeField] private FridgeController _fridgeController;
        private List<Item> unPlacedItems;
        private List<Item> PlacedItems;
        private List<Container> containers;
        private List<Item> allItems;


        private int _currentSegmentIndex;

        private IEnumerator InitItemAndContainers()
        {
            yield return null;
            // Debug.Log("ItemTake");
            if (containers != null)
            {
                foreach (var container in containers)
                {
                    container.DeActivate();
                }

                foreach (var placedItem in PlacedItems)
                {
                    placedItem.DeActivate();
                }
            }

            unPlacedItems = new List<Item>();
            PlacedItems = new List<Item>();
            containers = new List<Container>();

            itemsParent = _fridgeController.GetCurrentItemsParent();
            containersParent = _fridgeController.GetCurrentContainersParent();

            yield return Wait.ForSeconds(0.8f);

            for (int i = 0; i < containersParent.childCount; i++)
            {
                var container = containersParent.GetChild(i).GetComponent<Container>();
                containers.Add(container);
                container.Activate();
            }
            
            
            foreach (var container in containers)
            {
                var containerTransform = container.transform;
                for (int i = 0; i < containerTransform.childCount; i++)
                {
                    if (!containerTransform.GetChild(i).TryGetComponent(out Item item)) continue;
                    item.Hold(PlayerController.Instance.PlayerData);
                    allItems.Add(item);
                    if(item.TryToPlace(5f))
                    {
                        PlacedItems.Add(item);
                    }
                    else
                    {
                        unPlacedItems.Add(item);
                    }
                    item.Activate();
                }
            }
            
            for (int i = 0; i < itemsParent.childCount; i++)
            {
                var item = itemsParent.GetChild(i).GetComponent<Item>();
                unPlacedItems.Add(item);
                basket.Spawn(item);
                yield return Wait.ForSeconds(0.1f);
            }


        }

        private void Start()
        {
            _fridgeController.HideItems();
        }

        public void DestroyLevel()
        {
            Destroy(gameObject);
        }

        public void Initialize(int levelNumber)
        {
            allItems = new List<Item>();

            foreach (var containerParent in _fridgeController.GetParentsContainers())
            {
                for (int j = 0; j < containerParent.childCount; j++)
                {
                    var containerTransform = containerParent.GetChild(j).transform;
                    for (int i = 0; i < containerTransform.childCount; i++)
                    {
                        if (!containerTransform.GetChild(i).TryGetComponent(out Item item)) continue;
                        item.InitialItem();
                        allItems.Add(item);
                    }
                }
            }

            foreach (var itemParent in _fridgeController.GetItemParents())
            {
                for (int i = 0; i < itemParent.childCount; i++)
                {
                    
                    var item = itemParent.GetChild(i).GetComponent<Item>();
                    allItems.Add(item);
                }
            }
        }

        public void StartLevel()
        {
            _fridgeController.TryPassNextSection(basket, moveArea);

            StartCoroutine(InitItemAndContainers());
        }

        public bool TryPassNextSection()
        {
            if (!_fridgeController.TryPassNextSection(basket, moveArea))
            {
                basket.Move(basketStartPoint, 1.3f);
                return false;
            }

            StartCoroutine(InitItemAndContainers());
            return true;
        }

        public void OnLevelEnd()
        {
            foreach (var confetti in _confetti)
            {
                confetti.Play();
            }
        }

        public void RemoveItem(Item itemToRemove)
        {
            if (!unPlacedItems.Contains(itemToRemove))
            {
                // Debug.Log("Already unplaced Item");
                return;
            }

            unPlacedItems.Remove(itemToRemove);
            PlacedItems.Add(itemToRemove);

            if (unPlacedItems.Count == 0)
            {
                int score = containers.Count;
                foreach (var container in containers)
                {
                    if (container.items.Count == 0)
                        continue;

                    var itemId = container.items[0].id;
                    foreach (var item in container.items)
                    {
                        if (item.id != itemId)
                        {
                            score--;
                            break;
                        }
                    }
                }

                int starCount = Mathf.CeilToInt(((float) score / containers.Count) * 3);
                ActionManager.Instance.TriggerAction(ActionIDHolder.OnSectionObjectiveFinish,
                    new SectionObjectiveFinishData(starCount, PlacedItems.Count * 10));

                basket.Fireworks();
            }
        }

        public void AddItem(Item itemToRemove)
        {
            if (unPlacedItems.Contains(itemToRemove))
            {
                Debug.Log("Already placed Item");
                return;
            }

            unPlacedItems.Add(itemToRemove);

            PlacedItems.Remove(itemToRemove);
        }

        public void OnLevelFail()
        {
        }

        private Coroutine _resetItemCor;
        
        public void ResetItems()
        {
            if (PlacedItems.Count == 0)
            {
                return;
            }
            var item = PlacedItems[PlacedItems.Count - 1];

            item.transform.DOMove(item.transform.position + Vector3.up * 5f, 0.3f)
                .SetEase(Ease.Linear)
                .OnComplete(() => { item.ReturnToBasket(basket, 1f); });
                // .OnKill(()=>{ item.ReturnToBasket(basket, 1f);});
            
            unPlacedItems.Add(item);
            PlacedItems.Remove(item);
        }
        //
        // public void ResetItems()
        // {
        //     if (_resetItemCor != null)
        //     {
        //         return;
        //     }
        //     _resetItemCor = StartCoroutine(nameof(ResetItemCor));
        // }
        //
        // private IEnumerator ResetItemCor()
        // {
        //     foreach (var item in PlacedItems)
        //     {
        //         item.transform.DOMove(item.transform.position + Vector3.up * 5f, 0.3f)
        //             .SetEase(Ease.Linear)
        //             .OnComplete(() => { item.ReturnToBasket(basket, 1f); });
        //         unPlacedItems.Add(item);
        //         yield return Wait.ForSeconds(0.4f);
        //     }
        //
        //     PlacedItems = new List<Item>();
        //     _resetItemCor = null;
        // }

        public void HideItems()
        {
            foreach (var item in PlacedItems)
            {
                item.gameObject.SetActive(false);
            }


            foreach (var item in unPlacedItems)
            {
                item.gameObject.SetActive(false);
            }
        }

        public void EatItems()
        {
            StartCoroutine(nameof(ItemDestroyCor));
        }

        IEnumerator ItemDestroyCor()
        {
            while (allItems.Count > 0)
            {
                for (int i = 0; i < itemEatRate; i++)
                {
                    if (allItems.Count == 0)
                    {
                        break;
                    }
                    var item = allItems[Random.Range(0, allItems.Count)];
                    item.transform.DOScale(Vector3.zero, 0.8f);
                    item.transform.DOJump(Vector3.up * 24f, 1f, 1, 1f).OnComplete((() =>
                    {
                        item.gameObject.SetActive(false);
                    }));
                    allItems.Remove(item);
                }

                yield return Wait.ForSeconds(itemEatInterval);
            }
        }

        public void CloseFridge()
        {
            _fridgeController.CloseAll();
        }

        public BaseActionData GetData()
        {
            return new LevelData(allItems.Count);
        }
    }
}