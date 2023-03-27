using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace UI.Views
{
    public class HomeworkView : View
    {
        [SerializeField] SubViewManager subViewManager;
        
        [SerializeField] Homework homeworkObject;
        [SerializeField] GameObject homeworkPrefab;
        [SerializeField] GameObject content;
        [SerializeField] GameObject DividerPrefab;
        [SerializeField] GameObject dayContainterPrefab;

        [SerializeField] private CustomHomework _CustomHomework;
        [SerializeField] private ScrollRect _ScrollRect;
        [SerializeField] private RectTransform _rectTransformDefault;

        [SerializeField] private Button _AddHomeworkButton;
        [SerializeField] private Button _recenterButton;
        [SerializeField] private Toggle _FilterButton;
        [SerializeField] private RectTransform _filterObject;
        [SerializeField] AnimationCurve Maincurve;
        [SerializeField] AnimationCurve Loadingcurve;

        private List<DateTime> _Dates = new List<DateTime>();
        private List<GameObject> _homeworkDateDividers = new List<GameObject>();

        public override void Initialize()
        {
            MonoBehaviour Mono = ViewManager.Instance.GetComponent<MonoBehaviour>();
            Mono.StartCoroutine(subViewManager.Initialize());

            //_AddHomeworkButton.onClick.AddListener(() => ViewManager.Instance.ShowNewView<AddHomeworkView>());
            _recenterButton.onClick.AddListener(() => Recenter());
            ViewManager.onInitializeComplete += ctx =>
            {
                _ScrollRect.decelerationRate = 0f;
                _ScrollRect.content
                    .DOLocalMove(
                        _ScrollRect.GetSnapToPositionToBringChildIntoView((RectTransform) _rectTransformDefault), 0.5f,
                        true).onComplete += () => _ScrollRect.decelerationRate = 0.135f;
            };

            _FilterButton.onValueChanged.AddListener((ctx) =>
            {
                // _filterObject.offsetMin = new Vector2(_filterObject.offsetMin.x, 20f);
                // StartCoroutine(lerpTo(_filterObject, ctx ? 173f : 0f, 0.5f));
                _filterObject.DOAnchorPosY(ctx ? -173f : 150, 0.5f);

                if (ctx)
                {
                    _ScrollRect.viewport.DOAnchorPosY(-148f, 0.4f).onComplete += () => _ScrollRect.viewport.Bottom(62f);
                }
                else
                {
                    _ScrollRect.viewport.Bottom(-200f);
                    _ScrollRect.viewport.DOAnchorPosY(0f, 0.4f).onComplete += () => _ScrollRect.viewport.Bottom(62f);
                }
            });

            List<Homework.Item> homework = homeworkObject.GetHomework();

            if (homework == null)
            {
                base.Initialize();
                return;
            }

            int day = -1;
            GameObject dayContainer = null;

            foreach (Homework.Item HomeworkItem in homework)
            {
                if (HomeworkItem.datumTijd.Day != day)
                {
                    dayContainer = Instantiate(dayContainterPrefab, content.transform);
                    var go = Instantiate(DividerPrefab, dayContainer.transform);
                    go.GetComponent<homeworkDivider>().Datum.text =
                        ((DateTimeOffset) HomeworkItem.datumTijd).DateTime.ToString("dddd d MMMM");
                    _Dates.Add(((DateTimeOffset) HomeworkItem.datumTijd).DateTime.Date);
                    _homeworkDateDividers.Add(go);
                }

                var homeworkItem = Instantiate(homeworkPrefab, dayContainer.transform);

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

                homeworkItem.GetComponent<HomeworkInfo>().SetHomeworkInfo(vak, onderwerp, HomeworkItem.datumTijd,
                    HomeworkItem.additionalObjects.swigemaaktVinkjes?.items[0].gemaakt ?? false, HomeworkItem, subViewManager);

                // homeworkItem.GetComponent<Button>().onClick.AddListener(() =>
                // {
                //     subViewManager.ShowNewView<HomeworkItemView>(homeworkItem.GetComponent<HomeworkInfo>()
                //         .homeworkInfo);
                // });

                homeworkItem.GetComponent<HomeworkInfo>().gemaakt.onValueChanged.AddListener((isOn) =>
                {
                    bool done = UpdateGemaaktStatus((long) HomeworkItem.links[0].id, isOn);
                    homeworkItem.GetComponent<HomeworkInfo>().gemaakt.GetComponentInChildren<TMP_Text>().text = done ? "Voltooid" : "Onvoltooid";
                });

                day = HomeworkItem.datumTijd.Day;
            }

            int closestTimeIndex =
                _Dates.IndexOf(_Dates.OrderBy(t => Math.Abs((t - TimeManager.Instance.CurrentDateTime).Ticks)).First());
            // LayoutRebuilder.ForceRebuildLayoutImmediate(_ScrollRect.content);
            gameObject.SetActive(true);
            _ScrollRect.content.DOLocalMove(_ScrollRect.content.position = _ScrollRect.GetSnapToPositionToBringChildIntoView(_rectTransformDefault = (RectTransform) _homeworkDateDividers[closestTimeIndex].GetComponent<RectTransform>().parent) - new Vector2(0f, 11f), 0.5f, true);
            gameObject.SetActive(false);

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
            foreach (HomeworkInfo homeworkInfo in homeworkInfos)
            {
                homeworkInfo.gameObject.SetActive(false);

                if (homeworkInfo.homeworkInfo.studiewijzerItem.huiswerkType == "HUISWERK" &&
                    types.HasFlag(HomeworkTypes.huiswerk))
                {
                    if (homeworkInfo.gemaakt.isOn && status.HasFlag(HomeworkStatus.gemaakt))
                        homeworkInfo.gameObject.SetActive(true);
                    if (!homeworkInfo.gemaakt.isOn && status.HasFlag(HomeworkStatus.niet_gemaakt))
                        homeworkInfo.gameObject.SetActive(true);
                }

                if (homeworkInfo.homeworkInfo.studiewijzerItem.huiswerkType == "TOETS" &&
                    types.HasFlag(HomeworkTypes.toets))
                {
                    if (homeworkInfo.gemaakt.isOn && status.HasFlag(HomeworkStatus.gemaakt))
                        homeworkInfo.gameObject.SetActive(true);
                    if (!homeworkInfo.gemaakt.isOn && status.HasFlag(HomeworkStatus.niet_gemaakt))
                        homeworkInfo.gameObject.SetActive(true);
                }

                if (homeworkInfo.homeworkInfo.studiewijzerItem.huiswerkType == "GROTE_TOETS" &&
                    types.HasFlag(HomeworkTypes.grote_toets))
                {
                    if (homeworkInfo.gemaakt.isOn && status.HasFlag(HomeworkStatus.gemaakt))
                        homeworkInfo.gameObject.SetActive(true);
                    if (!homeworkInfo.gemaakt.isOn && status.HasFlag(HomeworkStatus.niet_gemaakt))
                        homeworkInfo.gameObject.SetActive(true);
                }

                var parent = homeworkInfo.gameObject.transform.parent.gameObject;
                bool allActive = false;
                foreach (Transform child in parent.transform)
                {
                    if (child.gameObject.name == "HomeworkViewDivider(Clone)") continue;
                    if (child.gameObject.activeSelf)
                    {
                        allActive = true;
                        break;
                    }

                }

                parent.gameObject.SetActive(allActive);
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

        public override void Show(object args = null)
        {
            base.Show(args);
            StartCoroutine(LoadHomework());
        }

        private IEnumerator LoadHomework()
        {
            float time = 3f;
            
            while (_ScrollRect.content.rect.height < 100f && time > 0f)
            {
                time -= Time.deltaTime;
                yield return null;
            }
            Recenter(false);
        }

        private void Recenter(bool showMainCurve = true)
        {
            if (showMainCurve)
            {
                _ScrollRect.decelerationRate = 0f;
                _ScrollRect.vertical = false;
                _ScrollRect.ScrollToTop(_rectTransformDefault, Maincurve);
            }
            else
            {
                _ScrollRect.decelerationRate = 0f;
                _ScrollRect.vertical = false;
                _ScrollRect.ScrollToTop(_rectTransformDefault, Loadingcurve);
            }
        }

        private bool UpdateGemaaktStatus(long huiswerkId, bool gemaakt)
        {
            SomtodayHoweworkStatus root = new SomtodayHoweworkStatus
            {
                leerling = new Leerling
                {
                    links = new List<Link>
                    {
                        new Link()
                        {
                            id = int.Parse(LocalPrefs.GetString("somtoday-student_id")),
                            rel = "self",
                            href =
                                $"{LocalPrefs.GetString("somtoday-api_url")}/rest/v1/leerlingen/{LocalPrefs.GetString("somtoday-student_id")}"
                        }
                    }
                },
                swiToekenningId = huiswerkId,
                gemaakt = gemaakt
            };

            //root to a json string
            string json = JsonConvert.SerializeObject(root);

            UnityWebRequest www =
                UnityWebRequest.Put($"{LocalPrefs.GetString("somtoday-api_url")}/rest/v1/swigemaakt/cou", json);

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
            AndroidUIToast.ShowToast("Kon de huiswerk status niet updaten");
            return false;
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
            Vector2 childLocalPosition = child.localPosition;
            Vector2 result = new Vector2(
                0 - (viewportLocalPosition.x + childLocalPosition.x),
                0 - (viewportLocalPosition.y + childLocalPosition.y)
            );
            result.y += ((instance.viewport.rect.height - 10) / 2) - (child.rect.height) - 11f;
            result.x = 0f;
            return result;
        }
    }

    public static class ScrollToCenterHelper
    {

        private const int MaxCornersCount = 4;
        private const float ScrollTimeStep = 0.25f;
        private const int MaxScrollTimeSec = 2;

        private static readonly Vector3[] Corners = new Vector3[MaxCornersCount];
        private static readonly WaitForEndOfFrame WaitForEndOfFrame = new();
        private static Coroutine _coroutine;

        public static void ScrollToCenter(this ScrollRect scrollRect, RectTransform target, float time = 0.5f)
        {
            MonoBehaviour monoBehaviour = ViewManager.Instance.GetComponent<MonoBehaviour>();
            // The scroll rects' view's space is used to calculate scroll position
            var view = scrollRect.viewport != null ? scrollRect.viewport : scrollRect.GetComponent<RectTransform>();

            // Calculate the scroll offset in the view's space
            var viewRect = view.rect;
            var elementBounds = target.TransformBoundsTo(view);
            var offset = viewRect.center.y - elementBounds.center.y;

            // Normalize and apply the calculated offset
            var scrollPos = scrollRect.verticalNormalizedPosition - scrollRect.NormalizeScrollDistance(1, offset);

            if (_coroutine != null)
            {
                monoBehaviour.StopCoroutine(_coroutine);
            }

            _coroutine =
                monoBehaviour.StartCoroutine(
                    VerticalNormalizedPositionSmooth(scrollRect, Mathf.Clamp(scrollPos, 0f, 1f)));
        }
        
        public static void ScrollToTop(this ScrollRect scrollRect, RectTransform target, AnimationCurve animationCurve)
        {
            MonoBehaviour monoBehaviour = ViewManager.Instance.GetComponent<MonoBehaviour>();
            // The scroll rects' view's space is used to calculate scroll position
            var view = scrollRect.viewport != null ? scrollRect.viewport : scrollRect.GetComponent<RectTransform>();

            // Calculate the scroll offset in the view's space
            var viewRect = view.rect;
            var elementBounds = target.TransformBoundsTo(view);
            var offset = viewRect.center.y - elementBounds.center.y + viewRect.height / 2 - elementBounds.size.y / 2;

            
            
            // Normalize and apply the calculated offset
            var scrollPos = scrollRect.verticalNormalizedPosition - scrollRect.NormalizeScrollDistance(1, offset);

            if (_coroutine != null)
            {
                monoBehaviour.StopCoroutine(_coroutine);
            }

            _coroutine =
                monoBehaviour.StartCoroutine(
                    VerticalNormalizedPositionSmooth(scrollRect, Mathf.Clamp(scrollPos, 0f, 1f), animationCurve));
        }

        private static Bounds TransformBoundsTo(this RectTransform source, Transform target)
        {
            var bounds = new Bounds();

            if (source != null)
            {
                source.GetWorldCorners(Corners);

                var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

                var matrix = target.worldToLocalMatrix;

                for (int j = 0; j < MaxCornersCount; j++)
                {
                    Vector3 v = matrix.MultiplyPoint3x4(Corners[j]);
                    vMin = Vector3.Min(v, vMin);
                    vMax = Vector3.Max(v, vMax);
                }

                bounds = new Bounds(vMin, Vector3.zero);
                bounds.Encapsulate(vMax);
            }

            return bounds;
        }

        private static float NormalizeScrollDistance(this ScrollRect scrollRect, int axis, float distance)
        {
            var viewport = scrollRect.viewport;
            var viewRect = viewport != null ? viewport : scrollRect.GetComponent<RectTransform>();
            var rect = viewRect.rect;
            var viewBounds = new Bounds(rect.center, rect.size);

            var content = scrollRect.content;
            var contentBounds = content != null ? content.TransformBoundsTo(viewRect) : new Bounds();

            var hiddenLength = contentBounds.size[axis] - viewBounds.size[axis];

            return distance / hiddenLength;
        }

        private static IEnumerator VerticalNormalizedPositionSmooth(ScrollRect scrollRect, float position, AnimationCurve AnimationCurve = null)
        {
            var time = 0f;
            var startPosition = scrollRect.verticalNormalizedPosition;
            var endPosition = position;

            while (time < AnimationCurve.keys[AnimationCurve.length - 1].time)
            {
                scrollRect.verticalNormalizedPosition = Mathf.Lerp(startPosition, endPosition, AnimationCurve?.Evaluate(time) ?? time);
                time += Time.deltaTime;
                yield return WaitForEndOfFrame;
            }

            scrollRect.verticalNormalizedPosition = endPosition;
            scrollRect.vertical = true;
            scrollRect.decelerationRate = 0.135f;
        }
    }
}
