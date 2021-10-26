using Core;
using System.Collections.Generic;
using Core.ActionData;
using UnityEngine;

namespace Managers
{
    [AddComponentMenu("Managers/Action Manager")]
    public class ActionManager : MonoSingleton<ActionManager>
    {
        #region Properties

        private Dictionary<int, ListenerObject> _actionListeners = new Dictionary<int, ListenerObject>();
        public delegate void ActionDelegateWithData(BaseActionData data = null);

        private static int _lastTriggerIndex = -1;

        #endregion

        #region Functions
        
        public void TriggerAction(int action, BaseActionData data = null)
        {
            if (_actionListeners.ContainsKey(action))
            {
                _actionListeners[action].ProcessDelegates(data);
            }
        }

        // Add new action to listener
        public void AddAction(int action, ActionDelegateWithData newAction)
        {
            if (_actionListeners.ContainsKey(action))
            {
                _actionListeners[action].AddListener(newAction);
            }
            else
            {
                _actionListeners[action] = new ListenerObject();
                _actionListeners[action].AddListener(newAction);
            }
        }

        //Remove one action from list
        public void RemoveListener(int triggerName, ActionDelegateWithData processToRemove)
        {
            if (_actionListeners.TryGetValue(triggerName, out ListenerObject temp))
            {
                temp.RemoveListener(processToRemove);
            }
        }

        //Get new trigger index for actions
        public static int GetTriggerIndex()
        {
            return ++_lastTriggerIndex;
        }

        public class ListenerObject
        {
            ActionDelegateWithData _thisActionDelegateWithData;

            public void ProcessDelegates(BaseActionData data)
            {
                _thisActionDelegateWithData?.Invoke(data);
            }
            
            public void AddListener(ActionDelegateWithData param)
            {
                _thisActionDelegateWithData += param;
            }
            
            public void RemoveListener(ActionDelegateWithData param)
            {
                _thisActionDelegateWithData -= param;
            }
        }

        // Delete all actions
        public void ClearListeners() { _actionListeners.Clear(); }

        #endregion
    }
}
