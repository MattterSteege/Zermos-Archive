using TMPro;
using UI.Views;
using UnityEngine;
using UnityEngine.UI;

namespace UI.SubViews
{
    public class AlgemeneInstellingenSubView : SubView
    {
        [SerializeField] Toggle show_tussenuren;
        [SerializeField] PlusMinusButtons minutes_before_class;
        [SerializeField] PlusMinusButtons homework_stays_till;

        [Space, SerializeField] WeekRoosterView weekRoosterView;
        [SerializeField] DagRoosterView dagRoosterView;
        [SerializeField] private TMP_Dropdown view_at_startup;
        
        public override void Initialize()
        {
            backButton.onClick.AddListener(() =>
            {
                gameObject.GetComponentInParent<SubViewManager>().ShowParentView();
            });
            
            
            //settings components
            show_tussenuren.isOn = LocalPrefs.GetBool("show_tussenuren", true);
            show_tussenuren.onValueChanged.AddListener((enabled) =>
            {
                if (enabled)
                {
                    LocalPrefs.SetBool("show_tussenuren", true);
                    weekRoosterView.showTussenUren();
                    dagRoosterView.showTussenUren();
                }
                else
                {
                    LocalPrefs.SetBool("show_tussenuren", false);
                }
                
                ViewManager.Instance.Refresh<NavBarView>();
            });
            
            //---
            
            minutes_before_class.valueText.text = LocalPrefs.GetInt("minutes_before_class", 1).ToString();
            int minutes = LocalPrefs.GetInt("minutes_before_class", 1);
            minutes_before_class.plusButton.onClick.AddListener(() =>
            {
                minutes++;
                LocalPrefs.SetInt("minutes_before_class", minutes);
                minutes_before_class.valueText.text = minutes.ToString();
            });
            minutes_before_class.minusButton.onClick.AddListener(() =>
            {
                minutes--;
                LocalPrefs.SetInt("minutes_before_class", minutes);
                minutes_before_class.valueText.text = minutes.ToString();
            });
            minutes_before_class.valueText.onValueChanged.AddListener((value) =>
            {
                if (int.TryParse(value, out int result))
                {
                    minutes = result;
                    LocalPrefs.SetInt("minutes_before_class", minutes);
                }
            });
            
            //---
                        
            homework_stays_till.valueText.text = LocalPrefs.GetInt("homework_stays_till", 1).ToString();
            int dagen = LocalPrefs.GetInt("homework_stays_till", 14);
            homework_stays_till.plusButton.onClick.AddListener(() =>
            {
                dagen++;
                LocalPrefs.SetInt("homework_stays_till", dagen);
                homework_stays_till.valueText.text = dagen.ToString();
            });
            homework_stays_till.minusButton.onClick.AddListener(() =>
            {
                dagen--;
                LocalPrefs.SetInt("homework_stays_till", dagen);
                homework_stays_till.valueText.text = dagen.ToString();
            });
            homework_stays_till.valueText.onValueChanged.AddListener((value) =>
            {
                if (int.TryParse(value, out int result))
                {
                    dagen = result;
                    LocalPrefs.SetInt("homework_stays_till", dagen);
                }
            });
            
            //---
            
            view_at_startup.value = LocalPrefs.GetInt("view_at_startup", 1);
            view_at_startup.onValueChanged.AddListener((value) =>
            {
                LocalPrefs.SetInt("view_at_startup", value);
            });

            base.Initialize();
        }
    }
}
