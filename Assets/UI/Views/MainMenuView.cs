using UnityEngine;
using UnityEngine.UI;

public sealed class MainMenuView : View
{
	[SerializeField] private Button RoosterButton;
	[SerializeField] private Button HomeworkButton;
	[SerializeField] private Button LoginButton;
	[SerializeField] private Button SettingsButton;

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
			RoosterButton.gameObject.transform.parent.gameObject.SetActive(true);
			SettingsButton.gameObject.transform.parent.gameObject.SetActive(true);
			LoginButton.gameObject.transform.parent.gameObject.SetActive(false);
			
			RoosterButton.onClick.AddListener(() =>
			{
				ViewManager.Instance.Show<RoosterView, MainMenuView>();
			});
			
			SettingsButton.onClick.AddListener(() =>
			{
				ViewManager.Instance.Show<SettingsView, MainMenuView>();
			});
		}

		base.Initialize();
	}
}
