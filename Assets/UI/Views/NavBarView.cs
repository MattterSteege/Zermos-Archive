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
		[SerializeField] private Button SettingsButton;
		[SerializeField] private TMP_Text UsernameText;
	
		[SerializeField] private Grades gradesObject;
		[SerializeField] private AuthenticateSomtoday authenticateSomtodayObject;

		public override void Initialize()
		{
			openNavigationButton.onClick.RemoveAllListeners();
			openNavigationButton.onClick.AddListener(() => ViewManager.Instance.HideNavigation());

			UsernameText.text = $"Hoi, <b>{PlayerPrefs.GetString("zermelo-full_name").Split(" ")[0]}</b>";
		
			if (PlayerPrefs.GetString("zermelo-access_token") == null ||
			    PlayerPrefs.GetString("zermelo-access_token") == "")
			{
				RoosterButton.gameObject.SetActive(false);
				SettingsButton.gameObject.SetActive(false);
				HomeButton.gameObject.SetActive(false);

			}
			else
			{
				if (!(PlayerPrefs.GetString("somtoday-access_token")==null ||
				      PlayerPrefs.GetString("somtoday-access_token")==""))
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

				if (PlayerPrefs.GetString("SecretSettings", "0").ToCharArray()[0] == '1')
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
			
				HomeButton.onClick.RemoveAllListeners();
				HomeButton.onClick.AddListener(() =>
				{
					ViewManager.Instance.ShowNewView<NewsAndInformationView>();
				});
			}

			base.Initialize();
		}
	
	
	}
}
