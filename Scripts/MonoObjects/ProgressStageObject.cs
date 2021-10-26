using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonoObjects
{
    public class ProgressStageObject : MonoBehaviour
    {
        [SerializeField] private List<StageComponents> stageComponents;
        private int _currentStageIndex;

        
        
        
        public void NextStage()
        {
            if (_currentStageIndex >= stageComponents.Count)
            {
                _currentStageIndex = 0;
                return;
            }
            
            foreach (var element in stageComponents[_currentStageIndex].stageElementsToClose)
            {
                element.SetActive(false);
            }
            
            foreach (var element in stageComponents[_currentStageIndex].stageElementsToOpen)
            {
                element.SetActive(true);
            }

            _currentStageIndex++;
        }
        
    }

    [Serializable]
    public class StageComponents
    {
        public List<GameObject> stageElementsToOpen;
        public List<GameObject> stageElementsToClose;
    }
}