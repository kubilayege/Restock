using System;
using Controllers;
using Core;
using Core.ActionData;
using DG.Tweening;
using MonoObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Managers
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [SerializeField] private GameObject TapToPlayPanel;
        [SerializeField] private GameObject GamePanel;
        [SerializeField] private GameObject SectionCompletePanel;
        [SerializeField] private GameObject LevelCompletePanel;
        [SerializeField] private GameObject LevelFailedPanel;
        [SerializeField] private PopUpCanvas popUpStarCanvas;
        [SerializeField] private Vector3 offsetForPopUp;
        [SerializeField] private ProgressStageObject _progressStageObject;
        [SerializeField] private Toggle musicToggle;
        [SerializeField] private AudioClip spendSound;
        [SerializeField] private AudioClip earnSound;
        [SerializeField] private AudioClipSettings spendSettings;
        [SerializeField] private AudioClipSettings earnSettings;
        
        

        [SerializeField] private TextMeshProUGUI totalScore;
        [SerializeField] private Image totalScoreIcon;

        [SerializeField] private TextMeshProUGUI earnedScore;
        [SerializeField] private TextMeshProUGUI expanseScore;
        [SerializeField] private Transform scoreCenterPivot;
        [SerializeField] private TextMeshProUGUI[] levelInfoTexts;
        [SerializeField] private TweenData panelAnimationData;

        [SerializeField] private EventController inGameInputController;


        private int currentLevelScore;
        private int currentPlayerScore;

        private int PlayerTotalScore
        {
            get
            {
                if (!PlayerPrefs.HasKey("Score"))
                    PlayerPrefs.SetInt("Score", 1000);
                return PlayerPrefs.GetInt("Score");
            }
            set => PlayerPrefs.SetInt("Score", value);
        }

        private void Awake()
        {
            ActionManager.Instance.AddAction(ActionIDHolder.PrepareLevelID, OnPrepareLevel);
            ActionManager.Instance.AddAction(ActionIDHolder.PassNextSection, OnLevelPrepared);
            ActionManager.Instance.AddAction(ActionIDHolder.OnTapToPlay, OnTapToPlay);
            ActionManager.Instance.AddAction(ActionIDHolder.OnLevelCompleted, OnLevelComplete);
            ActionManager.Instance.AddAction(ActionIDHolder.OnLevelFailedID, OnLevelFailed);
            ActionManager.Instance.AddAction(ActionIDHolder.OnSectionObjectiveFinish, OnSectionCompleted);
            ActionManager.Instance.AddAction(ActionIDHolder.PassNextSection, OnLevelStarted);
            ActionManager.Instance.AddAction(ActionIDHolder.OnLevelStartedID, OnLevelStarted);
            ActionManager.Instance.AddAction(ActionIDHolder.OnLevelStartedID, OnLevelPrepared);

            if (!PlayerPrefs.HasKey("Music"))
            {
                PlayerPrefs.SetInt("Music", 1);
            }

            musicToggle.isOn = PlayerPrefs.GetInt("Music") != 1;
        }

        private void OnLevelStarted(BaseActionData data)
        {
            TapToPlayPanel.SetActive(false);
            GamePanel.SetActive(true);
            LevelCompletePanel.SetActive(false);
            LevelFailedPanel.SetActive(false);
            SectionCompletePanel.SetActive(false);

            _progressStageObject.NextStage();

        }

        private void UpdateScoreText(int value)
        {
            totalScore.text = value--.ToString();
        }

        private void OnLevelPrepared(BaseActionData data)
        {
            foreach (var levelInfoText in levelInfoTexts)
            {
                levelInfoText.text = $"LEVEL {LevelManager.Instance.CurrentLevelNumber}";
            }

        }

        private void OnLevelFailed(BaseActionData data)
        {
            TapToPlayPanel.SetActive(false);
            GamePanel.SetActive(false);
            LevelCompletePanel.SetActive(false);
            LevelFailedPanel.SetActive(true);
            SectionCompletePanel.SetActive(false);

            DoPanelTransition(LevelFailedPanel.transform);
        }


        private void OnTapToPlay(BaseActionData data)
        {
            TapToPlayPanel.SetActive(false);
            GamePanel.SetActive(false);
            LevelCompletePanel.SetActive(false);
            LevelFailedPanel.SetActive(false);
            SectionCompletePanel.SetActive(false);

            // DoPanelTransition(GamePanel.transform);


            PlayerInputController.Instance.SetInputPanel(inGameInputController);
            
            
            expanseScore.transform.localScale = Vector3.zero;
            expanseScore.transform.position = totalScore.transform.position;
            var levelData = (LevelData) data;
            totalScore.DOColor(expanseScore.color, 0.5f)
                .OnStart(() =>
                {
                    expanseScore.gameObject.SetActive(true);
                    expanseScore.transform.DOScale(1f, 0.7f);
                    expanseScore.text = $"-{levelData.TotalItemCount * 5f}";
                    currentPlayerScore = PlayerTotalScore;
                    int targetScore = currentPlayerScore - levelData.TotalItemCount * 5;
                    DOTween.To(() => currentPlayerScore, UpdateScoreText, targetScore, 2f).OnComplete(() =>
                    {
                        currentLevelScore = targetScore;
                    });
                })
                .OnComplete(() =>
                {
                    expanseScore.transform.DOMove(scoreCenterPivot.position, 1f).OnComplete(() =>
                    {
                        ObjectPoolManager.Instance.GetFromPool<AudioSourceController>().PlayOnce(spendSound,spendSettings);
                        expanseScore.transform.DOPunchScale(Vector3.one / 4f, 0.3f).OnComplete(() =>
                        {
                            expanseScore.transform.DOScale(Vector3.zero, 1f).OnComplete((() =>
                            {
                                expanseScore.gameObject.SetActive(false);
                            }));

                            totalScore.DOColor(Color.white, 0.5f);
                        });
                    });
                });
        }

        private void OnLevelComplete(BaseActionData data)
        {
            TapToPlayPanel.SetActive(false);
            GamePanel.SetActive(false);
            LevelCompletePanel.SetActive(true);
            LevelFailedPanel.SetActive(false);
            SectionCompletePanel.SetActive(false);


            PlayerTotalScore = currentPlayerScore;

            DoPanelTransition(LevelCompletePanel.transform);
        }

        private void OnPrepareLevel(BaseActionData data)
        {
            currentPlayerScore = PlayerTotalScore;
            currentLevelScore = 0;
            AddScore(0);

            TapToPlayPanel.SetActive(true);
            GamePanel.SetActive(false);
            LevelCompletePanel.SetActive(false);
            LevelFailedPanel.SetActive(false);
            SectionCompletePanel.SetActive(false);

            
            totalScore.text = currentPlayerScore.ToString();
            // DoPanelTransition(TapToPlayPanel.transform);
        }

        private void OnSectionCompleted(BaseActionData data)
        {
            TapToPlayPanel.SetActive(false);
            GamePanel.SetActive(false);
            LevelCompletePanel.SetActive(false);
            LevelFailedPanel.SetActive(false);
            SectionCompletePanel.SetActive(true);

            if (data != null)
            {
                var sectionFinishData = ((SectionObjectiveFinishData) data);
                popUpStarCanvas.ActivateStars(sectionFinishData.star);
                earnedScore.gameObject.SetActive(true);
                earnedScore.text = sectionFinishData.money.ToString();
                earnedScore.transform.position = scoreCenterPivot.position;
                earnedScore.transform.localScale = Vector3.zero;
                currentLevelScore += sectionFinishData.money;

                DOTween.Kill(earnedScore.GetInstanceID(), true);
                earnedScore.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutElastic).SetDelay(1f)
                    .OnStart((() =>
                    {
                        ObjectPoolManager.Instance.GetFromPool<AudioSourceController>().PlayOnce(earnSound,earnSettings);

                    }))
                    .OnComplete(() =>
                {

                    earnedScore.transform.DOScale(Vector3.one / 2f, 0.8f)
                        .SetId(earnedScore.GetInstanceID());
                    earnedScore.transform.DOMove(totalScore.transform.position, 0.7f)
                        .SetEase(Ease.Linear)
                        .SetId(earnedScore.GetInstanceID())
                        .OnComplete(
                            (() =>
                            {
                                ObjectPoolManager.Instance.GetFromPool<AudioSourceController>().PlayOnce(spendSound,spendSettings);

                                earnedScore.gameObject.SetActive(false);
                            })
                        );

                    totalScore.DOColor(earnedScore.color, 0.6f).SetLoops(1, LoopType.Yoyo)
                        .SetId(earnedScore.GetInstanceID())
                        .SetDelay(1f)
                        .OnStart(() =>
                        {
                            currentPlayerScore = currentLevelScore;
                            totalScore.text = currentPlayerScore.ToString();
                            totalScoreIcon.transform.DOPunchScale(Vector3.one / 2f, 0.9f)
                                .SetId(earnedScore.GetInstanceID())
                                .OnComplete((() => { }));
                        })
                        .OnComplete((() =>
                        {
                            totalScore.color = Color.white;
                            totalScoreIcon.transform.localScale = Vector3.one;
                        }));
                }).SetId(earnedScore.GetInstanceID());
            }

            PlayerCameraController.Instance.PopUpStar(popUpStarCanvas.transform, offsetForPopUp);
        }

        public void AddScore(int addAmount)
        {
            currentLevelScore += addAmount;

            // totalScore.text = $"${currentPlayerScore + currentLevelScore}";
            earnedScore.text = $"Earned: ${currentLevelScore}";
        }

        public void TapToPlay()
        {
            ActionManager.Instance.TriggerAction(ActionIDHolder.OnTapToPlay, LevelManager.Instance.GetCurrentLevelData());
        }

        public void Continue()
        {
            ActionManager.Instance.TriggerAction(ActionIDHolder.PrepareLevelID);
        }

        public void PassLevel()
        {
            LevelManager.Instance.HideLastItems();
            ActionManager.Instance.TriggerAction(ActionIDHolder.OnSectionObjectiveFinish);
        }

        public void PassSection()
        {
            ActionManager.Instance.TriggerAction(ActionIDHolder.PassNextSection);
        }

        public void FailLevel()
        {
            LevelManager.Instance.ResetItems();
        }

        public void ToggleMusic(bool value)
        {
            PlayerPrefs.SetInt("Music", musicToggle.isOn ? 0 : 1);
            LevelManager.Instance.ToggleMusic();
        }

        private void DoPanelTransition(Transform panel)
        {
            DOTween.Kill("PanelTween");
            panel.gameObject.SetActive(true);
            panel.localScale = Vector3.zero;
            panel.DOScale(Vector3.one, panelAnimationData.duration).SetEase(panelAnimationData.ease).OnKill(() =>
            {
                panel.localScale = Vector3.one;
            }).SetId("PanelTween");
        }

        public int GetPlayerScore()
        {
            return currentLevelScore;
        }
    }
}