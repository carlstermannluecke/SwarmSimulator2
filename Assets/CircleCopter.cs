using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CircleCopter : Copter
{
    public float Radius { get; private set; }
    public Color Colour { get; set; }
    public CircleCopter(float radius)
    {
        Radius = radius;
    }
    public CircleCopter(Vector2 position, float radius)
    {
        this.SetPosition(position);
        Radius = radius;
    }
    public void SetVisible(Color c)
    {
        Colour = c;
        GizmoManager.Instance.GizmoSubscribers += DrawGizmos;
        
    }

    //CollisionWithThingsDetectors:    
    public override CollisionResult PolygonCircleCollision(Vector2[] cornerPoints)
    {
        return GeometryUtility.PolygonCircleCollision(this.Position(), this.Radius, cornerPoints);
    }

    public override CollisionResult CollidesWithCopter(Copter other)
    {
        if (other.GetType().Equals(typeof(CircleCopter)))
        {
            CircleCopter otherCircleCopter = (CircleCopter)other;
            return GeometryUtility.CircleCollision(this.Position(), this.Radius, otherCircleCopter.Position(), otherCircleCopter.Radius);
        }
        //That should not happen
        throw new NotImplementedException();
    }
    //Visualisation:
    public override void DrawGizmos(GizmoManager gizmoManager)
    {
        gizmoManager.SetColor(Colour);
        Vector3 pos3d = new Vector3(Position().x, Position().y, 0.0f);
        gizmoManager.DrawSolidSphere(pos3d, Radius, "CircleCopters");
    }
    
}
