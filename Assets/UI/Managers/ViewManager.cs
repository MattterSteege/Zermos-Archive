using System;
using System.Collections;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public sealed class ViewManager : MonoBehaviour
{
	public static ViewManager Instance { get; private set; }
	
	[SerializeField] private bool autoInitialize;
	[SerializeField] private View Loginview;
	[SerializeField] private View[] views;
	[SerializeField] private View[] defaultViews;

	[SerializeField] private Image Background;
	[SerializeField] private float animationTime = 0.5f;
	
	[Space, SerializeField] public View currentView;
	[SerializeField] public View lastView;

	[Space, SerializeField] private static GameObject ViewPrefab;
	
	private void Awake()
	{
		Instance = this;
	}

	private IEnumerator Start()
	{
		yield return new WaitForEndOfFrame();
		if (autoInitialize) StartCoroutine(Initialize());
	}

	public delegate void OnIntializeComplete(bool done = false);
	public static event OnIntializeComplete onInitializeComplete;
	
	public delegate void OnLoadedView(float loadingComplete = 0f);
	public static event OnLoadedView onLoadedView;

	float viewsLoaded;
	public IEnumerator Initialize()
	{
		foreach (View view in views)
		{
			if (view != null)
			{
				yield return new WaitForEndOfFrame();
				
				try
				{
					view.Initialize();
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}

				view.Hide();
			}
			
			viewsLoaded += 1f / views.Length;
			onLoadedView?.Invoke(viewsLoaded);
		}

		if(string.IsNullOrEmpty(PlayerPrefs.GetString("zermelo-access_token")))
		{
			Loginview.Show();
			yield break;
		}
		
		if (defaultViews != null)
		{
			foreach (View view in defaultViews)
			{
				view.Show(null);
			}
		}
		
		currentView = defaultViews[0];
		
		HideNavigation();
		
		onLoadedView?.Invoke(1f);
		onInitializeComplete?.Invoke(true);
	}
	
	[ContextMenu("Show Navigation")]
	public void ShowNavigation()
	{
		currentView.CloseButtonWholePage.enabled = true;
		currentView.openNavigationButton.enabled = false;
		
		Background.DOColor(new Color(0.06666667f, 0.1529412f, 0.4352941f), animationTime);
		
		RectTransform rectTransform = currentView.GetComponent<RectTransform>();

		rectTransform.DOLocalMove(new Vector3(125f, -450f, 0f), animationTime);
		rectTransform.DOLocalRotate(new Vector3(0f, 0f, 8.3f), animationTime).WaitForCompletion();
		
	}

	[ContextMenu("Hide Navigation")]
	public void HideNavigation()
	{
		currentView.CloseButtonWholePage.enabled = false;
		currentView.openNavigationButton.enabled = true;
		
		RectTransform rectTransform = currentView.GetComponent<RectTransform>();
		
		rectTransform.DOLocalMove(new Vector3(-rectTransform.rect.width / 2f, -rectTransform.rect.height / 2f, 0f), animationTime);
		rectTransform.DOLocalRotate(new Vector3(0f, 0f, 0f), animationTime).WaitForCompletion();
		Background.DOColor(currentView.GetComponent<Image>().color, animationTime);
	}
	
	public void ShowNewView<TView>(object args = null) where TView : View
	{
		lastView = currentView;
		
		foreach (View view in views)
		{
			if (view == currentView && view is TView)
			{
				HideNavigation();
				return;
			}
			
			if (view is TView)
			{
				currentView = view;	
				view.transform.SetAsLastSibling();
				
				RectTransform rectTransform = view.GetComponent<RectTransform>();
				
				rectTransform.transform.position = new Vector3(Screen.width * 2.4f, -100f, 0f);
				rectTransform.transform.rotation = Quaternion.Euler(0f, 0f, 16.6f);
				view.Show(args);

				rectTransform.DOLocalMove(new Vector3(-rectTransform.rect.width / 2f, -rectTransform.rect.height / 2f, 0f), animationTime * 2f);
				rectTransform.DOLocalRotate(new Vector3(0f, 0f, 0f), animationTime * 2f).WaitForCompletion();
				Background.DOColor(view.GetComponent<Image>().color, animationTime * 2f);

				Invoke("HideLastView", animationTime * 2f);
			}
		}
	}
	private void HideLastView()
	{
		lastView.Hide();
	}

	public void HideView<TView>() where TView : View
	{
		View LastView = currentView;
		
		foreach (View view in views)
		{
			if (view is TView)
			{
				currentView = view;	
				view.transform.SetAsLastSibling();
				
				RectTransform rectTransform = view.GetComponent<RectTransform>();
				
				rectTransform.transform.position = new Vector3(-rectTransform.rect.width / 2f, -rectTransform.rect.height / 2f, 0f);
				rectTransform.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
				
				Background.DOColor(view.GetComponent<Image>().color, animationTime);
				
				new WaitForSeconds(animationTime);
				
				Invoke(nameof(changeView), animationTime);

				void changeView()
				{
					view.Hide();
				}
			}
		}
	}

	public void Show<TView>(object args = null) where TView : View
	{
		foreach (View view in views)
		{
			if (view is TView)
			{
				view.Show(args);
				currentView = view;
			}
			else
			{
				view.Hide();
			}
		}
	}
	
	public void Show<TView, TView2>(object args = null) where TView : View
	{
		foreach (View view in views)
		{
			if (view is TView || view is TView2)
			{
				view.Show(args);
			}
			else
			{
				view.Hide();
			}
		}
	}
	
	public void Show<TView, TView2, TView3>(object args = null) where TView : View
	{
		foreach (View view in views)
		{
			if (view is TView || view is TView2 || view is TView3)
			{
				view.Show(args);
			}
			else
			{
				view.Hide();
			}
		}
	}
	
	public void Hide<TView>(object args = null) where TView : View
	{
		foreach (View view in views)
		{
			if (view is TView)
			{
				view.Hide();
			}
		}
	}
	
	public void Hide<TView, TView2>(object args = null) where TView : View
	{
		foreach (View view in views)
		{
			if (view is TView || view is TView2)
			{
				view.Hide();
			}
		}
	}
	
	public void Hide<TView, TView2, TView3>(object args = null) where TView : View
	{
		foreach (View view in views)
		{
			if (view is TView || view is TView2 || view is TView3)
			{
				view.Hide();
			}
		}
	}
	
	public void Refresh<TView>(object args = null) where TView : View
	{
		foreach (View view in views)
		{
			if (view is TView)
			{
				view.Initialize();
			}
		}
	}

#if UNITY_EDITOR
	// Add a menu item to create custom GameObjects.
	// Priority 10 ensures it is grouped with the other menu items of the same kind
	// and propagated to the hierarchy dropdown and hierarchy context menus.
	[MenuItem("GameObject/UI/View", false, 10)]
	static void AddCustomView(MenuCommand menuCommand)
	{
		// Create a custom game object
		GameObject go = Instantiate(Resources.Load("View")) as GameObject;
		// Ensure it gets reparented if this was a context click (otherwise does nothing)
		GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
		// Register the creation in the undo system
		Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
		Selection.activeObject = go;
	}
#endif
}
