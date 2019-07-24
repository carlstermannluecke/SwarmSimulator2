using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WallElement
{

    public WallElement(Vector2[] vector2)
    {
        CornerPoints = vector2;
        GizmoManager.Instance.GizmoSubscribers += DrawGizmos;
    }

    public Vector2[] CornerPoints { get; set; }
    public void AddCornerPoint(Vector2 point)
    {
        List<Vector2> points = CornerPoints.ToList();
        points.Add(point);
        CornerPoints = points.ToArray();
    }
    public void DrawGizmos(GizmoManager gizmoManager)
    {
        gizmoManager.SetColor(Color.black);
        for(int i = 0; i < CornerPoints.Length; i++)
        {
            Vector2 to = CornerPoints[(i + 1) % CornerPoints.Length];
            Vector2 from = CornerPoints[(i) % CornerPoints.Length];
            Vector3 to3 = new Vector3(to.x, to.y, 0.0f);
            Vector3 from3 = new Vector3(from.x, from.y, 0.0f);
            gizmoManager.DrawLine(from3, to3, 0.1f, "Walls");
        }
    }
    public override string ToString()
    {
        return CornerPoints.ToString();
        //return base.ToString();
    }
}

