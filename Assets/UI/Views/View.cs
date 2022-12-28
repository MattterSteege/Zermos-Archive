using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
	[RequireComponent(typeof(Image))]
	[RequireComponent(typeof(Button))]
	public abstract class View : MonoBehaviour
	{
		[Header("View buttons"), SerializeField] public Button closeButtonWholePage;
		[SerializeField] public Button openNavigationButton;

		[Header("Own component fields"), HideInInspector] public bool isInitialized;
		[HideInInspector] public bool isVisible;
		public object args;
	
		public virtual void Initialize()
		{
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
	}
}
