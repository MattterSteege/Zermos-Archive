using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace UI.Views
{
    public class WatMoetIkHalenView : View
    {
        [SerializeField] private List<Grades.Item> Grades;
        [SerializeField] private TMP_InputField cijferInput;
        [SerializeField] private TMP_InputField wegingInput;
        [SerializeField] private TMP_Text cijfersText;
        [SerializeField] private TMP_Text wegingText;


        public override void Show(object args = null)
        {
            this.Grades = (List<Grades.Item>)args ?? new List<Grades.Item>();

            if (Grades.Count == 0) return;
            
            UpdateTable();

            base.Show(args);
        }

        private void UpdateTable()
        {
            cijfersText.text = "";
            cijfersText.text = "cijfers‏‏‎ ‎‎ ‎\n" + string.Join("‎ ‎‎ ‎\n", Grades.Select(x => x.geldendResultaat.ToString().Replace(",", "."))) + "‎ ‎‎ ‎\n‾‾‾‾‾‾‾‾‾‾";
            cijfersText.text += $"\n{WatMoetIkHalen(Grades, Convert.ToInt32(wegingInput.text), float.Parse(cijferInput.text.Replace(".", ",")))}‎ ‎‎ ‎";
            
            wegingText.text = "";
            wegingText.text = "weging‏‏‎ ‎‎ ‎\n" + string.Join("‎ ‎‎ ‎\n", Grades.Select(x => x.weging.ToString())) + "‎ ‎‎ ‎\n‾‾‾‾‾‾‾‾‾‾";
            wegingText.text += $"\n{wegingInput.text}‎ ‎‎ ‎";
        }

        public override void Initialize()
        {
            openNavigationButton.onClick.AddListener(() =>
            {
                ViewManager.Instance.ShowNewView<GradeItemView>(Grades);  
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
            
            base.Initialize();
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
            openNavigationButton.onClick.RemoveAllListeners();
            base.Refresh(args);
        }
    }
}
