using System.Collections.Generic;
using TMPro;
using UI.Views;
using UnityEngine;
using UnityEngine.UI;

namespace UI.SubViews
{
    public class SavedUserInformationSubView : SubView
    {
        [SerializeField] GameObject content;
        [SerializeField] GameObject prefab;

        public override void Initialize()
        {
            backButton.onClick.AddListener(() =>
            {
                gameObject.GetComponentInParent<SubViewManager>().ShowParentView();
            });
            
            foreach (Transform child in content.transform)
                Destroy(child.gameObject);
            

            Dictionary<string, string> savedData = new Dictionary<string, string>();
            var keys = LocalPrefs.AllKeys<string>();
            var values = LocalPrefs.AllValues<string>();
            for (int i = 0; i < keys.Length; i++)
            {
                savedData.Add(keys[i], values[i]);
            }
            
            foreach (var data in savedData)
            {
                var item = Instantiate(prefab, content.transform);
                item.GetComponent<SavedUserInformationItem>().SetData(data.Key, data.Value);
            }

            base.Initialize();
        }
    }
}
