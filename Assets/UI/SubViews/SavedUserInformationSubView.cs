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

        public override void Initialize()
        {
            backButton.onClick.AddListener(() =>
            {
                gameObject.GetComponentInParent<SubViewManager>().ShowParentView();
            });
            
            
            Dictionary<string, string> savedData = new Dictionary<string, string>();
            var keys = LocalPrefs.AllKeys<string>();
            var values = LocalPrefs.AllValues<string>();
            for (int i = 0; i < keys.Length; i++)
            {
                savedData.Add(keys[i], values[i]);
                //Debug.Log(keys[i] + " : " + values[i]);
            }
            
            

            base.Initialize();
        }
    }
}
