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

                foreach (Leermiddelen.Item vak in _leermiddelen.GetLeermiddelen().items ?? new List<Leermiddelen.Item>())
                {
                    GameObject leermiddel = Instantiate(leermiddelPrefab, content.transform);
                    leermiddel.GetComponent<LeermiddelenInfo>().SetLeermiddelenText(vak.description, vak.href);
                }
            }catch(Exception){}

            base.Initialize();
        }
    }
}
