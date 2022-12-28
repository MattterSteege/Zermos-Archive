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
        [SerializeField] RectTransform _rectTransformDefault;

        [SerializeField] private Button _AddHomeworkButton;
        [SerializeField] private Button _recenterButton;

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

                homeworkItem.GetComponent<HomeworkInfo>().SetHomeworkInfo(vak, onderwerp,
                    HomeworkItem.additionalObjects.swigemaaktVinkjes?.items[0].gemaakt ?? false, HomeworkItem);

                homeworkItem.GetComponent<Button>().onClick.AddListener(() =>
                {
                    ViewManager.Instance.ShowNewView<HomeworkItemView>(homeworkItem.GetComponent<HomeworkInfo>()
                        .homeworkInfo);
                });

                day = HomeworkItem.datumTijd.Day;
            }
            
            int closestTimeIndex = _Dates.IndexOf(_Dates.OrderBy(t => Math.Abs((t - TimeManager.Instance.CurrentDateTime).Ticks)).First());
            _ScrollRect.content.position = _ScrollRect.GetSnapToPositionToBringChildIntoView(_rectTransformDefault = _homeworkDateDividers[closestTimeIndex].GetComponent<RectTransform>());
            
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
            _ScrollRect.content.DOLocalMove(new Vector3(0f, 0f, 0f), 0.1f, true);
            base.Refresh(args);
        }

        private void Recenter()
        {
            float decelerationRate = _ScrollRect.decelerationRate;
            _ScrollRect.decelerationRate = 0f;
            _ScrollRect.content.DOLocalMove(_ScrollRect.GetSnapToPositionToBringChildIntoView(_rectTransformDefault), 0.5f, true).onComplete += () => _ScrollRect.decelerationRate = decelerationRate;
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
