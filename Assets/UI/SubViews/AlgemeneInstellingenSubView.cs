using UI.Views;
using UnityEngine;

namespace UI.SubViews
{
    public class AlgemeneInstellingenSubView : SubView
    {
        public override void Initialize()
        {
            backButton.onClick.AddListener(() =>
            {
                gameObject.GetComponentInParent<SubViewManager>().ShowParentView();
            });
            
            base.Initialize();
        }
    }
}
