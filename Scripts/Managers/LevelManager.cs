using Core;
using Core.ActionData;
using ScriptableObjects;
using UnityEngine;
using MonoObjects;

namespace Managers
{
    public class LevelManager : MonoSingleton<LevelManager> 
    {
        [SerializeField] private LevelPool levelPool;
        [SerializeField] private AudioClip[] levelMusics;
        [SerializeField] private AudioClipSettings levelMusicSettings;
        
        
        private AudioSourceController _mainAudioSourceController;
        
        public int CurrentLevelNumber
        {
            get
            {
                if (PlayerPrefs.GetInt("LevelNumber") <= 0)
                    PlayerPrefs.SetInt("LevelNumber", 1);
                return PlayerPrefs.GetInt("LevelNumber");
            }
            private set => PlayerPrefs.SetInt("LevelNumber", value);
        }

        public int CurrentLevelIndex
        {
            get
            {
                if (PlayerPrefs.GetInt("LevelIndex") < 0)
                    PlayerPrefs.SetInt("LevelIndex", 0);
                return PlayerPrefs.GetInt("LevelIndex");
            }
            private set
            {
                PlayerPrefs.SetInt("LevelIndex", value % levelPool.Length);
            }
        }

        private Level _currentLevel;
        
        private void Awake()
        {
            Application.targetFrameRate = 60;

            ActionManager.Instance.AddAction(ActionIDHolder.OnLevelCompleted, LevelCompleted);
            ActionManager.Instance.AddAction(ActionIDHolder.PrepareLevelID, PrepareLevel);
            ActionManager.Instance.AddAction(ActionIDHolder.OnLevelPreparedID, LevelInit);
            ActionManager.Instance.AddAction(ActionIDHolder.OnLevelStartedID, StartLevel);
            ActionManager.Instance.AddAction(ActionIDHolder.OnLevelFailedID, FailLevel);
            ActionManager.Instance.AddAction(ActionIDHolder.OnSectionObjectiveFinish, FinishLevel);
            ActionManager.Instance.AddAction(ActionIDHolder.PassNextSection, NextSection);
            ActionManager.Instance.AddAction(ActionIDHolder.OnTapToPlay, OnTapToPlay);
            
        }

        private void LevelCompleted(BaseActionData data)
        {
            CurrentLevelIndex++;
        }

        private void OnTapToPlay(BaseActionData data)
        {
            _mainAudioSourceController.PlayLoop(levelMusics[Random.Range(0, levelMusics.Length)], levelMusicSettings);
        }

        private void Start()
        {
            _mainAudioSourceController = ObjectPoolManager.Instance.GetFromPool<AudioSourceController>();
        }

        private void NextSection(BaseActionData data)
        {
            if (_currentLevel.TryPassNextSection())
            {
                Debug.Log("Level Started");
                TinySauce.OnGameStarted(CurrentLevelNumber.ToString());
                return;
            }
            
            ActionManager.Instance.TriggerAction(ActionIDHolder.OnEndingStarted);
        }

        private void PrepareLevel(BaseActionData data)
        {
            if (_currentLevel != null)
            {
                _currentLevel.DestroyLevel();
            }

            // Debug.Log(CurrentLevelIndex);
            _currentLevel = Instantiate(levelPool.GetLevel(CurrentLevelIndex));

            ActionManager.Instance.TriggerAction(ActionIDHolder.OnLevelPreparedID);
        }

        private void LevelInit(BaseActionData data)
        {
            _currentLevel.Initialize(CurrentLevelNumber);
        }
        
        private void StartLevel(BaseActionData data)
        {
            _currentLevel.StartLevel();                
            TinySauce.OnGameStarted(CurrentLevelNumber.ToString());
        }
        
        private void FailLevel(BaseActionData data)
        {
            _currentLevel.OnLevelFail();
            Debug.Log("Level Failed");
            TinySauce.OnGameFinished(false, 0f, CurrentLevelNumber.ToString());
        }
        
        private void FinishLevel(BaseActionData data)
        {
            CurrentLevelNumber += 1;
            _currentLevel.OnLevelEnd();
            Debug.Log("Level Succeed");
            TinySauce.OnGameFinished(true, UIManager.Instance.GetPlayerScore(), CurrentLevelNumber.ToString());
        }


        public Transform GetCurrentLevelParent()
        {
            return _currentLevel.transform;
        }

        public void ResetItems()
        {
            _currentLevel.ResetItems();
        }

        public void ItemPlaced(Item currentHoldingItem)
        {
            _currentLevel.RemoveItem(currentHoldingItem);
        }
        
        public void ItemRemoved(Item currentHoldingItem)
        {
            _currentLevel.AddItem(currentHoldingItem);
        }

        public void HideLastItems()
        {
            _currentLevel.HideItems();
        }

        public void ToggleMusic()
        {
            _mainAudioSourceController?.ToggleMusic();
        }
        
        public void EatItems()
        {
            _currentLevel.EatItems();
        }

        public void CloseFridge()
        {
            _currentLevel.CloseFridge();
        }

        public BaseActionData GetCurrentLevelData()
        {
            return _currentLevel.GetData();
        }
    }
}
