using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;
using NUnit.Framework;
using Unity.Mathematics;

namespace UI.Views
{
    public class HomeworkView : View
    {
        [SerializeField] Homework homeworkObject;
        [SerializeField] GameObject homeworkPrefab;
        [SerializeField] GameObject content;
        [SerializeField] GameObject DividerPrefab;

        [SerializeField] private CustomHomework _CustomHomework;
        [SerializeField] private ScrollRect _ScrollRect;
        private RectTransform _rectTransformDefault;

        [SerializeField] private Button _AddHomeworkButton;
        [SerializeField] private Button _recenterButton;
        [SerializeField] private Toggle _FilterButton;

        private List<DateTime> _Dates = new List<DateTime>();
        private List<GameObject> _homeworkDateDividers = new List<GameObject>();

        public override void Initialize()
        {
            openNavigationButton.onClick.AddListener(() =>
            {
                openNavigationButton.enabled = false;
                ViewManager.Instance.ShowNavigation();
            });

            closeButtonWholePage.onClick.AddListener(() =>
            {
                openNavigationButton.enabled = true;
                ViewManager.Instance.HideNavigation();
            });

            _AddHomeworkButton.onClick.AddListener(() => ViewManager.Instance.ShowNewView<AddHomeworkView>());
            _recenterButton.onClick.AddListener(() => Recenter());
            ViewManager.onInitializeComplete += ctx =>
            {
                float decelerationRate = _ScrollRect.decelerationRate;
                _ScrollRect.decelerationRate = 0f;
                _ScrollRect.content.DOLocalMove(_ScrollRect.GetSnapToPositionToBringChildIntoView(_rectTransformDefault), 0.1f, true).onComplete += () => _ScrollRect.decelerationRate = decelerationRate;
            };
            
            _FilterButton.onValueChanged.AddListener((ctx) =>
            {
                RectTransform rectTransform = _ScrollRect.GetComponent<RectTransform>();
                rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, 20f);
                StartCoroutine(lerpTo(rectTransform, ctx ? -185f : -40f, 0.5f));
            });

            List<Homework.Item> homework = homeworkObject.getHomework();

            if (homework == null)
            {
                base.Initialize();
                return;
            }

            int day = 0;

            foreach (Homework.Item HomeworkItem in homework)
            {
                if (HomeworkItem.datumTijd.Day > day)
                {
                    var go = Instantiate(DividerPrefab, content.transform);
                    go.GetComponent<homeworkDivider>().Datum.text =
                        ((DateTimeOffset) HomeworkItem.datumTijd).DateTime.ToString("d MMMM");
                    _Dates.Add(((DateTimeOffset) HomeworkItem.datumTijd).DateTime.Date);
                    _homeworkDateDividers.Add(go);
                }

                if (HomeworkItem.datumTijd.Day < day)
                {
                    var go = Instantiate(DividerPrefab, content.transform);
                    go.GetComponent<homeworkDivider>().Datum.text =
                        ((DateTimeOffset) HomeworkItem.datumTijd).DateTime.ToString("d MMMM");
                    _Dates.Add(((DateTimeOffset) HomeworkItem.datumTijd).DateTime.Date);
                    _homeworkDateDividers.Add(go);
                }

                var homeworkItem = Instantiate(homeworkPrefab, content.transform);

                string onderwerp = HomeworkItem.studiewijzerItem.onderwerp ?? "";

                if (onderwerp == "" || onderwerp.Length == 0)
                    onderwerp = HomeworkItem.studiewijzerItem.omschrijving;

                string vak;
                try
                {
                    vak = HomeworkItem.lesgroep.vak.naam;
                }
                catch (Exception)
                {
                    vak = "error";
                }

                homeworkItem.GetComponent<HomeworkInfo>().SetHomeworkInfo(vak, onderwerp, HomeworkItem.additionalObjects.swigemaaktVinkjes?.items[0].gemaakt ?? false, HomeworkItem);

                homeworkItem.GetComponent<Button>().onClick.AddListener(() =>
                {
                    ViewManager.Instance.ShowNewView<HomeworkItemView>(homeworkItem.GetComponent<HomeworkInfo>().homeworkInfo);
                });

                homeworkItem.GetComponent<HomeworkInfo>().gemaakt.onValueChanged.AddListener((isOn) =>
                {
                    UpdateGemaaktStatus((long) HomeworkItem.links[0].id, isOn);
                });
                
                day = HomeworkItem.datumTijd.Day;
            }
            
            int closestTimeIndex = _Dates.IndexOf(_Dates.OrderBy(t => Math.Abs((t - TimeManager.Instance.CurrentDateTime).Ticks)).First());
            _ScrollRect.content.position = _ScrollRect.GetSnapToPositionToBringChildIntoView(_rectTransformDefault = _homeworkDateDividers[closestTimeIndex].GetComponent<RectTransform>());

            #region Sorting Shit
            huiswerk.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                    _HomeworkTypes |= HomeworkTypes.huiswerk;
                else
                    _HomeworkTypes &= ~HomeworkTypes.huiswerk;
            });
            
            toets.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                    _HomeworkTypes |= HomeworkTypes.toets;
                else
                    _HomeworkTypes &= ~HomeworkTypes.toets;
            });
            
            grote_toets.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                    _HomeworkTypes |= HomeworkTypes.grote_toets;
                else
                    _HomeworkTypes &= ~HomeworkTypes.grote_toets;
            });
            
            gemaakt.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                    _HomeworkStatus |= HomeworkStatus.gemaakt;
                else
                    _HomeworkStatus &= ~HomeworkStatus.gemaakt;
            });
            
            niet_gemaakt.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                    _HomeworkStatus |= HomeworkStatus.niet_gemaakt;
                else
                    _HomeworkStatus &= ~HomeworkStatus.niet_gemaakt;
            });
            sorteerButton.onClick.AddListener(() => StartCoroutine(SortHomework(_HomeworkTypes, _HomeworkStatus)));
            #endregion
            
            base.Initialize();
        }

        #region sorting
        [Flags]
        public enum HomeworkTypes
        {
            huiswerk = 1,
            toets = 2,
            grote_toets = 4,
        }
        
        [Flags]
        public enum HomeworkStatus
        {
            gemaakt = 1,
            niet_gemaakt = 2,
        }

        [SerializeField] private HomeworkTypes _HomeworkTypes;
        [SerializeField] private HomeworkStatus _HomeworkStatus;
        [SerializeField] private Toggle huiswerk;
        [SerializeField] private Toggle toets;
        [SerializeField] private Toggle grote_toets;
        [SerializeField] private Toggle gemaakt;
        [SerializeField] private Toggle niet_gemaakt;
        [SerializeField] private Button sorteerButton;
        
        [ContextMenu("Sort by")]
        public void sort()
        {
            StartCoroutine(SortHomework(_HomeworkTypes, _HomeworkStatus));
        }
        
        public IEnumerator SortHomework(HomeworkTypes types, HomeworkStatus status) 
        {
            HomeworkInfo[] homeworkInfos = content.GetComponentsInChildren<HomeworkInfo>(true);
            foreach(HomeworkInfo homeworkInfo in homeworkInfos) {
                homeworkInfo.gameObject.SetActive(false);

                if (homeworkInfo.homeworkInfo.studiewijzerItem.huiswerkType == "HUISWERK" && types.HasFlag(HomeworkTypes.huiswerk)) {
                    if (homeworkInfo.gemaakt.isOn && status.HasFlag(HomeworkStatus.gemaakt)) homeworkInfo.gameObject.SetActive(true);
                    if (!homeworkInfo.gemaakt.isOn && status.HasFlag(HomeworkStatus.niet_gemaakt)) homeworkInfo.gameObject.SetActive(true);
                }
                if (homeworkInfo.homeworkInfo.studiewijzerItem.huiswerkType == "TOETS" && types.HasFlag(HomeworkTypes.toets)) {
                    if (homeworkInfo.gemaakt.isOn && status.HasFlag(HomeworkStatus.gemaakt)) homeworkInfo.gameObject.SetActive(true);
                    if (!homeworkInfo.gemaakt.isOn && status.HasFlag(HomeworkStatus.niet_gemaakt)) homeworkInfo.gameObject.SetActive(true);
                }
                if (homeworkInfo.homeworkInfo.studiewijzerItem.huiswerkType == "GROTE_TOETS" && types.HasFlag(HomeworkTypes.grote_toets)) {
                    if (homeworkInfo.gemaakt.isOn && status.HasFlag(HomeworkStatus.gemaakt)) homeworkInfo.gameObject.SetActive(true);
                    if (!homeworkInfo.gemaakt.isOn && status.HasFlag(HomeworkStatus.niet_gemaakt)) homeworkInfo.gameObject.SetActive(true);
                }

                yield return null;
            }
        }
        #endregion

        public override void Refresh(object args)
        {
            openNavigationButton.onClick.RemoveAllListeners();
            closeButtonWholePage.onClick.RemoveAllListeners();
            _AddHomeworkButton.onClick.RemoveAllListeners();
            _recenterButton.onClick.RemoveAllListeners();
            base.Refresh(args);
        }

        private void Recenter()
        {
            float decelerationRate = _ScrollRect.decelerationRate;
            _ScrollRect.decelerationRate = 0f;
            _ScrollRect.content.DOLocalMove(_ScrollRect.GetSnapToPositionToBringChildIntoView(_rectTransformDefault), 0.5f, true).onComplete += () => _ScrollRect.decelerationRate = decelerationRate;
        }
        
        private bool UpdateGemaaktStatus(long huiswerkId, bool gemaakt)
        {
            SomtodayHoweworkStatus root = new SomtodayHoweworkStatus
            {
                leerling = new Leerling
                {
                    links = new List<Link>
                    {
                        new()
                        {
                            id = int.Parse(LocalPrefs.GetString("somtoday-student_id")),
                            rel = "self",
                            href = $"{LocalPrefs.GetString("somtoday-api_url")}/rest/v1/leerlingen/{LocalPrefs.GetString("somtoday-student_id")}"
                        }
                    }
                },
                swiToekenningId = huiswerkId,
				gemaakt = gemaakt
            };

            //root to a json string
            string json = JsonConvert.SerializeObject(root);

            UnityWebRequest www = UnityWebRequest.Put($"{LocalPrefs.GetString("somtoday-api_url")}/rest/v1/swigemaakt/cou", json);

            www.SetRequestHeader("authorization", "Bearer " + LocalPrefs.GetString("somtoday-access_token"));

            www.SetRequestHeader("Accept", "application/json");
            www.SetRequestHeader("Content-Type", "application/json");
            www.SendWebRequest();

            while (!www.isDone)
            {
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                if (www.downloadHandler.text.Contains("\"gemaakt\":true"))
                {
                    www.Dispose();
                    return true;
                }

                www.Dispose();
                return false;
            }

            www.Dispose();
            return false;
        }
        
        float lerpDuration = 3; 
        float startValue = 0; 
        float endValue = 10; 
        float valueToLerp;
        [SerializeField] AnimationCurve curve;
        public IEnumerator lerpTo(RectTransform instance, float top, float duration)
        {
            float timeElapsed = 0;
            lerpDuration = duration;
            startValue = instance.offsetMax.y;
            endValue = top;
            while (timeElapsed < lerpDuration)
            {
                valueToLerp = Vector2.Lerp(new Vector2(startValue, 0f), new Vector2(endValue, 0f), curve.Evaluate(timeElapsed)).x;
                timeElapsed += Time.deltaTime;
                instance.offsetMax = new Vector2(instance.offsetMax.x, valueToLerp);
                yield return null;
            }
            valueToLerp = endValue;
            instance.offsetMax = new Vector2(instance.offsetMax.x, valueToLerp);
        }
        
        #region model

        public class Leerling
        {
            public List<Link> links { get; set; }
        }

        public class Link
        {
            public int id { get; set; }
            public string rel { get; set; }
            public string href { get; set; }
        }

        [Serializable]
        public class SomtodayHoweworkStatus
        {
            public Leerling leerling { get; set; }
            public long swiToekenningId { get; set; }
            public bool gemaakt { get; set; }
        }

        #endregion
    }
    
    
    public static class ScrollRectExtensions
    {
        public static Vector2 GetSnapToPositionToBringChildIntoView(this ScrollRect instance, RectTransform child)
        {
            if (child == null)
                return Vector2.zero;
            
            Canvas.ForceUpdateCanvases();
            Vector2 viewportLocalPosition = instance.viewport.localPosition;
            Vector2 childLocalPosition   = child.localPosition;
            Vector2 result = new Vector2(
                0 - (viewportLocalPosition.x + childLocalPosition.x),
                0 - (viewportLocalPosition.y + childLocalPosition.y)
            );
            result.y += (instance.viewport.rect.height -10) / 2 - 5;
            result.x = 0f;
            return result;
        }
    }
}
