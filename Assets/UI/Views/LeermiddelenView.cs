using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class LeermiddelenView : View
    {
        [SerializeField] Button AddleermiddelButton;

        [SerializeField] private GameObject content;
        [SerializeField] private GameObject leermiddelPrefab;
    
        [SerializeField] private CustomLeermiddelen _leermiddelen;

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

            try
            {
                AddleermiddelButton.onClick.AddListener(() =>
                {
                    ViewManager.Instance.ShowNewView<AddLeermiddelenView>();
                });

                foreach (CustomLeermiddelen.LeermiddelenItem vak in GetLeermiddelenItems() ??
                                                                    new List<CustomLeermiddelen.LeermiddelenItem>())
                {
                    GameObject leermiddel = Instantiate(leermiddelPrefab, content.transform);
                    leermiddel.GetComponent<LeermiddelenInfo>().SetLeermiddelenText(vak.vak, vak.url);
                }
            }catch(Exception){}

            base.Initialize();
        }
    
        public List<CustomLeermiddelen.LeermiddelenItem> GetLeermiddelenItems()
        {
            string destination = _leermiddelen.savePath.Replace("*", Application.persistentDataPath);

            if (!File.Exists(destination))
            {
                Debug.LogWarning("File not found");
                return null;
            }

            using (StreamReader r = new StreamReader(destination))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<List<CustomLeermiddelen.LeermiddelenItem>>(json);
            }
        }
    }
}
