using System;
using UnityEngine;

public enum FridgeSection
{
    Freezer,
    Center,
    Double
}

namespace MonoObjects
{
    public class FridgeData : MonoBehaviour
    {
        public float openCloseDuration;
        public Vector3 hingeRotation;
        
        [Header("Fridge Pivots")]
        public Transform freezer;
        public Transform freezerDrawer;
        public Transform centerDrawer;
        public Transform doubleDrawerL;
        public Transform doubleDrawerR;
        // public Transform midDrawer;
        public Transform hingeL;
        public Transform hingeR;
        
        [Header("Fridge Open Positions")]
        public Transform openFreezer;
        public Transform openFreezerDrawer;
        public Transform openCenterDrawer;
        public Transform openDoubleDrawerL;
        public Transform openDoubleDrawerR;
        // public Transform openMidDrawer;

        [HideInInspector] public Vector3 fridgeDefaultFreezer;
        [HideInInspector] public Vector3 fridgeDefaultFreezerDrawer;
        [HideInInspector] public Vector3 fridgeDefaultCenterDrawer;
        [HideInInspector] public Vector3 fridgeDefaultDoubleDrawerL;
        [HideInInspector] public Vector3 fridgeDefaultDoubleDrawerR;
        // [HideInInspector] public Vector3 fridgeDefaultMidDrawer;
        
        public Transform[] cameraParents;

        public Transform[] basketParents;
        
        public Transform[] containerParents;
        
        public Transform[] containerOpen;
        public Vector3[] containerClosed;
        
        public Transform[] moveAreaParents;
        
        public Transform[] itemParents;
        
        private void Awake()
        {
            fridgeDefaultFreezer = freezer.position;
            fridgeDefaultFreezerDrawer = freezerDrawer.position;
            fridgeDefaultCenterDrawer = centerDrawer.position;
            fridgeDefaultDoubleDrawerL = doubleDrawerL.position;
            fridgeDefaultDoubleDrawerR = doubleDrawerR.position;
            // fridgeDefaultMidDrawer = midDrawer.position;

            containerClosed = new Vector3[containerOpen.Length];

            for (int i = 0; i < containerOpen.Length; i++)
            {
                containerClosed[i] = containerParents[i].position;
            }
        }

        public void Open(FridgeSection fridgeSection)
        {
            if( containerClosed.Length != containerOpen.Length ){
                containerClosed = new Vector3[containerOpen.Length];

                for (int i = 0; i < containerOpen.Length; i++)
                {
                    containerClosed[i] = containerParents[i].position;
                }
                
                
                fridgeDefaultFreezer = freezer.position;
                fridgeDefaultFreezerDrawer = freezerDrawer.position;
                fridgeDefaultCenterDrawer = centerDrawer.position;
                fridgeDefaultDoubleDrawerL = doubleDrawerL.position;
                fridgeDefaultDoubleDrawerR = doubleDrawerR.position;
                // fridgeDefaultMidDrawer = midDrawer.position;
            }
            
            
            CloseAll();
            
            switch (fridgeSection)
            {
                case FridgeSection.Freezer:
                    containerParents[(int) fridgeSection].position = containerOpen[(int) fridgeSection].position;
                    freezer.position = openFreezer.position;
                    freezerDrawer.position = openFreezerDrawer.position;
                    break;
                case FridgeSection.Center:
                    containerParents[(int) fridgeSection].position = containerOpen[(int) fridgeSection].position;
                    centerDrawer.position = openCenterDrawer.position;
                    hingeL.localRotation = Quaternion.Euler(hingeRotation);
                    hingeR.localRotation = Quaternion.Euler(-hingeRotation);
                    break;
                case FridgeSection.Double:
                    containerParents[(int) fridgeSection].position = containerOpen[(int) fridgeSection].position;
                    doubleDrawerL.position = openDoubleDrawerL.position;
                    doubleDrawerR.position = openDoubleDrawerR.position;
                    
                    hingeL.localRotation = Quaternion.Euler(hingeRotation);
                    hingeR.localRotation = Quaternion.Euler(-hingeRotation);
                    break;
                default:
                    break;
            }
        }
        
        public void CloseAll()
        {
            freezer.position = fridgeDefaultFreezer;
            freezerDrawer.position = fridgeDefaultFreezerDrawer;
            centerDrawer.position = fridgeDefaultCenterDrawer;
            doubleDrawerL.position = fridgeDefaultDoubleDrawerL;
            doubleDrawerR.position = fridgeDefaultDoubleDrawerR;
            // midDrawer.position = fridgeDefaultMidDrawer;
            hingeL.localRotation = Quaternion.identity;
            hingeR.localRotation = Quaternion.identity;

            for (int i = 0; i < containerOpen.Length; i++)
            {
                containerParents[i].position = containerClosed[i];
            }
        }
    }
}