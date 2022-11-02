using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
		CloseButtonWholePage.onClick.AddListener(() => ViewManager.Instance.HideNavigation());
		//openNavigationButton is niet nodig omdat het al de navigatie zelf is ;)
		
		UsernameText.text = $"Hoi, <b>{PlayerPrefs.GetString("zermelo-full_name").Split(" ")[0]}</b>";
		
		if (PlayerPrefs.GetString("zermelo-access_token") == null ||
		    PlayerPrefs.GetString("zermelo-access_token") == "")
		{
			RoosterButton.gameObject.transform.parent.gameObject.SetActive(false);
			SettingsButton.gameObject.transform.parent.gameObject.SetActive(false);
			HomeButton.gameObject.transform.parent.gameObject.SetActive(false);

		}
		else
		{
			if (!(PlayerPrefs.GetString("somtoday-access_token")==null ||
			    PlayerPrefs.GetString("somtoday-access_token")==""))
			{
				bool authenticated = authenticateSomtodayObject.checkToken();
				if (authenticated == false)
				{
					CijfersButton.gameObject.transform.parent.gameObject.SetActive(false);
					HomeworkButton.gameObject.transform.parent.gameObject.SetActive(false);
				}
				else
				{
					CijfersButton.gameObject.transform.parent.gameObject.SetActive(true);
					HomeworkButton.gameObject.transform.parent.gameObject.SetActive(true);
				
					CijfersButton.onClick.AddListener(() =>
					{
						ViewManager.Instance.Show<CijferView>();
					});
					
					HomeworkButton.onClick.AddListener(() =>
					{
						ViewManager.Instance.Show<HomeworkView>();
					});
				}
			}
			else
			{
				CijfersButton.gameObject.transform.parent.gameObject.SetActive(false);
				HomeworkButton.gameObject.transform.parent.gameObject.SetActive(false);
			}

			if (PlayerPrefs.GetString("SecretSettings", "0").Split(",")[0] == "1")
			{
				LeermiddelenButton.gameObject.transform.parent.gameObject.SetActive(true);
				LeermiddelenButton.onClick.AddListener(() =>
				{
					ViewManager.Instance.Show<LeermiddelenView, NavBarView>();
				});
			}
			else
			{
				LeermiddelenButton.gameObject.transform.parent.gameObject.SetActive(false);
			}
			
			RoosterButton.gameObject.transform.parent.gameObject.SetActive(true);
			SettingsButton.gameObject.transform.parent.gameObject.SetActive(true);
			HomeButton.gameObject.transform.parent.gameObject.SetActive(true);

			RoosterButton.onClick.AddListener(() =>
			{
				ViewManager.Instance.Show<DagRoosterView, NavBarView>();
			});

			SettingsButton.onClick.AddListener(() =>
			{
				ViewManager.Instance.Show<SettingsView, NavBarView>();
			});
			
			HomeButton.onClick.AddListener(() =>
			{
				ViewManager.Instance.Show<MainMenuView, NavBarView>();
			});
		}

		base.Initialize();
	}
	
	
}
