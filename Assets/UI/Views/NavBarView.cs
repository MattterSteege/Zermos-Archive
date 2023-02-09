using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
	public sealed class NavBarView : View
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

		public override void Initialize()
		{
			openNavigationButton.onClick.AddListener(() => ViewManager.Instance.HideNavigation());

			UsernameText.text = $"Hoi, <b>{LocalPrefs.GetString("zermelo-full_name", "<br><size=12>hoe gaat het").Split(" ")[0]}</b>";
		
			if (string.IsNullOrEmpty(LocalPrefs.GetString("zermelo-access_token")))
			{
				RoosterButton.gameObject.SetActive(false);
				SettingsButton.gameObject.SetActive(false);
				HomeButton.gameObject.SetActive(false);

			}
			else
			{
				if (!string.IsNullOrEmpty(LocalPrefs.GetString("somtoday-access_token")))
				{
					CijfersButton.gameObject.SetActive(true);
					HomeworkButton.gameObject.SetActive(true);
					CijfersButton.onClick.AddListener(() =>
					{
						ViewManager.Instance.ShowNewView<GradeView>();
					});
					HomeworkButton.onClick.AddListener(() =>
					{
						ViewManager.Instance.ShowNewView<HomeworkView>();
					});
				}
				else
				{
					CijfersButton.gameObject.SetActive(false);
					HomeworkButton.gameObject.SetActive(false);
				}

				// if (LocalPrefs.GetBool("show_leermiddelen"))
				// {
				// 	LeermiddelenButton.gameObject.SetActive(true);
				// 	LeermiddelenButton.onClick.AddListener(() =>
				// 	{
				// 		ViewManager.Instance.ShowNewView<LeermiddelenView>();
				// 	});
				// }
				// else
				// {
					LeermiddelenButton.gameObject.SetActive(false);
				//}
			
				RoosterButton.gameObject.SetActive(true);
				SettingsButton.gameObject.SetActive(true);
				HomeButton.gameObject.SetActive(true);
				RoosterButton.onClick.AddListener(() =>
				{
					ViewManager.Instance.ShowNewView<DagRoosterView>();
				});
				SettingsButton.onClick.AddListener(() =>
				{
					ViewManager.Instance.ShowNewView<SettingsView>();
				});

				HomeButton.onClick.AddListener(() =>
				{
					ViewManager.Instance.ShowNewView<NewsAndInformationView>();
				});
				
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
			}
			
			base.Initialize();
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

		public override void Hide()
		{
			//do nothing when hide is called on this view
		}
	}
}
