using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DG.Tweening;
using UI.Views;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public sealed class ViewManager : MonoBehaviour
{
	public static ViewManager Instance { get; private set; }
	
	[SerializeField] private bool autoInitialize;
	[SerializeField] public bool isShowingNavigation;
	[SerializeField] private View Loginview;
	[SerializeField] private View NewUserView;
	[SerializeField] public View[] views;
	[SerializeField] private View[] defaultViews;
	[SerializeField] private View[] view_at_startup;

	[SerializeField] private Image Background;
	[SerializeField] private float animationTime = 0.5f;
	[SerializeField] private bool saveLoadingTimes = false;
	
	[Space, SerializeField] public View currentView;
	[SerializeField] public View lastView;

	[Space] private static GameObject ViewPrefab;
	[Space, SerializeField] private SubView secretSettingsSubView;

	private void Awake()
	{
		Instance = this;
		secretSettingsSubView.Show();
		LocalPrefs.Load();
	}

	private IEnumerator Start()
	{
		yield return new WaitForEndOfFrame();

		if (autoInitialize)
		{
			beforeInitialize?.Invoke();
			StartCoroutine(Initialize());
		}
	}

	public delegate void BeforeInitialize();
	public static event BeforeInitialize beforeInitialize;
	
	public delegate void OnIntializeComplete(bool done = false);
	public static event OnIntializeComplete onInitializeComplete;
	
	public delegate void OnLoadedView(float loadingComplete = 0f, string viewName = "");
	public static event OnLoadedView onLoadedView;

	float viewsLoaded;
	Stopwatch _timer;
	float passedTime;
	List<float> times = new List<float>();
	public IEnumerator Initialize()
	{
		_timer = new Stopwatch();
		foreach (View view in views)
		{
			_timer.Start();
			if (view != null)
			{
				yield return new WaitForEndOfFrame();
				
				try
				{
					view.Initialize();
					view.SetInstance(view);
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}

				view.Hide();
			}
			
			viewsLoaded += 1f / views.Length;
			onLoadedView?.Invoke(viewsLoaded, view.GetType().Name);
			_timer.Stop();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
			if (saveLoadingTimes)
				Debug.Log(view.GetType ().Name + " at " + viewsLoaded.ToString("P0") + " - time passed: " + (float) Math.Round((_timer.ElapsedMilliseconds / 1000f) - passedTime, 3));
			times.Add((float) Math.Round((_timer.ElapsedMilliseconds / 1000f) - passedTime, 3));
			passedTime = _timer.ElapsedMilliseconds / 1000f;
#endif
		}
		


		var viewToShow = view_at_startup[LocalPrefs.GetInt("view_at_startup", 0)];

		if (!defaultViews.Contains(viewToShow))
		{
			List<View> defaultViewsList = defaultViews.ToList();
			defaultViewsList.Add(viewToShow);
			defaultViews = defaultViewsList.ToArray();
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
		
		if(string.IsNullOrEmpty(LocalPrefs.GetString("zermelo-access_token")))
		{
			Loginview.Show();
			yield break;
		}
		
		// if(LocalPrefs.GetBool("first_time", false) == false)
		// {
		// 	NewUserView.Show();
		// 	yield break;
		// }
		
		if(LocalPrefs.GetString("what_new_version", "0") != Application.version)
		{
			Show<WhatsNew>();
		}
		
		onLoadedView?.Invoke(1f);
		onInitializeComplete?.Invoke(true);

		
#if UNITY_EDITOR
		if (saveLoadingTimes == true)		
		{
			try
			{
				using (StreamWriter sw =
				       new StreamWriter(
					       @"C:\Users\mattt\AppData\Roaming\Unity\Games\Zermelo+Somtoday\Assets\LoadingTimes.csv", true))
				{
					times.Add((float) Math.Round(_timer.ElapsedMilliseconds / 1000f, 3));
					sw.WriteLine(string.Join(";", times));
				}
			}
			catch(Exception) { }
		}
#endif
	}

	[ContextMenu("Show Navigation")]
	public void ShowNavigation()
	{
		if (currentView.closeButtonWholePage != null)
			currentView.closeButtonWholePage.enabled = true;
		if(currentView.openNavigationButton != null)
			currentView.openNavigationButton.enabled = false;
		
		RectTransform rectTransform = currentView.GetComponent<RectTransform>();
		

		rectTransform.DOLocalMove(new Vector3(125f, -450f, 0f), animationTime);
		rectTransform.DOLocalRotate(new Vector3(0f, 0f, 8.3f), animationTime);
		Background.DOColor(new Color(0.06666667f, 0.1529412f, 0.4352941f), animationTime).onComplete += () =>
		{
			isShowingNavigation = true;
		};
		
	}

	[ContextMenu("Hide Navigation")]
	public void HideNavigation()
	{
		if (currentView.closeButtonWholePage != null)
			currentView.closeButtonWholePage.enabled = false;
		if(currentView.openNavigationButton != null)
			currentView.openNavigationButton.enabled = true;
		
		RectTransform rectTransform = currentView.GetComponent<RectTransform>();
		
		rectTransform.DOLocalMove(new Vector3(-rectTransform.rect.width / 2f, -rectTransform.rect.height / 2f, 0f), animationTime);
		rectTransform.DOLocalRotate(new Vector3(0f, 0f, 0f), animationTime).WaitForCompletion();
		Background.DOColor(currentView.GetComponent<Image>()?.color ?? Color.white, animationTime).onComplete += () =>
		{
			isShowingNavigation = false;
		};
	}
	
	public void ShowNewView<TView>(object args = null) where TView : View
	{
		foreach (View view in views)
		{
			if (view == currentView && view is TView)
				return;
			
			
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

	public void HideView<TView>() where TView : View
	{
		foreach (View view in views)
		{
			if (view is TView)
			{
				view.transform.SetAsLastSibling();
				
				RectTransform rectTransform = view.GetComponent<RectTransform>();
				
				rectTransform.transform.position = new Vector3(rectTransform.rect.width, 0f, 0f);

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
	
	public void ShowNavbar<TView>(object args = null) where TView : View
	{
		foreach (View view in views)
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
	
	public TView GetInstance<TView>() where TView : View
	{
		foreach (View view in views)
		{
			if (view is TView)
			{
				return (TView) view.GetInstance().Instance;
			}
		}
		
		return null;
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
	
	public bool IsViewActive<TView>() where TView : View
	{
		foreach (View view in views)
		{
			if (view is TView)
			{
				return view.gameObject.activeSelf;
			}
		}

		return false;
	}

	public void HideCurrentView()
	{
		currentView.Hide();
	}
}
