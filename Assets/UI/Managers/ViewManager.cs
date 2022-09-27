using System;
using UnityEngine;

public sealed class ViewManager : MonoBehaviour
{
	public static ViewManager Instance { get; private set; }
	
	public delegate void OnLoad();

	public event OnLoad onLoad;

	private void RaiseOnLoad()
	{
		if (onLoad != null)
		{
			onLoad();
		}
	}

	[SerializeField]
	private bool autoInitialize;

	[SerializeField]
	private View[] views;

	[SerializeField]
	private View[] defaultViews;
	
	[SerializeField]
	private View LoginView;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		if (autoInitialize) Initialize();
	}

	public void Initialize()
	{
		foreach (View view in views)
		{
			view.Initialize();

			view.Hide();
		}

		if (defaultViews != null)
		{
			foreach (View view in defaultViews)
			{
				view.Show(null);
			}
		}
		
		if (PlayerPrefs.GetString("zermelo-access_token") == null ||
		    PlayerPrefs.GetString("zermelo-access_token") == "")
		{
			foreach (View view in views)
			{
				view.Initialize();

				view.Hide();
			}
			
			LoginView.Show();
		}
	}

	public void Show<TView>(object args = null) where TView : View
	{
		foreach (View view in views)
		{
			if (view is TView)
			{
				RaiseOnLoad();
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
				RaiseOnLoad();
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
				RaiseOnLoad();
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
