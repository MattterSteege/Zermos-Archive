using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
		[SerializeField] private TMP_Text UsernameText;
		[SerializeField] private RectTransform NavigationPanel;
		[SerializeField] private RectTransform FirstRow;
		[SerializeField] private RectTransform contentRectTransform;
		private Camera _camera;
		float LayerHeight = 0f;

		private void Awake()
		{
			_camera = Camera.main;
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
				ShowNavigation(start, end);
			};
			
			SwipeDetector.onSwipeDown += (start, end) =>
			{
				HideNavigation(start, end);
			};
			
			base.Initialize();
		}

		private void ShowNavigation(Touch start, Touch end)
		{
			Vector2 touchPos = start.position;
			//touchPos.y = Screen.height - touchPos.y; // Flip Y coordinate to match UI coordinates
			touchPos -= (Vector2)FirstRow.position;
			touchPos /= FirstRow.lossyScale;

			// Get the highest Y value of the UI element in local space
			float highestY = FirstRow.rect.height / 2;

			// Check if touch is below the highest Y value of the UI element
			if (touchPos.y < highestY)
			{
				int childCount = NavigationPanel.childCount - 2;
				float spacing = NavigationPanel.GetComponent<VerticalLayoutGroup>().spacing;
				float HeightToMove = LayerHeight * childCount;
				HeightToMove += spacing * (childCount - 1);
				NavigationPanel.DOAnchorPosY(HeightToMove, 0.5f);
			}
		}

		private void HideNavigation(Touch start, Touch end)
		{
			Vector2 touchPos = start.position;
			//touchPos.y = Screen.height - touchPos.y; // Flip Y coordinate to match UI coordinates
			touchPos -= (Vector2)FirstRow.position;
			touchPos /= FirstRow.lossyScale;

			// Get the highest Y value of the UI element in local space
			float highestY = FirstRow.rect.height / 2;

			// Check if touch is below the highest Y value of the UI element
			if (touchPos.y < highestY)
			{
				NavigationPanel.DOAnchorPosY(LayerHeight, 0.5f);
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
            	ViewManager.Instance.ShowNewView<SettingsView>();
            });

            HomeButton.onClick.AddListener(() =>
            {
            	ViewManager.Instance.ShowNewView<NewsAndInformationView>();
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
            		ViewManager.Instance.ShowNewView<SchoolNewsView>();
            	});
            	
            	SchoolJaarKalenderButton.gameObject.SetActive(true);
            	SchoolJaarKalenderButton.onClick.AddListener(() =>
            	{
            		ViewManager.Instance.ShowNewView<JaarKalenderView>();
            	});
            }
            else
            {
            	SchoolNieuwsButton.gameObject.SetActive(false);
            	SchoolJaarKalenderButton.gameObject.SetActive(false);
            }
            #endregion
		}

		public override void Hide()
		{
			Debug.Log("You can't hide me! I'm the NavBar! :D");	
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
	}
}
