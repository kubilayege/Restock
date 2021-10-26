using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MonoObjects;
using UnityEngine;

namespace Controllers
{
    public class FridgeController : MonoBehaviour
    {
        [SerializeField] private FridgeData fridge;
        
        private int _sectionIndex;
        private delegate NextSection NextSection(Basket basket, Transform moveArea);

        private NextSection _nextSectionFunc;

        private void Awake()
        {
            _nextSectionFunc = FreezeSection;
        }

        private void MoveDefaults(Basket basket, Transform moveArea)
        {
            PlayerCameraController.Instance.MoveTo(fridge.cameraParents[_sectionIndex]);
            basket.Move(fridge.basketParents[_sectionIndex], 1f);
            moveArea.position = fridge.moveAreaParents[_sectionIndex].position;
            fridge.containerParents[_sectionIndex].DOMove(fridge.containerOpen[_sectionIndex].position, fridge.openCloseDuration);
        }

        private void CloseLastSection()
        {
            fridge.containerParents[_sectionIndex-1].DOMove(fridge.containerClosed[_sectionIndex-1], fridge.openCloseDuration);
        }
        
        public bool TryPassNextSection(Basket basket, Transform moveArea)
        {
            if (_nextSectionFunc == null)
            {
                // fridge.freezerDrawer.DOMove(fridge.openFreezerDrawer.position, fridge.openCloseDuration);
                // fridge.freezer.DOMove(fridge.openFreezer.position, fridge.openCloseDuration);
                // fridge.containerParents[0].DOMove(fridge.containerOpen[0].position, fridge.openCloseDuration);
                fridge.containerParents[1].DOMove(fridge.containerOpen[1].position, fridge.openCloseDuration);
                fridge.containerParents[2].DOMove(fridge.containerOpen[2].position, fridge.openCloseDuration);

                fridge.doubleDrawerL.DOMove(fridge.openDoubleDrawerL.position, fridge.openCloseDuration);
                fridge.doubleDrawerR.DOMove(fridge.openDoubleDrawerR.position, fridge.openCloseDuration);
                
                fridge.centerDrawer.DOMove(fridge.openCenterDrawer.position, fridge.openCloseDuration);
                return false;
            };
            // Debug.Log("NextSection");
            PassSection(_nextSectionFunc, basket, moveArea);
            _sectionIndex++;
            return true;
        }

        private NextSection FreezeSection(Basket basket, Transform moveArea)
        {
            fridge.freezerDrawer.DOMove(fridge.openFreezerDrawer.position, fridge.openCloseDuration);
            fridge.freezer.DOMove(fridge.openFreezer.position, fridge.openCloseDuration);
            MoveDefaults(basket, moveArea);

            fridge.hingeL.DOLocalRotate(fridge.hingeRotation, fridge.openCloseDuration);
            fridge.hingeR.DOLocalRotate(-fridge.hingeRotation, fridge.openCloseDuration);
            
            return CenterDrawerSection;
        }


        private NextSection CenterDrawerSection(Basket basket, Transform moveArea)
        {
            fridge.freezerDrawer.DOMove(fridge.fridgeDefaultFreezerDrawer, fridge.openCloseDuration);
            fridge.freezer.DOMove(fridge.fridgeDefaultFreezer, fridge.openCloseDuration);
            CloseLastSection();

            MoveDefaults(basket, moveArea);
            fridge.centerDrawer.DOMove(fridge.openCenterDrawer.position, fridge.openCloseDuration);
            return DoubleDrawerSection;
        } 
        
        private NextSection DoubleDrawerSection(Basket basket, Transform moveArea)
        {
            fridge.centerDrawer.DOMove(fridge.fridgeDefaultCenterDrawer, fridge.openCloseDuration);
            CloseLastSection();
            
            MoveDefaults(basket, moveArea);
            fridge.doubleDrawerL.DOMove(fridge.openDoubleDrawerL.position, fridge.openCloseDuration);
            fridge.doubleDrawerR.DOMove(fridge.openDoubleDrawerR.position, fridge.openCloseDuration);
            
            return MiddleSection;
        } 
        
        private NextSection MiddleSection(Basket basket, Transform moveArea)
        {
            fridge.doubleDrawerL.DOMove(fridge.fridgeDefaultDoubleDrawerL, fridge.openCloseDuration);
            fridge.doubleDrawerR.DOMove(fridge.fridgeDefaultDoubleDrawerR, fridge.openCloseDuration);
            CloseLastSection();
            
            MoveDefaults(basket, moveArea);
            // fridge.midDrawer.DOMove(fridge.openMidDrawer.position, fridge.openCloseDuration);
            
            return null;
        } 
        
        private NextSection TopSection(Basket basket, Transform moveArea)
        {
            CloseLastSection();
            MoveDefaults(basket, moveArea);

            return null;
        } 

        private void PassSection(NextSection nextSection, Basket basket, Transform moveArea)
        {
            _nextSectionFunc = nextSection(basket, moveArea);
        }

        public Transform GetCurrentItemsParent()
        {
            return fridge.itemParents[_sectionIndex-1];
        }
        
        public Transform GetCurrentContainersParent()
        {
            return fridge.containerParents[_sectionIndex-1];
        }

        public void HideItems()
        {
            foreach (var itemParent in fridge.itemParents)
            {
                for (int i = 0; i < itemParent.childCount; i++)
                {
                    itemParent.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        public void CloseAll()
        {
            
        }

        public Transform[] GetParentsContainers()
        {
            return fridge.containerParents;
        }

        public Transform[] GetItemParents()
        {
            return fridge.itemParents;
        }
    }
}