using UnityEngine;

public abstract class View : MonoBehaviour
{
	public bool IsInitialized { get; private set; }
	public bool IsVisible { get; private set; }
	public object args { get; private set; }

	[ContextMenu("Refresh")]
	public virtual void Initialize()
	{
		IsInitialized = true;
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
