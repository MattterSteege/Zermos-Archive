using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class LeermiddelenView : View
{
    [SerializeField] Button AddleermiddelButton;

    [SerializeField] private GameObject content;
    [SerializeField] private GameObject leermiddelPrefab;
    
    [SerializeField] private CustomLeermiddelen _leermiddelen;

    public override void Initialize()
    {
        AddleermiddelButton.onClick.AddListener(() => { ViewManager.Instance.Show<AddLeermiddelenView, NavBarView>(); });

        foreach (CustomLeermiddelen.LeermiddelenItem vak in GetLeermiddelenItems())
        {
            GameObject leermiddel = Instantiate(leermiddelPrefab, content.transform);
            leermiddel.GetComponent<LeermiddelenInfo>().SetLeermiddelenText(vak.vak, vak.url);
        }
        
        base.Initialize();
    }
    
    public List<CustomLeermiddelen.LeermiddelenItem> GetLeermiddelenItems()
    {
        string destination = _leermiddelen.savePath.Replace("*", Application.persistentDataPath);

        if (!File.Exists(destination))
        {
            Debug.LogError("File not found");
            return null;
        }

        using (StreamReader r = new StreamReader(destination))
        {
            string json = r.ReadToEnd();
            return JsonConvert.DeserializeObject<List<CustomLeermiddelen.LeermiddelenItem>>(json);
        }
    }
}
