using UnityEngine;
using UnityEngine.UI;

public sealed class MainMenuView : View
{
	[SerializeField] private Button RoosterButton;
	[SerializeField] private Button CijfersButton;
	[SerializeField] private Button HomeworkButton;
	[SerializeField] private Button LoginButton;
	[SerializeField] private Button SettingsButton;
	[SerializeField] private Grades gradesObject;
	[SerializeField] private AuthenticateSomtoday authenticateSomtodayObject;

	public override void Initialize()
	{

		if (PlayerPrefs.GetString("zermelo-access_token") == null ||
		    PlayerPrefs.GetString("zermelo-access_token") == "")
		{
			RoosterButton.gameObject.transform.parent.gameObject.SetActive(false);
			SettingsButton.gameObject.transform.parent.gameObject.SetActive(false);
			LoginButton.gameObject.transform.parent.gameObject.SetActive(true);
			
			LoginButton.onClick.AddListener(() =>
			{
				ViewManager.Instance.Show<ConnectZermeloView>();
			});
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
						ViewManager.Instance.Show<CijferView, MainMenuView>();
					});
					
					HomeworkButton.onClick.AddListener(() =>
					{
						ViewManager.Instance.Show<HomeworkView, MainMenuView>();
					});
				}
			}
			else
			{
				CijfersButton.gameObject.transform.parent.gameObject.SetActive(false);
				HomeworkButton.gameObject.transform.parent.gameObject.SetActive(false);
			}
			
			RoosterButton.gameObject.transform.parent.gameObject.SetActive(true);
			SettingsButton.gameObject.transform.parent.gameObject.SetActive(true);
			LoginButton.gameObject.transform.parent.gameObject.SetActive(false);
			
			RoosterButton.onClick.AddListener(() =>
			{
				ViewManager.Instance.Show<DagRoosterView, MainMenuView>();
			});
			
			SettingsButton.onClick.AddListener(() =>
			{
				ViewManager.Instance.Show<SettingsView, MainMenuView>();
			});
		}

		base.Initialize();
	}
}
