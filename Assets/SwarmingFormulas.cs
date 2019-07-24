using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SwarmingFormulas
{
    public static Vector2 Function3(Copter thisCopter, Copter otherCopter)
    {
        return Function3(thisCopter, otherCopter, 1, 20, 0.2f);
    }
    public static Vector2 Function3(Copter thisCopter, Copter otherCopter, float a, float b, float c)
    {
        return Function3(thisCopter, otherCopter.Position(), a, b, c);
    }
    public static Vector2 Function3(Copter thisCopter, Vector2 otherCopter, float a, float b, float c)
    {
        Vector2 direction = thisCopter.Position() - otherCopter;
        float attraction = a;
        float repulsion = b * ((float)Math.Exp(-(direction.magnitude * direction.magnitude) / c));
        return -direction * (attraction - repulsion);
    }
    public static Vector2 RepelWhenClose(Copter thisCopter, WallElement wall, float repulsionMagnitude, float repulsionRadius)
    {
        CollisionResult cr = thisCopter.PolygonCircleCollision(wall.CornerPoints);
        return RepelWhenClose(thisCopter, cr.CollisionPointOtherObject, repulsionMagnitude, repulsionRadius);
    }
    public static Vector2 RepelWhenClose(Copter thisCopter, Vector2 otherPoint, float repulsionMagnitude, float repulsionRadius)
    {
        return RepelWhenClose(thisCopter.Position(), otherPoint, repulsionMagnitude, repulsionRadius);
    }
    public static Vector2 RepelWhenClose(Vector2 thisCopter, Vector2 otherPoint, float repulsionMagnitude, float repulsionRadius)
    {
        Vector2 direction = thisCopter - otherPoint;
        float repulsion = repulsionMagnitude * ((float)Math.Exp(-0.5f * direction.magnitude * direction.magnitude / (repulsionRadius * repulsionRadius)));
        return repulsion * direction.normalized;
    }
    public static Vector2 AttractToTarget(Copter thisCopter, Vector2 target, float attractionMagnitude)
    {
        Vector2 direction = target- thisCopter.Position();
        Vector2 normedDirection = direction / (Mathf.Max(direction.magnitude, 1.0f));
        return normedDirection * attractionMagnitude;
    }
}

