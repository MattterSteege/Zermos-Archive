using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public abstract class View : MonoBehaviour
{
	[SerializeField] public Button CloseButtonWholePage;
	[SerializeField] public Button openNavigationButton;
	
	public bool IsInitialized { get; private set; }
	public bool IsVisible { get; private set; }
	public object args { get; private set; }
	
	public virtual void Initialize()
	{
		IsInitialized = true;
	}
	
	[ContextMenu("Refresh")]
	public virtual void Refresh(object args)
	{
		Initialize();
		this.args = args;
	}

	public virtual void Show(object args = null)
	{
		this.args = args;
		gameObject.SetActive(true);
		IsVisible = true;
	}

	public virtual void Hide()
	{
		gameObject.SetActive(false);
		IsVisible = false;
	}
}
