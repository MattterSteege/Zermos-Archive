using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class LeermiddelenView : View
    {
        [SerializeField] Button AddleermiddelButton;
        [SerializeField] SubViewManager subViewManager;

        [SerializeField] private GameObject content;
        [SerializeField] private GameObject leermiddelPrefab;
        [SerializeField] private TMP_Text descriptionText;
    
        [SerializeField] private Leermiddelen _leermiddelen;
        [SerializeField] private CustomLeermiddelen _customLeermiddelen;
        [SerializeField] GameObject vakContainterPrefab;
        [SerializeField] GameObject vakNameDivider;

        public override void Initialize()
        {
            MonoBehaviour Mono = ViewManager.Instance.GetComponent<MonoBehaviour>();
            Mono.StartCoroutine(subViewManager.Initialize());
            
            try
            {
                AddleermiddelButton.onClick.AddListener(() =>
                {
                    subViewManager.ShowNewView<AddLeermiddelenView>();
                });

                var leermiddelen = _leermiddelen.GetLeermiddelen().items ?? new List<Leermiddelen.Item>();
                var customLeermiddelen = _customLeermiddelen.GetLeermiddelen() ?? new List<CustomLeermiddelen.Item>();
                
                if (leermiddelen.Count != 0 || customLeermiddelen.Count != 0) descriptionText.gameObject.SetActive(false);
                
                var unifiedLeermiddelen = leermiddelen.Concat(CastItemsWithoutId(customLeermiddelen)).ToList();
                
                unifiedLeermiddelen = unifiedLeermiddelen.OrderBy(x => x.vak).ToList();
                
                string vakName = "";
                GameObject vakContainer = null;

                foreach (Leermiddelen.Item vak in unifiedLeermiddelen)
                {
                    if (vak.vak != vakName || vakContainer == null)
                    {
                        vakContainer = Instantiate(vakContainterPrefab, content.transform);
                        var go = Instantiate(vakNameDivider, vakContainer.transform);
                        go.GetComponentInChildren<TMP_Text>().text = vak.vak;
                    }

                    GameObject leermiddel = Instantiate(leermiddelPrefab, vakContainer.transform);
                    leermiddel.GetComponent<LeermiddelenInfo>().SetLeermiddelenText(vak.description, vak.href);
                    vakName = vak.vak;
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
        
        public List<Leermiddelen.Item> CastItemsWithoutId(List<CustomLeermiddelen.Item> itemsWithId)
        {
            List<Leermiddelen.Item> itemsWithoutId = itemsWithId.Select(itemWithId =>
                new Leermiddelen.Item
                {
                    href = itemWithId.href,
                    description = itemWithId.description,
                    vak = itemWithId.vak
                }).ToList();
            return itemsWithoutId;
        }
    }
}
