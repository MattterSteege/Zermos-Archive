using System;
using System.Collections;
using UnityEngine;

public sealed class ViewManager : MonoBehaviour
{
	public static ViewManager Instance { get; private set; }
	
	[SerializeField]
	private bool autoInitialize;

	[SerializeField]
	private View[] views;

	[SerializeField]
	private View[] defaultViews;
	
	[SerializeField]
	private View Loginview;
	
	private void Awake()
	{
		Instance = this;
	}

	private IEnumerator Start()
	{
		yield return new WaitForEndOfFrame();
		if (autoInitialize) StartCoroutine(Initialize());
	}

	public delegate void OnInitializeComplete();
	public static event OnInitializeComplete onInitializeComplete;
	
	public IEnumerator Initialize()
	{
		View BuggedView = null;
		
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
					BuggedView = view;
				}

				view.Hide();
			}
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
				if (BuggedView != view)
				{
					view.Show(null);
				}
			}
		}
		
		if (onInitializeComplete != null) onInitializeComplete();
	}

	public void Show<TView>(object args = null) where TView : View
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
}
