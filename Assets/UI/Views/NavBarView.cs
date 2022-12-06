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
		[SerializeField] private Button LeerlingBesprekingButton;
		[SerializeField] private Button SettingsButton;
		[SerializeField] private Image IconNotification;
		[SerializeField] private TMP_Text UsernameText;
	
		[SerializeField] private Grades gradesObject;
		[SerializeField] private AuthenticateSomtoday authenticateSomtodayObject;
		[SerializeField] private UpdateSystem updateSystem;

		public override void Initialize()
		{
			openNavigationButton.onClick.RemoveAllListeners();
			openNavigationButton.onClick.AddListener(() => ViewManager.Instance.HideNavigation());

			UsernameText.text = $"Hoi, <b>{LocalPrefs.GetString("zermelo-full_name").Split(" ")[0]}</b>";
		
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
					bool authenticated = authenticateSomtodayObject.checkToken();
					if (authenticated == false)
					{
						CijfersButton.gameObject.SetActive(false);
						HomeworkButton.gameObject.SetActive(false);
					}
					else
					{
						CijfersButton.gameObject.SetActive(true);
						HomeworkButton.gameObject.SetActive(true);
				
						CijfersButton.onClick.RemoveAllListeners();
						CijfersButton.onClick.AddListener(() =>
						{
							ViewManager.Instance.ShowNewView<GradeView>();
						});
					
						HomeworkButton.onClick.RemoveAllListeners();
						HomeworkButton.onClick.AddListener(() =>
						{
							ViewManager.Instance.ShowNewView<HomeworkView>();
						});
					}
				}
				else
				{
					CijfersButton.gameObject.SetActive(false);
					HomeworkButton.gameObject.SetActive(false);
				}

				if (LocalPrefs.GetBool("show_leermiddelen"))
				{
					LeermiddelenButton.gameObject.SetActive(true);
					LeermiddelenButton.onClick.RemoveAllListeners();
					LeermiddelenButton.onClick.AddListener(() =>
					{
						ViewManager.Instance.ShowNewView<LeermiddelenView>();
					});
				}
				else
				{
					LeermiddelenButton.gameObject.SetActive(false);
				}
			
				RoosterButton.gameObject.SetActive(true);
				SettingsButton.gameObject.SetActive(true);
				HomeButton.gameObject.SetActive(true);

				RoosterButton.onClick.RemoveAllListeners();
				RoosterButton.onClick.AddListener(() =>
				{
					ViewManager.Instance.ShowNewView<DagRoosterView>();
				});

				SettingsButton.onClick.RemoveAllListeners();
				SettingsButton.onClick.AddListener(() =>
				{
					ViewManager.Instance.ShowNewView<SettingsView>();
				});
#if UNITY_ANDROID
				int checkVoorUpdates = updateSystem.checkForUpdates();

				if (checkVoorUpdates == 1)
				{
					IconNotification.gameObject.SetActive(true);
				}
				else
#endif
				{
					IconNotification.gameObject.SetActive(false);
				}


				HomeButton.onClick.RemoveAllListeners();
				HomeButton.onClick.AddListener(() =>
				{
					ViewManager.Instance.ShowNewView<NewsAndInformationView>();
				});
				
				if (!string.IsNullOrEmpty(LocalPrefs.GetString("infowijs-access_token")))
				{
					SchoolNieuwsButton.gameObject.SetActive(true);
					SchoolNieuwsButton.onClick.RemoveAllListeners();
					SchoolNieuwsButton.onClick.AddListener(() =>
					{
						ViewManager.Instance.ShowNewView<SchoolNewsView>();
					});
				}
				else
				{
					SchoolNieuwsButton.gameObject.SetActive(false);
				}
				
				if (!string.IsNullOrEmpty(LocalPrefs.GetString("leerlingbespreking-access_token")))
				{
					LeerlingBesprekingButton.gameObject.SetActive(true);
					LeerlingBesprekingButton.onClick.RemoveAllListeners();
					LeerlingBesprekingButton.onClick.AddListener(() =>
					{
						ViewManager.Instance.ShowNewView<LeerlingbesprekingView>();
					});
				}
				else
				{
					LeerlingBesprekingButton.gameObject.SetActive(false);
				}
			}

			base.Initialize();
		}

		private void Update()
		{
			try
			{
				if (Input.GetKeyDown(KeyCode.Escape) && !ViewManager.Instance.isShowingNavigation)
				{
					ViewManager.Instance.ShowNavigation();
				}

				if (Input.GetKeyDown(KeyCode.Escape) && ViewManager.Instance.isShowingNavigation)
				{
					ViewManager.Instance.HideNavigation();
				}
			}
			catch (Exception) { }
		}
	}
}
