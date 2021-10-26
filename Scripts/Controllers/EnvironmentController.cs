using System;
using System.Collections.Generic;
using Core;
using Core.ActionData;
using DG.Tweening;
using Managers;
using MonoObjects;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Controllers
{
    public class EnvironmentController : MonoBehaviour
    {
        [SerializeField] private List<Person> persons;
        [SerializeField] private Person _currentKitchenPerson;
        [SerializeField] private Bag[] bags;

        private List<Person> _personsInUse;

        [Header("Emotes")] 
        [SerializeField] private List<Sprite> sadEmotes;
        [SerializeField] private List<Sprite> happyEmotes;
        [SerializeField] private List<Sprite> calmEmotes;

        private Dictionary<EmoteType, List<Sprite>> _emotes;

        [Header("Durations")] [SerializeField] private float cameraToRoomViewDuration;
        [SerializeField] private float personsToFridgeDuration;
        [SerializeField] private float doorToFridgeDuration;
        [SerializeField] private float fridgeToKitchenDuration;
        [SerializeField] private float kitchenToDoorDuration;
        [SerializeField] private float personEatDuration;

        [Header("Points")] [SerializeField] private Transform cameraDoorView;
        [SerializeField] private Transform cameraFridgeView;
        [SerializeField] private Transform cameraRoomView;

        [SerializeField] private Transform doorPersonTransform;
        [SerializeField] private Path doorToFridgePath;
        [SerializeField] private Path fridgeToKitchenPath;
        [SerializeField] private Path kitchenToDoorPath;
        [SerializeField] private Path[] otherFridgePaths;


        [Header("Door Settings")] [SerializeField]
        private Transform doorTransform;

        [SerializeField] private Vector3 doorOpenAngle;
        [SerializeField] private float doorOpenDuration;
        [SerializeField] private float doorCloseDelayForLevelStart;
        [SerializeField] private float doorCloseDelayForLevelEnd;
        [SerializeField] private float doorCloseDuration;

        private Vector3 _doorCloseAngle;

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying) return;

            foreach (var person in persons)
            {
                if (person.handL == null)
                {
                    person.handL =
                        person.transform.Find(
                            "ROOT/TP/TP Pelvis/TP Spine/TP Spine1/TP Spine2/TP L Clavicle/TP L UpperArm/TP L Forearm/TP L Hand");
                }

                if (person.handR == null)
                {
                    person.handR =
                        person.transform.Find(
                            "ROOT/TP/TP Pelvis/TP Spine/TP Spine1/TP Spine2/TP R Clavicle/TP R UpperArm/TP R Forearm/TP R Hand");
                }
            }

#endif
        }

        private void Awake()
        {
            _personsInUse = new List<Person>();
            InitEmoteLookUp();
            _doorCloseAngle = doorTransform.rotation.eulerAngles;

            ActionManager.Instance.AddAction(ActionIDHolder.OnTapToPlay, OnTapToPlay);
            ActionManager.Instance.AddAction(ActionIDHolder.OnLevelCompleted, OnLevelCompleted);
            ActionManager.Instance.AddAction(ActionIDHolder.OnEndingStarted, OnEndingStarted);
            ActionManager.Instance.AddAction(ActionIDHolder.PrepareLevelID, PrepareLevel);
        }

        private void InitEmoteLookUp()
        {
            _emotes = new Dictionary<EmoteType, List<Sprite>>
            {
                {EmoteType.Sad, sadEmotes}, {EmoteType.Happy, happyEmotes},
                {EmoteType.Calm, calmEmotes}
            };
        }

        private void PrepareLevel(BaseActionData data)
        {
            // PlayerCameraController.Instance.MoveTo(cameraDoorView,0.5f);
        }

        private void OnEndingStarted(BaseActionData data)
        {
            _currentKitchenPerson.MoveAlong(fridgeToKitchenPath, fridgeToKitchenDuration, BringPeopleIn,
                GetRandomEmote(EmoteType.Calm));
            _currentKitchenPerson.WalkSlow();
            PlayerCameraController.Instance.MoveTo(cameraRoomView, cameraToRoomViewDuration);
        }

        private Sprite GetRandomEmote(EmoteType emoteType)
        {
            return _emotes[emoteType][Random.Range(0, _emotes[emoteType].Count)];
        }

        private void OnLevelCompleted(BaseActionData data)
        {
        }

        private void OnTapToPlay(BaseActionData data)
        {
            _currentKitchenPerson.gameObject.SetActive(true);
            _currentKitchenPerson.MoveAlong(doorToFridgePath, doorToFridgeDuration, StartLevel,
                GetRandomEmote(EmoteType.Calm), bags);
            _currentKitchenPerson.WalkSlow();
            PlayerCameraController.Instance.LookAt(_currentKitchenPerson.transform, 0f);

            doorTransform.DOLocalRotate(doorOpenAngle, doorOpenDuration).OnComplete(() =>
            {
                doorTransform.DOLocalRotate(_doorCloseAngle, doorCloseDuration)
                    .SetDelay(doorCloseDelayForLevelStart);
            });
        }

        private void StartLevel()
        {
            _currentKitchenPerson.Wait();
            foreach (var bag in bags)
            {
                bag.DropItems();
            }

            ActionManager.Instance.TriggerAction(ActionIDHolder.OnLevelStartedID);
        }

        private void FinishLevel()
        {
            int i = 0;
            _currentKitchenPerson.React();
            _currentKitchenPerson.Emote(GetRandomEmote(EmoteType.Sad));
            foreach (var person in _personsInUse)
            {
                person.Return(otherFridgePaths[i++], true, personsToFridgeDuration, TakePersonBack,
                    GetRandomEmote(EmoteType.Happy), personEatDuration);
            }
        }

        private void TakePersonBack(Person person)
        {
            person.gameObject.SetActive(false);
            persons.Add(person);
            _personsInUse.Remove(person);

            if (_personsInUse.Count != 0) return;

            doorTransform.DOLocalRotate(doorOpenAngle, doorOpenDuration).OnComplete(() =>
            {
                doorTransform.DOLocalRotate(_doorCloseAngle, doorCloseDuration).SetDelay(doorCloseDelayForLevelEnd)
                    .OnComplete(
                        () =>
                        {
                            ActionManager.Instance.TriggerAction(ActionIDHolder.OnLevelCompleted);
                            ActionManager.Instance.TriggerAction(ActionIDHolder.PrepareLevelID);
                        });
            });


            LevelManager.Instance.CloseFridge();

            _currentKitchenPerson.ReturnKitchenPerson(kitchenToDoorPath, false, kitchenToDoorDuration,
                TakeKitchenPersonBack, GetRandomEmote(EmoteType.Sad));

            _currentKitchenPerson.WalkFast();
            PlayerCameraController.Instance.LookAt(_currentKitchenPerson.transform, 0.2f);
        }

        private void TakeKitchenPersonBack(Person person)
        {
            PlayerCameraController.Instance.MoveTo(cameraDoorView, 0.2f);
            person.gameObject.SetActive(false);
            persons.Add(person);
        }

        private void BringPeopleIn()
        {
            var index = Random.Range(0, persons.Count);
            _currentKitchenPerson.Wait();
            persons[index].gameObject.SetActive(true);
            
            foreach (var person in _personsInUse)
            {
                person.Emote(GetRandomEmote(EmoteType.Happy));
            }
            
            _personsInUse.Add(persons[index].MoveAlong(otherFridgePaths[0], personsToFridgeDuration, LetPeopleEat));
            persons[index].WalkFast();
            for (int i = 1; i < otherFridgePaths.Length; i++)
            {
                index = Random.Range(0, persons.Count);
                persons[index].gameObject.SetActive(true);
                _personsInUse.Add(persons[index].MoveAlong(otherFridgePaths[i], personsToFridgeDuration, null));
                persons[index].WalkFast();
            }
        }

        private void LetPeopleEat()
        {
            foreach (var person in _personsInUse)
            {
                person.EatFromFridge();
                person.Emote(GetRandomEmote(EmoteType.Happy));
            }

            LevelManager.Instance.EatItems();
            
            FinishLevel();
        }
    }
}