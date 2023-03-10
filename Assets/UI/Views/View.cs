using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
	public abstract class View : MonoBehaviour
	{
		[Header("View buttons")]
		[SerializeField] public Button closeButtonWholePage;
		[SerializeField] public Button openNavigationButton;
		[SerializeField] public string viewName;

		[Header("Own component fields"), HideInInspector] public bool isInitialized;
		[HideInInspector] public bool isVisible;
		public object args = null;
		
		public object Instance;
	
		public virtual void Initialize()
		{
			Instance = this;
			isInitialized = true;
		}
		
		public virtual void Refresh(object args)
		{
			this.args = args;
			Initialize();
		}

		public virtual void Show(object args = null)
		{
			this.args = args;
			gameObject.SetActive(true);
			isVisible = true;
		}

		public virtual void Hide()
		{
			gameObject.SetActive(false);
			isVisible = false;
		}

		public virtual View GetInstance()
		{
			if (Instance == null)
				Instance = this;
			
			return Instance as View;
		}
		
		public virtual void SetInstance(View view)
		{
			Instance = view;
		}
	}
}
