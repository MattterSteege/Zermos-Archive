using UnityEngine;
using UnityEngine.UI;

public sealed class NavBarView : View
{
	[SerializeField] private Button RoosterButton;
	[SerializeField] private Button CijfersButton;
	[SerializeField] private Button HomeworkButton;
	[SerializeField] private Button LoginButton;
	[SerializeField] private Button SettingsButton;
	[SerializeField] private Button HomeButton;
	[SerializeField] private Grades gradesObject;
	[SerializeField] private AuthenticateSomtoday authenticateSomtodayObject;

	public override void Initialize()
	{

		if (PlayerPrefs.GetString("zermelo-access_token") == null ||
		    PlayerPrefs.GetString("zermelo-access_token") == "")
		{
			RoosterButton.gameObject.transform.parent.gameObject.SetActive(false);
			SettingsButton.gameObject.transform.parent.gameObject.SetActive(false);
			HomeButton.gameObject.transform.parent.gameObject.SetActive(false);
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
						ViewManager.Instance.Show<CijferView, NavBarView>();
					});
					
					HomeworkButton.onClick.AddListener(() =>
					{
						ViewManager.Instance.Show<HomeworkView, NavBarView>();
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
			HomeButton.gameObject.transform.parent.gameObject.SetActive(true);
			LoginButton.gameObject.transform.parent.gameObject.SetActive(false);
			
			RoosterButton.onClick.AddListener(() =>
			{
				ViewManager.Instance.Show<DagRoosterView, NavBarView>();
			});
			
			SettingsButton.onClick.AddListener(() =>
			{
				ViewManager.Instance.Show<SettingsView, NavBarView>();
			});
		}

		base.Initialize();
	}
}
