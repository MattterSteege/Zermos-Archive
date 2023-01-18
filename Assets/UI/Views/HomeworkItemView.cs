using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class HomeworkItemView : View
    {
        private Homework.Item homeworkInfo;

        [SerializeField] TMP_Text Vak;
        [SerializeField] TMP_Text Datum;
        [SerializeField] TMP_Text Omschrijving;
        [SerializeField] Toggle Gemaakt;

        [SerializeField] private Button delete;
        [SerializeField] private CustomHomework _customHomework;

        [Space, SerializeField] private GameObject HuiswerkPill;
        [SerializeField] private GameObject ToetsPill;
        [SerializeField] private GameObject GroteToetsPill;

        [SerializeField] private GameObject BijlagePrefab;
        [SerializeField] private GameObject BijlageHolder;
        
        public override void Show(object args = null)
        {
            this.homeworkInfo = (Homework.Item) args;

            Refresh(args);

            base.Show();
        }

        public override void Initialize()
        {
            openNavigationButton.onClick.AddListener(() =>
            {
                ViewManager.Instance.ShowNewView<HomeworkView>();
            });

        
            if (homeworkInfo == null) return;
        
            delete.onClick.AddListener(() => DeleteHomework());

            delete.gameObject.SetActive(homeworkInfo.gemaakt);

            if (homeworkInfo == null) return;

            Vak.text = homeworkInfo.lesgroep.vak.naam ?? "";

            Datum.text = homeworkInfo.datumTijd.ToString("d MMMM");

            Omschrijving.text = homeworkInfo.studiewijzerItem.omschrijving;
            if (Omschrijving.text.Length == 0)
                Omschrijving.text = homeworkInfo.studiewijzerItem.onderwerp;
        
            Gemaakt.isOn = homeworkInfo.additionalObjects.swigemaaktVinkjes?.items?[0].gemaakt ?? false;

            if (homeworkInfo.studiewijzerItem.huiswerkType == "HUISWERK")
            {
                HuiswerkPill.SetActive(true); //
                ToetsPill.SetActive(false);
                GroteToetsPill.SetActive(false);
            }
            else if (homeworkInfo.studiewijzerItem.huiswerkType == "TOETS")
            {
                HuiswerkPill.SetActive(false);
                ToetsPill.SetActive(true); //
                GroteToetsPill.SetActive(false);
            }
            else if (homeworkInfo.studiewijzerItem.huiswerkType == "GROTE_TOETS")
            {
                HuiswerkPill.SetActive(false);
                ToetsPill.SetActive(false);
                GroteToetsPill.SetActive(true); //
            }
        
            var m1 = Regex.Matches(Omschrijving.text, @"(http|ftp|https):\/\/([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:\/~+#-]*[\w@?^=%&\/~+#-])");
        
            foreach (var result in m1)
            {
                Omschrijving.text = Omschrijving.text.Replace(result.ToString(), "<u><color=\"blue\"><link=" + result + ">" + result + "</link></color></u>");
            }

            if (homeworkInfo.studiewijzerItem.bijlagen?.Count > 0 )
            {
                BijlageHolder.transform.parent.gameObject.SetActive(false);
                Omschrijving.transform.parent.GetComponent<RectTransform>().offsetMin = new Vector2(25f, 15f);
            }
            else
            {
                Omschrijving.transform.parent.GetComponent<RectTransform>().offsetMin = new Vector2(25f, 180f);
                
                foreach (Transform child in BijlageHolder.transform)
                    Destroy(child.gameObject);

                BijlageHolder.transform.parent.gameObject.SetActive(true);
                for (int i = 0; i < homeworkInfo.studiewijzerItem.bijlagen.Count; i++)
                {
                    var go = Instantiate(BijlagePrefab, BijlageHolder.transform).GetComponent<BijlageInfo>();
                    go.setInfo(homeworkInfo.studiewijzerItem.bijlagen[i].omschrijving, (homeworkInfo.studiewijzerItem.bijlagen[i].assemblyResults[0].fileSize / 1024f).ToString("0.0") + " KB");
                    go.AddLink(homeworkInfo.studiewijzerItem.bijlagen[i].assemblyResults[0].fileUrl);
                }
            }
        }

        public override void Refresh(object args)
        {
            openNavigationButton.onClick.RemoveAllListeners();
            base.Refresh(args);
        }
        
        private void DeleteHomework()
        {
            if (homeworkInfo.gemaakt == true)
            {
                try
                {
                    _customHomework.DeleteCustomHomework(int.Parse(homeworkInfo.UUID));
                    ViewManager.Instance.Refresh<HomeworkView>();
                    ViewManager.Instance.ShowNewView<HomeworkView>();
                }
                catch(Exception){}
            }
        }

        //regex /^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$/g
    
    }
}