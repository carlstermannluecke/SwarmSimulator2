using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Copter {
    public float PosX { get; set; }
    public float PosY { get; set; }
    public Vector2 Position()
    {
        return new Vector2(PosX, PosY);
    }
    public void SetPosition(Vector2 newPosition)
    {
        PosX = newPosition.x;
        PosY = newPosition.y;
    }
    public void Translate(float x, float y)
    {
        PosX = PosX + x;
        PosY = PosY + y;
    }
    public bool CollidesWithWallElement(WallElement wall)
    {
        return CollidesWithPolygon(wall.CornerPoints);
    }
    //This assumes axis-parallel rectangles.
    public bool CollidesWithRectangle(float x, float y, float sizeX, float sizeY)
    {
        Vector2[] cornerPoints = {
            new Vector2(x, y), new Vector2(x + sizeX, y),
            new Vector2(x + sizeX, y + sizeY), new Vector2(x, y + sizeY)
        };
        return CollidesWithPolygon(cornerPoints);
    }
    public bool CollidesWithPolygon(Vector2[] cornerPoints)
    {
        Debug.Assert(cornerPoints.Length > 2);
        CollisionResult cr = PolygonCircleCollision(cornerPoints);
        //The Copter might intersect with the boundary of the polygon or be located fully within the polygon.
        return cr.IsCollided || GeometryUtility.PointInPolygon(this.Position(), cornerPoints);
    }
    public abstract CollisionResult PolygonCircleCollision(Vector2[] cornerPoints);
    public abstract CollisionResult CollidesWithCopter(Copter other);
    public abstract void DrawGizmos(GizmoManager gizmoManager);
}
