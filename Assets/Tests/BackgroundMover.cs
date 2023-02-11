using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BackgroundMover : MonoBehaviour
{
    [SerializeField] RectTransform rect1;
    [SerializeField] RectTransform rect2;
    [SerializeField] float speed = 1f;
    [SerializeField] float diffrence = 1f;

    private Vector2 direction1;
    private Vector2 direction2;

    private void Start()
    {
        direction1 = RandomDirection();
        direction2 = direction1 * (1 + diffrence / 100);
    }

    private void Update()
    {
        rect1.anchoredPosition += direction1 * (speed * Time.deltaTime);
        rect2.anchoredPosition += direction2 * (speed * Time.deltaTime);

        if (Vector2.Distance(rect1.anchoredPosition, Vector2.zero) >= 15 ||
            Vector2.Distance(rect2.anchoredPosition, Vector2.zero) >= 15)
        {
            direction1 = RandomDirection();
            direction2 = direction1 * (1 + diffrence / 100);
        }
    }

    private Vector2 RandomDirection()
    {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);

        return new Vector2(x, y).normalized * 10f;
    }
}