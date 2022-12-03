using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public abstract class SubView : MonoBehaviour
    {
        [Header("View buttons"), SerializeField] public Button backButton;

        [HideInInspector] public bool isInitialized;
        [HideInInspector] public bool isVisible;
        private object _args;
	
        public virtual void Initialize()
        {
            backButton.onClick.AddListener(() =>
            {
                gameObject.GetComponentInParent<SubViewManager>().ShowParentView();
            });
            
            isInitialized = true;
        }
		
        public virtual void Refresh(object args)
        {
            Initialize();
            this._args = args;
        }

        public virtual void Show(object args = null)
        {
            this._args = args;
            gameObject.SetActive(true);
            isVisible = true;
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
            isVisible = false;
        }
    }
}
