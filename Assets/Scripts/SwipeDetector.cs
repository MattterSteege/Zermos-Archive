using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwipeDetector : MonoBehaviour
{
	private Vector2 fingerDownPos;
	private Vector2 fingerUpPos;

	public bool detectSwipeAfterRelease = false;

	public float SWIPE_THRESHOLD = 75f;

	// Update is called once per frame
	void Update ()
	{

		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Began) {
				fingerUpPos = touch.position;
				fingerDownPos = touch.position;
			}

			//Detects Swipe while finger is still moving on screen
			if (touch.phase == TouchPhase.Moved) {
				if (!detectSwipeAfterRelease) {
					fingerDownPos = touch.position;
					DetectSwipe ();
				}
			}

			//Detects swipe after finger is released from screen
			if (touch.phase == TouchPhase.Ended) {
				fingerDownPos = touch.position;
				DetectSwipe ();
			}
		}
	}

	void DetectSwipe ()
	{
		
		if (VerticalMoveValue () > SWIPE_THRESHOLD && VerticalMoveValue () > HorizontalMoveValue ()) {
			if (fingerDownPos.y - fingerUpPos.y > 0) {
				SwipeUp ();
			} else if (fingerDownPos.y - fingerUpPos.y < 0) {
				SwipeDown ();
			}
			fingerUpPos = fingerDownPos;

		} else if (HorizontalMoveValue () > SWIPE_THRESHOLD && HorizontalMoveValue () > VerticalMoveValue ()) {
			if (fingerDownPos.x - fingerUpPos.x > 0) {
				SwipeRight ();
			} else if (fingerDownPos.x - fingerUpPos.x < 0) {
				SwipeLeft ();
			}
			fingerUpPos = fingerDownPos;

		}
	}

	float VerticalMoveValue ()
	{
		return Mathf.Abs (fingerDownPos.y - fingerUpPos.y);
	}

	float HorizontalMoveValue ()
	{
		return Mathf.Abs (fingerDownPos.x - fingerUpPos.x);
	}

	//make 4 events for left,right,up and down
	public delegate void OnSwipeLeft();
	public static event OnSwipeLeft onSwipeLeft;
	
	public delegate void OnSwipeRight();
	public static event OnSwipeRight onSwipeRight;
	
	public delegate void OnSwipeUp();
	public static event OnSwipeUp onSwipeUp;
	
	public delegate void OnSwipeDown();
	public static event OnSwipeDown onSwipeDown;

	void SwipeLeft() => onSwipeLeft?.Invoke();
	void SwipeRight() => onSwipeRight?.Invoke();
	void SwipeUp() => onSwipeUp?.Invoke();
	void SwipeDown() => onSwipeDown?.Invoke();
}