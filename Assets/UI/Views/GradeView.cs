using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class GradeView : View
    {
        [SerializeField] private GameObject content;
        [SerializeField] private GameObject gradePrefab;
        [SerializeField] private Grades gradesObject;
        [SerializeField] private Vakken vakkenObject;
    
        [SerializeField] private GameObject LastGradeContentGameObject;
        private List<Grades.Item> lastGrades = new();

        [ContextMenu("Refresh")]
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
        
            var grades = gradesObject.getGrades();
            if (grades == null) return;

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
            
            foreach (Vakken.Item Vak in vakkenObject.getVakken().items ?? new List<Vakken.Item>())
            {
                List<Grades.Item> GradesPerVak = grades.items.Where(x => x.vak.naam == Vak.naam).ToList();
                
                if (GradesPerVak.Count > 0)
                {
                    int totalWeight = GradesPerVak.Sum(x => x.weging);
                    float avarege;
                    
                    if (totalWeight == 0)
                        avarege = GradesPerVak.Sum(x => float.Parse(x.geldendResultaat));
                    else
                        avarege = GradesPerVak.Sum(x => x.weging * float.Parse(x.geldendResultaat) / totalWeight);
                    
                    var gradeView = Instantiate(gradePrefab, content.transform);
                    gradeView.GetComponent<GradeInfo>().SetGradeInfo(Vak.naam ?? "", "",  "", totalWeight + "x", avarege.ToString("0.0"));
                    
                    gradeView.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        ViewManager.Instance.ShowNewView<GradeItemView>(GradesPerVak);
                    });
                }
            }
        
            base.Initialize();
        }
    }

    public static class MiscExtensions
    {
        // Ex: collection.TakeLast(5);
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N)
        {
            return source.Skip(Math.Max(0, source.Count() - N));
        }
    }
}