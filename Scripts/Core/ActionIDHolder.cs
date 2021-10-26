using Managers;

namespace Core
{
    public static class ActionIDHolder
    {
        // Level failed event ID
        private static int _onLevelFailedID = ActionManager.GetTriggerIndex();
        public static int OnLevelFailedID { get { return _onLevelFailedID; } }

        // Level finish event ID
        private static int _onLevelCompleted = ActionManager.GetTriggerIndex();
        public static int OnLevelCompleted { get { return _onLevelCompleted; } }

        // Level completed event ID
        private static int _onSectionObjectiveFinish = ActionManager.GetTriggerIndex();
        public static int OnSectionObjectiveFinish { get { return _onSectionObjectiveFinish; } }
        
        
        // Level completed event ID
        private static int _passNextSection = ActionManager.GetTriggerIndex();
        public static int PassNextSection { get { return _passNextSection; } }
        
        // Level is prepared event ID
        private static int _onLevelPreparedID = ActionManager.GetTriggerIndex();
        public static int OnLevelPreparedID { get { return _onLevelPreparedID; } }
    
        // Level completed event ID
        private static int _onLevelStartedID = ActionManager.GetTriggerIndex();
        public static int OnLevelStartedID { get { return _onLevelStartedID; } }
        
        
        // Level completed event ID
        private static int _onTapToPlay = ActionManager.GetTriggerIndex();
        public static int OnTapToPlay { get { return _onTapToPlay; } }
        
        
        // Level completed event ID
        private static int _onEndingStarted = ActionManager.GetTriggerIndex();
        public static int OnEndingStarted { get { return _onEndingStarted; } }

        
        // Level prepare event ID
        private static int _prepareLevelID = ActionManager.GetTriggerIndex();
        public static int PrepareLevelID { get { return _prepareLevelID; } }

        // Coin Pickup
        private static int _itemHold = ActionManager.GetTriggerIndex();
        public static int ItemHold { get { return _itemHold; } }
        
        
        // Coin Pickup
        private static int _itemDrop = ActionManager.GetTriggerIndex();
        public static int ItemDrop { get { return _itemDrop; } }

    }
}
