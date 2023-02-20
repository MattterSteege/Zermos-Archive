using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UILineRenderer : MonoBehaviour
{
    // public List<Vector2> points;
    //
    // [SerializeField] private RectTransform rectTransform;
    // [SerializeField] private Image image;
    //
    // private void Awake()
    // {
    //     rectTransform = GetComponent<RectTransform>();
    //     image = GetComponent<Image>();
    // }
    //
    // [ContextMenu("update")]
    // public void Update()
    // {
    //     if (points.Count < 2)
    //     {
    //         return;
    //     }
    //
    //     var vertices = new List<UIVertex>();
    //     for (int i = 1; i < points.Count; i++)
    //     {
    //         Vector2 start = points[i - 1];
    //         Vector2 end = points[i];
    //         Vector2 direction = (end - start).normalized;
    //         float distance = Vector2.Distance(start, end);
    //         Vector2 perpendicular = new Vector2(-direction.y, direction.x);
    //         float thickness = rectTransform.rect.width;
    //
    //         vertices.Add(CreateVertex(start + perpendicular * thickness / 2f, image.color));
    //         vertices.Add(CreateVertex(start - perpendicular * thickness / 2f, image.color));
    //         vertices.Add(CreateVertex(end + perpendicular * thickness / 2f, image.color));
    //         vertices.Add(CreateVertex(end - perpendicular * thickness / 2f, image.color));
    //
    //         if (i < points.Count - 1)
    //         {
    //             Vector2 next = points[i + 1];
    //             Vector2 prev = points[i - 2];
    //             Vector2 startControl = start + direction * (distance / 3f);
    //             Vector2 endControl = end - direction * (distance / 3f);
    //             Vector2 startControl2 = startControl + (perpendicular + direction) * thickness / 8f;
    //             Vector2 endControl2 = endControl + (perpendicular - direction) * thickness / 8f;
    //
    //             for (int j = 0; j <= 10; j++)
    //             {
    //                 float t = (float)j / 10f;
    //                 Vector2 point = Bezier(start, startControl, startControl2, endControl2, end, t);
    //                 vertices.Add(CreateVertex(point + perpendicular * thickness / 2f, image.color));
    //                 vertices.Add(CreateVertex(point - perpendicular * thickness / 2f, image.color));
    //             }
    //         }
    //     }
    //
    //     List<UIVertex> uiVertices = new List<UIVertex>();
    //     for (int i = 0; i < vertices.Count; i++)
    //     {
    //         uiVertices[i] = vertices[i];
    //     }
    //
    //     var canvasRenderer = GetComponent<CanvasRenderer>();
    //     canvasRenderer.SetVertices(uiVertices);
    // }
    //
    // private static Vector2 Bezier(Vector2 start, Vector2 startControl, Vector2 endControl, Vector2 endControl2, Vector2 end, float t)
    // {
    //     float u = 1f - t;
    //     float tt = t * t;
    //     float uu = u * u;
    //     float uuu = uu * u;
    //     float ttt = tt * t;
    //
    //     Vector2 point = uuu * start;
    //     point += 3f * uu * t * startControl;
    //     point += 3f * u * tt * endControl;
    //     point += ttt * end;
    //
    //     return point;
    // }
    //
    // private static UIVertex CreateVertex(Vector2 position, Color color)
    // {
    //     var vertex = new UIVertex();
    //     vertex.position = position;
    //     vertex.color = color;
    //     return vertex;
    // }
}
       
