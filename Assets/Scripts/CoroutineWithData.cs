using System.Collections;
using UnityEngine;


public class CoroutineWithData<T>
{
    private IEnumerator _target;
    public T result;
    private Coroutine Coroutine { get; set; }

    public CoroutineWithData(MonoBehaviour owner_, IEnumerator target_)
    {
        _target = target_;
        Coroutine = owner_.StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        while (_target.MoveNext())
        {
            if (_target.Current != null && _target.Current is not WaitForSeconds)
            {
                result = (T) _target.Current;
                yield return result;
            }
        }
    }
}