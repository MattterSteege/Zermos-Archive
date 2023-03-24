using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class WatMoetIkHalenView : SubView
    {
        [SerializeField] private List<Grades.Item> Grades;
        [SerializeField] private TMP_InputField cijferInput;
        [SerializeField] private TMP_InputField wegingInput;
        [SerializeField] private GameObject WatMoetIkHalenPrefab;
        [SerializeField] private GameObject content;

        public override void Show(object args = null)
        {
            this.Grades = (List<Grades.Item>)args ?? new List<Grades.Item>();

            if (Grades.Count == 0) return;
            
            UpdateTable();

            base.Show(args);
        }

        private void UpdateTable()
        {
            foreach (Transform gameObject in content.transform)
            {
                Destroy(gameObject.gameObject);
            }

            foreach (Grades.Item grade in Grades)
            {
                var gradeItem = Instantiate(WatMoetIkHalenPrefab, content.transform);
                gradeItem.GetComponent<WatMoetIkHalenPrefab>().SetGradeInfo(grade.geldendResultaat, grade.weging.ToString());
            }

            var go = new GameObject("Spacing - 8");
            go.AddComponent<LayoutElement>().minHeight = 8;
            go.GetComponent<LayoutElement>().preferredHeight = 8;
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 8);
            go.transform.SetParent(content.transform, false);
            
            var watMoetIkHalen = Instantiate(WatMoetIkHalenPrefab, content.transform);
            watMoetIkHalen.GetComponent<WatMoetIkHalenPrefab>().SetGradeInfo(WatMoetIkHalen(Grades, Convert.ToInt32(wegingInput.text), float.Parse(cijferInput.text.Replace(".", ","))), wegingInput.text);
        }

        public override void Initialize()
        {
            base.Initialize();
            return;
            
            backButton.onClick.AddListener(() =>
            {
                gameObject.GetComponentInParent<SubViewManager>().HideView<WatMoetIkHalenView>();
            });

            cijferInput.onValueChanged.AddListener( (x) =>
            {
                if (cijferInput.text == "") return;
                UpdateTable();
            });
            wegingInput.onValueChanged.AddListener( (x) =>
            {
                if (wegingInput.text == "") return;
                UpdateTable();
            });
            
        }

        private string WatMoetIkHalen(List<Grades.Item> cijfers, int weging, float gewenstCijfer = 5.5f)
        {
            int totaleWeging = cijfers.Sum(x => x.weging);

            float alBehaaldePunten = cijfers.Sum(x => x.weging * float.Parse(x.geldendResultaat));
        
            float nogTeBehalen = gewenstCijfer * totaleWeging - alBehaaldePunten;

            return (gewenstCijfer + nogTeBehalen / weging).ToString("0.00");
        }
        
        public override void Refresh(object args)
        {
            backButton.onClick.RemoveAllListeners();
            base.Refresh(args);
        }
    }
}
