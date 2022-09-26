using UnityEngine;

public abstract class View : MonoBehaviour
{
	public bool IsInitialized { get; private set; }
	public object args { get; private set; }

	public virtual void Initialize()
	{
		IsInitialized = true;
	}

	public virtual void Show(object args = null)
	{
		this.args = args;
		gameObject.SetActive(true);
	}

	public virtual void Hide()
	{
		gameObject.SetActive(false);
	}
}
