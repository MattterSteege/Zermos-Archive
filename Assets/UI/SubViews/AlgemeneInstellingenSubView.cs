using UI.Views;
using UnityEngine;
using UnityEngine.UI;

namespace UI.SubViews
{
    public class AlgemeneInstellingenSubView : SubView
    {
        [SerializeField] Toggle show_tussenuren;
        [SerializeField] PlusMinusButtons minutes_before_class;

        [Space, SerializeField] WeekRoosterView weekRoosterView;
        [SerializeField] DagRoosterView dagRoosterView;
        
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
            
            
            
            base.Initialize();
        }
    }
}
