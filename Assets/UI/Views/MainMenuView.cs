using UnityEngine;
using UnityEngine.UI;

public sealed class MainMenuView : View
{
	[SerializeField] private Button RoosterButton;
	[SerializeField] private Button LoginButton;
	[SerializeField] private Button userInfoButton;

	public override void Initialize()
	{

		if (PlayerPrefs.GetString("zermelo-access_token") == null ||
		    PlayerPrefs.GetString("zermelo-access_token") == "")
		{
			RoosterButton.gameObject.transform.parent.gameObject.SetActive(false);
			userInfoButton.gameObject.transform.parent.gameObject.SetActive(false);
			LoginButton.gameObject.transform.parent.gameObject.SetActive(true);
			
			LoginButton.onClick.AddListener(() =>
			{
				ViewManager.Instance.Show<ConnectZermeloView>();
			});
		}
		else
		{
			RoosterButton.gameObject.transform.parent.gameObject.SetActive(true);
			userInfoButton.gameObject.transform.parent.gameObject.SetActive(true);
			LoginButton.gameObject.transform.parent.gameObject.SetActive(false);
			
			RoosterButton.onClick.AddListener(() =>
			{
				ViewManager.Instance.Show<RoosterView, MainMenuView>();
			});
			
			userInfoButton.onClick.AddListener(() =>
			{
				ViewManager.Instance.Show<UserInfoView, MainMenuView>();
			});
		}

		base.Initialize();
	}
}
