using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HomeworkView : View
{
    [SerializeField] Homework homeworkObject;    
    [SerializeField] GameObject homeworkPrefab;    
    [SerializeField] GameObject content;    
    [SerializeField] GameObject DividerPrefab;    
    
    //[SerializeField] private Button RefreshButton;
    
    public override void Initialize()
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
        
        //RefreshButton.onClick.AddListener(Initialize);
        List<Homework.Item> homework = homeworkObject.getHomework();
        
        //TODO: if date is different, create divider

        foreach (Homework.Item HomeworkItem in homework)
        {
            var homeworkItem = Instantiate(homeworkPrefab, content.transform);

            string onderwerp = HomeworkItem.studiewijzerItem.onderwerp;
            
            if (onderwerp.Length == 0)
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
                ViewManager.Instance.Show<HomeworkItemView, MainMenuView>(homeworkItem.GetComponent<HomeworkInfo>().homeworkInfo);
            });
            
            homeworkItem.GetComponent<HomeworkInfo>().gemaakt.onValueChanged.AddListener((bool isOn) =>
            {
                bool succesfull = UpdateGemaaktStatus(HomeworkItem, isOn);
                
                homeworkItem.GetComponent<HomeworkInfo>().gemaakt.isOn = succesfull;
            });
        }
        
        base.Initialize();
    }

    private bool UpdateGemaaktStatus(Homework.Item HomeworkItem, bool isOn)
    {
        string vinkje = ("{\"links\": [{\"rel\": \"self\"}],\"leerling\": {\"links\": [{\"id\": {0},\"rel\": \"self\"}]},\"swiToekenning\": {\"links\": [{\"id\": {1},\"rel\": \"koppeling\"}] },\"gemaakt\": {2} }")
            .Replace("{0}", HomeworkItem.additionalObjects.leerlingen.items[0].links[0].id.ToString())
            .Replace("{1}", HomeworkItem.studiewijzerItem.links[0].id.ToString())
            .Replace("{2}", isOn.ToString().ToLower());
        
        
        string baseurl = string.Format($"{PlayerPrefs.GetString("somtoday-api_url")}/rest/v1/swigemaakt/{HomeworkItem.studiewijzerItem.links[0].id}");

        UnityWebRequest www = UnityWebRequest.Put(baseurl, vinkje);
        www.SetRequestHeader("authorization", "Bearer " + PlayerPrefs.GetString("somtoday-access_token"));
        www.SetRequestHeader("Accept", "application/json");
        www.SendWebRequest();

        while (!www.isDone)
        {
        }

        if (www.result == UnityWebRequest.Result.Success)
        {
            bool success = JsonUtility.FromJson<Gemaakt>(www.downloadHandler.text).gemaakt;

            www.Dispose();
            return success;
        }

        www.Dispose();
        return false;
    }

    #region gemaakt
    public class Gemaakt
    {
        public bool gemaakt { get; set; }
    }
    #endregion
}
