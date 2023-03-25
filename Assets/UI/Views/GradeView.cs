using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class GradeView : View
    {
        [SerializeField] SubViewManager subViewManager;
        [SerializeField] private GameObject content;
        [SerializeField] private GameObject gradePrefab;
        [SerializeField] private Grades gradesObject;
        [SerializeField] private Vakken vakkenObject;
        [SerializeField] private Toggle _FilterButton;
        [SerializeField] private RectTransform _filterObject;
        [SerializeField] private ScrollRect _ScrollRect;

        [SerializeField] private GameObject LastGradeContentGameObject;
        private List<Grades.Item> lastGrades = new List<Grades.Item>();

        [ContextMenu("Refresh")]
        public override void Initialize()
        {
            MonoBehaviour Mono = ViewManager.Instance.GetComponent<MonoBehaviour>();
            Mono.StartCoroutine(subViewManager.Initialize());
            
            // openNavigationButton.onClick.AddListener(() =>
            // {
            //     openNavigationButton.enabled = false;
            //     ViewManager.Instance.ShowNavigation();
            // });
            //
            // closeButtonWholePage.onClick.AddListener(() =>
            // {
            //     openNavigationButton.enabled = true;
            //     ViewManager.Instance.HideNavigation();
            // });

            var grades = gradesObject.getGrades();
            if (grades == null) return;

            if (LocalPrefs.GetBool("show_laatste_cijfers", false) == true)
            {
                _FilterButton.isOn = true;
                _filterObject.DOAnchorPosY(-153f, 0.5f);
                //but keep the recttransform's bottom at 62f
                _ScrollRect.viewport.DOAnchorPosY(-148f, 0.4f).onComplete += () => _ScrollRect.viewport.Bottom(62f);
            }
            
            _FilterButton.onValueChanged.AddListener((ctx) =>
            {
                // _filterObject.offsetMin = new Vector2(_filterObject.offsetMin.x, 20f);
                // StartCoroutine(lerpTo(_filterObject, ctx ? 173f : 0f, 0.5f));
                _filterObject.DOAnchorPosY(ctx ? -153f : 150, 0.5f);

                _ScrollRect.viewport.Bottom(-84f);
                _ScrollRect.viewport.DOAnchorPosY(ctx ? -108 : 0f, 0.4f).onComplete += () => _ScrollRect.viewport.Bottom(62f);
            });

            grades.items.RemoveAll(x =>
                x.type.ToLower() == "deeltoetskolom"); //TODO: deelcijfers moeten nog worden opgeteld

            lastGrades = grades.items.TakeLast(2).Reverse().ToList();

            foreach (Transform child in content.transform)
                Destroy(child.gameObject);
            

            foreach (Transform child in LastGradeContentGameObject.transform)
                Destroy(child.gameObject);
            
            foreach (var grade in lastGrades)
            {
                var gradeView = Instantiate(gradePrefab, LastGradeContentGameObject.transform);
                gradeView.GetComponent<GradeInfo>().SetGradeInfo(grade.vak.naam ?? "",
                    grade.datumInvoer.ToString("d MMMM"), /*grade.omschrijving*/ "", grade.weging + "x",
                    grade.geldendResultaat ?? "-");
            }

            var vakken = vakkenObject.getVakken();

            if (vakken.items.Count == 0) vakken = vakkenObject.getVakken(false);

            grade[] AllGrades = new grade[grades.items.Count];
            int index = 0;
            float average;
            
            foreach (Vakken.Item Vak in vakken.items ?? new List<Vakken.Item>())
            {
                List<Grades.Item> GradesPerVak = grades.items.Where(x => x.vak.naam == Vak.vak.naam).ToList();

                if (GradesPerVak.Count > 0)
                {
                    grade[] Grades = new grade[GradesPerVak.Count];

                    
                    for (int i = 0; i < GradesPerVak.Count; i++)
                    {
                        int weging = GradesPerVak[i].weging == 0 ? GradesPerVak[i].examenWeging : GradesPerVak[i].weging;
                        Grades[i] = new grade
                        {
                            weging = weging,
                            resultaat = (float)double.Parse(GradesPerVak[i].resultaat.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture)
                        };
                        AllGrades[index] = new grade
                        {
                            weging = weging,
                            resultaat = (float)double.Parse(GradesPerVak[i].resultaat.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture)
                        };
                        index++;
                    }

                    average = CalculateWeightedAverage(Grades.Select(x => x.resultaat).ToList(),
                        Grades.Select(x => x.weging).ToList());

                    var gradeView = Instantiate(gradePrefab, content.transform);
                    gradeView.GetComponent<GradeInfo>().SetGradeInfo(Vak.vak.naam ?? "", "", "",
                        Grades.Sum(x => x.weging) + "x", MathF.Round(average, LocalPrefs.GetInt("decimals_grades", 1)).ToString());

                    gradeView.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        subViewManager.ShowNewView<GradeItemView>(GradesPerVak);
                    });

                }
            }
            
            average = CalculateWeightedAverage(AllGrades.Select(x => x.resultaat).ToList(), AllGrades.Select(x => x.weging).ToList());
            var totaalCijferGradeView = Instantiate(gradePrefab, content.transform);
            totaalCijferGradeView.GetComponent<RectTransform>().SetAsFirstSibling();
            totaalCijferGradeView.GetComponent<GradeInfo>().SetGradeInfo("Totaalcijfers", "", "", AllGrades.Sum(x => x.weging) + "x", MathF.Round(average, LocalPrefs.GetInt("decimals_grades", 1)).ToString());
            totaalCijferGradeView.GetComponent<Button>().onClick.AddListener(() =>
            {
                subViewManager.Refresh<GradeStatisticsView>(true);
                subViewManager.ShowNewView<GradeStatisticsView>(grades.items);
                subViewManager.Refresh<GradeStatisticsView>(false);
            });
            
            base.Initialize();
        }

        public override void Refresh(object args)
        {
            openNavigationButton.onClick.RemoveAllListeners();
            closeButtonWholePage.onClick.RemoveAllListeners();
            base.Refresh(args);
        }

        class grade
        {
            public int weging;
            public float resultaat;
        }

        public static float CalculateWeightedAverage(List<float> grades, List<int> weights)
        {
            if (grades == null || weights == null || grades.Count != weights.Count)
            {
                throw new ArgumentException("The grades and weights lists must not be null and must have the same number of elements.");
            }

            float sumOfWeightedGrades = 0.0f;
            int sumOfWeights = weights.Sum();
            bool hasNonZeroWeight = false;

            for (int i = 0; i < grades.Count; i++)
            {
                if (weights[i] == 0 && sumOfWeights == 0)
                {
                    sumOfWeightedGrades += grades[i];
                    continue;
                }

                sumOfWeightedGrades += grades[i] * weights[i];
                hasNonZeroWeight = true;
            }

            if (!hasNonZeroWeight)
            {
                sumOfWeights = grades.Count;
            }

            if (sumOfWeights == 0.0f)
            {
                throw new ArgumentException("The sum of weights must not be zero.");
            }

            return sumOfWeightedGrades / sumOfWeights;
        }

    }

    public static class MiscExtensions
    {
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N)
        {
            return source.Skip(Math.Max(0, source.Count() - N));
        }
    }

    public static class RectTransformExtensions
    {
        public static RectTransform SetRect(this RectTransform rt, float left, float top, float right, float bottom)
        {
            rt.offsetMin = new Vector2(left, bottom);
            rt.offsetMax = new Vector2(-right, -top);
            return rt;
        }

        public static RectTransform Left(this RectTransform rt, float x)
        {
            rt.offsetMin = new Vector2(x, rt.offsetMin.y);
            return rt;
        }

        public static RectTransform Right(this RectTransform rt, float x)
        {
            rt.offsetMax = new Vector2(-x, rt.offsetMax.y);
            return rt;
        }

        public static RectTransform Bottom(this RectTransform rt, float y)
        {
            rt.offsetMin = new Vector2(rt.offsetMin.x, y);
            return rt;
        }

        public static RectTransform Top(this RectTransform rt, float y)
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -y);
            return rt;
        }

        public static RectTransform SetRect(this RectTransform rt, float left, float top, float right, float bottom,
            float duration)
        {
            var offsetMin = rt.offsetMin;
            var offsetMax = rt.offsetMax;
            var newOffsetMin = new Vector2(left, bottom);
            var newOffsetMax = new Vector2(-right, -top);

            AnimateOffsetMin(rt, offsetMin, newOffsetMin, duration);
            AnimateOffsetMax(rt, offsetMax, newOffsetMax, duration);

            return rt;
        }

        public static RectTransform Left(this RectTransform rt, float x, float duration)
        {
            var offsetMin = rt.offsetMin;
            var newOffsetMin = new Vector2(x, offsetMin.y);

            AnimateOffsetMin(rt, offsetMin, newOffsetMin, duration);

            return rt;
        }

        public static RectTransform Right(this RectTransform rt, float x, float duration)
        {
            var offsetMax = rt.offsetMax;
            var newOffsetMax = new Vector2(-x, offsetMax.y);

            AnimateOffsetMax(rt, offsetMax, newOffsetMax, duration);

            return rt;
        }

        public static RectTransform Bottom(this RectTransform rt, float y, float duration)
        {
            var offsetMin = rt.offsetMin;
            var newOffsetMin = new Vector2(offsetMin.x, y);

            AnimateOffsetMin(rt, offsetMin, newOffsetMin, duration);

            return rt;
        }

        public static RectTransform Top(this RectTransform rt, float y, float duration)
        {
            var offsetMax = rt.offsetMax;
            var newOffsetMax = new Vector2(offsetMax.x, -y);

            AnimateOffsetMax(rt, offsetMax, newOffsetMax, duration);

            return rt;
        }

        private static void AnimateOffsetMin(RectTransform rt, Vector2 start, Vector2 end, float duration)
        {
            Tweener tweener = DOTween.To(() => rt.offsetMin, x => rt.offsetMin = x, end, duration);
            tweener.SetEase(Ease.OutQuint);
        }

        private static void AnimateOffsetMax(RectTransform rt, Vector2 start, Vector2 end, float duration)
        {
            Tweener tweener = DOTween.To(() => rt.offsetMax, x => rt.offsetMax = x, end, duration);
            tweener.SetEase(Ease.OutQuint);
        }
    }
}