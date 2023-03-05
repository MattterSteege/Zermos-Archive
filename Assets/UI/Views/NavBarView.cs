using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace UI.Views
{
	public class NavBarView : View
	{
		[SerializeField] private Button HomeButton;
		[SerializeField] private Button RoosterButton;
		[SerializeField] private Button HomeworkButton;
		[SerializeField] private Button CijfersButton;
		[SerializeField] private Button LeermiddelenButton;
		[SerializeField] private Button SchoolNieuwsButton;
		[SerializeField] private Button SchoolJaarKalenderButton;
		[SerializeField] private Button SettingsButton;
		[SerializeField] private RectTransform NavigationPanel;
		[SerializeField] private RectTransform FirstRow;
		float LayerHeight = 0f;

		private void Awake()
		{
			LayerHeight = FirstRow.rect.height;
		}

		public override void Initialize()
		{
			openNavigationButton.onClick.AddListener(() => ViewManager.Instance.HideNavigation());
			
			//UsernameText.text = $"Hoi, <b>{LocalPrefs.GetString("zermelo-full_name", "<br><size=12>hoe gaat het").Split(" ")[0]}</b>";

			if (string.IsNullOrEmpty(LocalPrefs.GetString("zermelo-access_token")))
			{
				RoosterButton.gameObject.SetActive(false);
				SettingsButton.gameObject.SetActive(false);
				HomeButton.gameObject.SetActive(false);
			}
			
			CheckButtons();

			SwipeDetector.onSwipeUp += (start, end) =>
			{
				ShowNavigation(start);
			};
			
			SwipeDetector.onSwipeDown += (start, end) =>
			{
				HideNavigation(start);
			};
			
			base.Initialize();
		}

		public void ShowNavigation(Touch start, bool force = false)
		{
			Vector2 touchPos = start.position;
			//touchPos.y = Screen.height - touchPos.y; // Flip Y coordinate to match UI coordinates
			touchPos -= (Vector2)FirstRow.position;
			touchPos /= FirstRow.lossyScale;

			// Get the highest Y value of the UI element in local space
			float highestY = FirstRow.rect.height / 2;

			// Check if touch is below the highest Y value of the UI element
			if (touchPos.y < highestY || force)
			{
				int childCount = NavigationPanel.childCount - 2;
				float spacing = NavigationPanel.GetComponent<VerticalLayoutGroup>().spacing;
				float HeightToMove = LayerHeight * childCount;
				HeightToMove += spacing * (childCount - 1);
				NavigationPanel.DOAnchorPosY(HeightToMove, force ? 0.01f : 0.5f);
			}
		}

		public void HideNavigation(Touch start, bool force = false)
		{
			Vector2 touchPos = start.position;
			//touchPos.y = Screen.height - touchPos.y; // Flip Y coordinate to match UI coordinates
			touchPos -= (Vector2)FirstRow.position;
			touchPos /= FirstRow.lossyScale;

			// Get the highest Y value of the UI element in local space
			float highestY = FirstRow.rect.height / 2;

			// Check if touch is below the highest Y value of the UI element
			if (touchPos.y < highestY || force)
			{
				NavigationPanel.DOAnchorPosY(LayerHeight, force ? 0.01f : 0.5f);
			}
		}


		private void CheckButtons()
		{
			LeermiddelenButton.gameObject.SetActive(false);
            RoosterButton.gameObject.SetActive(true);
            SettingsButton.gameObject.SetActive(true);
            HomeButton.gameObject.SetActive(true);
            RoosterButton.onClick.AddListener(() =>
            {
            	SwitchView.Instance.Show<DagRoosterView>();
            });
            SettingsButton.onClick.AddListener(() =>
            {
	            SwitchView.Instance.Show<SettingsView>();
            });

            HomeButton.onClick.AddListener(() =>
            {
            	SwitchView.Instance.Show<NewsAndInformationView>();
            });

            #region IfSomtoday
            if (!string.IsNullOrEmpty(LocalPrefs.GetString("somtoday-access_token")))
            {
            	CijfersButton.gameObject.SetActive(true);
            	HomeworkButton.gameObject.SetActive(true);
            	CijfersButton.onClick.AddListener(() =>
            	{
            		SwitchView.Instance.Show<GradeView>();
            	});
            	HomeworkButton.onClick.AddListener(() =>
            	{
	                SwitchView.Instance.Show<HomeworkView>();
            	});
            }
            else
            {
            	CijfersButton.gameObject.SetActive(false);
            	HomeworkButton.gameObject.SetActive(false);
            }
            #endregion
            #region IfAntoniusApp
            if (!string.IsNullOrEmpty(LocalPrefs.GetString("infowijs-access_token")))
            {
            	SchoolNieuwsButton.gameObject.SetActive(true);
            	SchoolNieuwsButton.onClick.AddListener(() =>
            	{
	                SwitchView.Instance.Show<SchoolNewsView>("Alle items worden nu ingeladen...");
            	});
            	
            	SchoolJaarKalenderButton.gameObject.SetActive(true);
            	SchoolJaarKalenderButton.onClick.AddListener(() =>
            	{
            		SwitchView.Instance.Show<JaarKalenderView>();
            	});
            }
            else
            {
            	SchoolNieuwsButton.gameObject.SetActive(false);
            	SchoolJaarKalenderButton.gameObject.SetActive(false);
            }
            #endregion
		}

		public override void Refresh(object args)
		{			
			openNavigationButton.onClick.RemoveAllListeners();
			CijfersButton.onClick.RemoveAllListeners();
			HomeworkButton.onClick.RemoveAllListeners();
			LeermiddelenButton.onClick.RemoveAllListeners();
			RoosterButton.onClick.RemoveAllListeners();
			SettingsButton.onClick.RemoveAllListeners();
			HomeButton.onClick.RemoveAllListeners();
			SchoolNieuwsButton.onClick.RemoveAllListeners();
			base.Refresh(args);
		}

		public override void Show(object args = null)
		{
			Debug.Log("Showing Navigation");
			gameObject.SetActive(true);
			base.Show(args);
		}

		public override void Hide()
		{
			string caller = new StackTrace().GetFrame(1).GetMethod().Name;
			if (caller != "MoveNext" && caller != "Show")
			{
				base.Hide();
			}
		}
	}
}
