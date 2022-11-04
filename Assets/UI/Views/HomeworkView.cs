using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HomeworkView : View
{
    [SerializeField] Homework homeworkObject;    
    [SerializeField] GameObject homeworkPrefab;    
    [SerializeField] GameObject content;    
    [SerializeField] GameObject DividerPrefab;    
    
    [SerializeField] private CustomHomework _CustomHomework;
    
    [SerializeField] private Button _AddHomeworkButton;
    
    //[SerializeField] private Button RefreshButton;
    
    public override void Initialize()
    {
        openNavigationButton.onClick.AddListener(() =>
        {
            openNavigationButton.enabled = false;
            ViewManager.Instance.ShowNavigation();
        });
        
        CloseButtonWholePage.onClick.AddListener(() =>
        {
            openNavigationButton.enabled = true;
            ViewManager.Instance.HideNavigation();
        });
        
        _AddHomeworkButton.onClick.AddListener(() => ViewManager.Instance.Show<AddHomeworkView, NavBarView>());

        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
        
        //RefreshButton.onClick.AddListener(Initialize);
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
                go.GetComponent<homeworkDivider>().Datum.text = ((DateTimeOffset) HomeworkItem.datumTijd).DateTime.ToString("d MMMM");
            }
            if (HomeworkItem.datumTijd.Day < day)
            {
                var go = Instantiate(DividerPrefab, content.transform);
                go.GetComponent<homeworkDivider>().Datum.text = ((DateTimeOffset) HomeworkItem.datumTijd).DateTime.ToString("d MMMM");
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
                ViewManager.Instance.Show<HomeworkItemView, NavBarView>(homeworkItem.GetComponent<HomeworkInfo>().homeworkInfo);
            });
            
            homeworkItem.GetComponent<HomeworkInfo>().gemaakt.onValueChanged.AddListener((bool isOn) =>
            {
                if (HomeworkItem.gemaakt == false)
                {
                    bool succesfull = UpdateGemaaktStatus(HomeworkItem, isOn);
                    
                    homeworkItem.GetComponent<HomeworkInfo>().gemaakt.SetIsOnWithoutNotify(succesfull);
                }
                else
                {
                    _CustomHomework.SetCustomHomework(int.Parse(HomeworkItem.UUID), HomeworkItem.lesgroep.vak.naam, HomeworkItem.studiewijzerItem.omschrijving, HomeworkItem.datumTijd, isOn);
                }
            });

            
            day = HomeworkItem.datumTijd.Day;
        }
        
        base.Initialize();
    }

    private bool UpdateGemaaktStatus(Homework.Item HomeworkItem, bool gemaakt)
    {
        SomtodayHoweworkStatus root = new SomtodayHoweworkStatus
        {
            leerling = new Leerling
            {
                links = new List<Link>
                {
                    new()
                    {
                        id = int.Parse(HomeworkItem.additionalObjects.leerlingen.items[0].links[0].id.ToString()),
                        rel = "self",
                        href = $"{PlayerPrefs.GetString("somtoday-api_url")}/rest/v1/leerlingen/{HomeworkItem.additionalObjects.leerlingen.items[0].links[0].id}"
                    }
                }
            },
            gemaakt = gemaakt
        };

        //root to a json string
        string json = JsonConvert.SerializeObject(root);
        
        UnityWebRequest www = UnityWebRequest.Put($"{PlayerPrefs.GetString("somtoday-api_url")}/rest/v1/swigemaakt/{HomeworkItem.additionalObjects.swigemaaktVinkjes.items[0].links[0].id}", json);
        
        www.SetRequestHeader("authorization", "Bearer " + PlayerPrefs.GetString("somtoday-access_token"));
        
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
