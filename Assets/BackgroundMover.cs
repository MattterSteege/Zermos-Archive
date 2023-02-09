using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BackgroundMover : MonoBehaviour
{
    //make an UI recttransform move randomly and smoothly in the background
    //do this WITH the anchors being streched to the edges of the screen
    
    public RectTransform rect;
    public float speed = 1;
    public float waitTime = 1;
    public Vector2 MaxMin;
    private Vector2 targetPos;
    
    void Start()
    {
        targetPos = new Vector2(Random.Range(MaxMin.x, MaxMin.y), Random.Range(MaxMin.x, MaxMin.y));
        
        StartCoroutine(Move());
    }
    
    //the movement method
    IEnumerator Move()
    {
        rect.DOLocalMove(new Vector3(Random.Range(MaxMin.x, MaxMin.y), Random.Range(MaxMin.x, MaxMin.y), 0), speed).onComplete = () =>
        {
            StartCoroutine(Move());
        };
        yield return new WaitForEndOfFrame();
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(MaxMin.x, MaxMin.x, 0), 0.1f);
        Gizmos.DrawWireSphere(new Vector3(MaxMin.y, MaxMin.y, 0), 0.1f);
    }
}
