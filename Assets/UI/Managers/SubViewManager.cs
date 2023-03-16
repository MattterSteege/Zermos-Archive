using System;
using System.Collections;
using DG.Tweening;
using UI.Views;
using UnityEditor;
using UnityEngine;

public sealed class SubViewManager : MonoBehaviour
{
	public static SubViewManager Instance { get; private set; }
	[SerializeField] private SubView[] views;
	[SerializeField] private float animationTime = 0.5f;
	[Space, SerializeField] public SubView currentView;
	[Space, SerializeField] public bool ShowEventOnEveryView = false;

	private static GameObject ViewPrefab;
	
	private void Awake()
	{
		Instance = this;
	}

	public delegate void OnIntializeComplete(bool done = false);
	public static event OnIntializeComplete onInitializeComplete;
	
	public delegate void OnLoadedView(float loadingComplete = 0f, string viewName = "");
	public static event OnLoadedView onLoadedView;

	float viewsLoaded;
	public IEnumerator Initialize()
	{
		foreach (SubView view in views)
		{
			yield return new WaitForEndOfFrame();
			if (view != null)
			{
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
			onLoadedView?.Invoke(viewsLoaded, view.GetType ().Name);
		}
		HideParentView();
		
		onLoadedView?.Invoke(1f);
		onInitializeComplete?.Invoke(true);
	}
	
	[ContextMenu("Show parent view")]
	public void ShowParentView()
	{
		if (currentView == null) return;
		RectTransform rectTransform = currentView.GetComponent<RectTransform>();
		
		rectTransform.DOLocalMove(new Vector3(rectTransform.rect.width, -rectTransform.rect.height / 2f, 0f), animationTime * 2f);
	}

	[ContextMenu("Hide Navigation")]
	public void HideParentView()
	{
		if (currentView == null) return;
		RectTransform rectTransform = currentView.GetComponent<RectTransform>();
		
		//rectTransform.DOLocalMove(new Vector3(rectTransform.rect.width, -rectTransform.rect.height / 2f, 0f), 0.001f);
		rectTransform.DOLocalMove(new Vector3(-rectTransform.rect.width / 2f, -rectTransform.rect.height / 2f, 0f), animationTime * 2f);
	}
	
	public void ShowNewView<TView>(object args = null) where TView : SubView
	{
		
		foreach (SubView view in views)
		{
			if (view == currentView && view is TView && !ShowEventOnEveryView)
			{
				HideParentView();
				return;
			}
			
			// if(OpendCurrentViewFromView == view)
			// {
			// 	OpendCurrentViewFromView = null;
			// 	
			// 	RectTransform rectTransform = view.GetComponent<RectTransform>();
			// 	rectTransform.SetAsLastSibling();
			// 	rectTransform.DOLocalMove(new Vector3(rectTransform.rect.width, -rectTransform.rect.height / 2f, 0f), 0.001f);
			// 		
			// 	RectTransform currentRectTransform = currentView.GetComponent<RectTransform>();
			// 	currentRectTransform.SetAsLastSibling();
			// 	currentRectTransform.DOLocalMove(new Vector3(-currentRectTransform.rect.width / 2f, -currentRectTransform.rect.height / 2f, 0f), animationTime * 2f).onComplete += () =>
			// 	{
			// 		currentView.Hide();
			// 	};
			// 	
			// }
			
			if (view is TView)
			{
				currentView = view;	
				view.transform.SetAsLastSibling();
				
				RectTransform rectTransform = view.GetComponent<RectTransform>();
				
				rectTransform.DOLocalMove(new Vector3(rectTransform.rect.width, -rectTransform.rect.height / 2f, 0f), 0.001f);
				view.Show(args);

				rectTransform.DOLocalMove(new Vector3(-rectTransform.rect.width / 2f, -rectTransform.rect.height / 2f, 0f), animationTime * 2f).SetDelay(0.1f);
			}
		}
	}

	public void HideView<TView>() where TView : SubView
	{
		foreach (SubView view in views)
		{
			if (view is TView)
			{
				RectTransform currentRectTransform = view.GetComponent<RectTransform>();
				currentRectTransform.SetAsLastSibling();
				currentRectTransform.DOLocalMove(new Vector3(currentRectTransform.rect.width / 2f, -currentRectTransform.rect.height / 2f, 0f), animationTime * 2f).onComplete += () =>
				{
					view.Hide();
				};
			}
		}
	}

	public void Show<TView>(object args = null) where TView : SubView
	{
		foreach (SubView view in views)
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
	
	public void ShowNavbar<TView>(object args = null) where TView : SubView
	{
		foreach (SubView view in views)
		{
			if (view is TView)
			{
				view.Show(args);
			}
			else
			{
				view.Hide();
			}
		}
	}
	
	public void Show<TView, TView2>(object args = null) where TView : SubView
	{
		foreach (SubView view in views)
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
	
	public void Show<TView, TView2, TView3>(object args = null) where TView : SubView
	{
		foreach (SubView view in views)
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
	
	public void Hide<TView>(object args = null) where TView : SubView
	{
		foreach (SubView view in views)
		{
			if (view is TView)
			{
				view.Hide();
			}
		}
	}
	
	public void Hide<TView, TView2>(object args = null) where TView : SubView
	{
		foreach (SubView view in views)
		{
			if (view is TView || view is TView2)
			{
				view.Hide();
			}
		}
	}
	
	public void Hide<TView, TView2, TView3>(object args = null) where TView : SubView
	{
		foreach (SubView view in views)
		{
			if (view is TView || view is TView2 || view is TView3)
			{
				view.Hide();
			}
		}
	}
	
	public void Refresh<TView>(object args = null) where TView : SubView
	{
		foreach (SubView view in views)
		{
			if (view is TView)
			{
				view.Initialize();
			}
		}
	}

	public bool IsViewActive<TView>() where TView : SubView
	{
		foreach (SubView view in views)
		{
			if (view is TView)
			{
				return view.gameObject.activeSelf;
			}
		}

		return false;
	}
	
	
#if UNITY_EDITOR
	// Add a menu item to create custom GameObjects.
	// Priority 10 ensures it is grouped with the other menu items of the same kind
	// and propagated to the hierarchy dropdown and hierarchy context menus.
	[MenuItem("GameObject/UI/Sub View", false, 10)]
	static void AddCustomView(MenuCommand menuCommand)
	{
		// Create a custom game object
		GameObject go = Instantiate(Resources.Load("SubView")) as GameObject;
		// Ensure it gets reparented if this was a context click (otherwise does nothing)
		GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
		// Register the creation in the undo system
		Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
		Selection.activeObject = go;
	}
#endif
	
	public TView GetView<TView>() where TView : SubView
	{
		foreach (SubView view in views)
		{
			if (view is TView)
			{
				return view as TView;
			}
		}

		return null;
	}
}
