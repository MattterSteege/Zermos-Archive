using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class LeermiddelenView : View
    {
        [SerializeField] Button AddleermiddelButton;

        [SerializeField] private GameObject content;
        [SerializeField] private GameObject leermiddelPrefab;
        [SerializeField] private TMP_Text descriptionText;
    
        [SerializeField] private Leermiddelen _leermiddelen;

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

                var leermiddelen = _leermiddelen.GetLeermiddelen().items ?? new List<Leermiddelen.Item>();
                
                if (leermiddelen.Count != 0) descriptionText.gameObject.SetActive(false);
                
                foreach (Leermiddelen.Item vak in leermiddelen)
                {
                    GameObject leermiddel = Instantiate(leermiddelPrefab, content.transform);
                    leermiddel.GetComponent<LeermiddelenInfo>().SetLeermiddelenText(vak.description, vak.href);
                }
            }catch(Exception){}

            base.Initialize();
        }
        
        public override void Refresh(object args)
        {
            openNavigationButton.onClick.RemoveAllListeners();
            closeButtonWholePage.onClick.RemoveAllListeners();
            AddleermiddelButton.onClick.RemoveAllListeners();
            base.Refresh(args);
        }
    }
}
