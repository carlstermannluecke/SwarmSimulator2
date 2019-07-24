using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class GeometryUtility
{
    //Finds the closest point on the line (linestart, lineend) to the point (point).
    public static Vector2 ClosestPointOnLine(Vector2 point, Vector2 linestart, Vector2 lineend)
    {
        //Vector2 orthogonalVector = new Vector2(lineend.y - linestart.y, -(lineend.x - lineend.y));
        Vector2 a = lineend - linestart;
        Vector2 b = point - linestart;
        double t = Vector2.Dot(a, b) / Math.Pow(Vector2.Distance(lineend, linestart), 2);
        if (t < 0) t = 0;
        if (t > 1) t = 1;
        return linestart + (a * (float)t);
    }
    public static float LengthVec2(Vector2 vector)
    {
        return Vector2.Distance(new Vector2(0, 0), vector);
    }
    //Finds out whether the circle collides with the polygon, the closest point on the polygon, and the distance to the polygon.
    //compare https://www.spieleprogrammierer.de/wiki/2D-Kollisionserkennung#Beliebige_Polygone_im_2-dimensionalen
    //Problem: Doesn't work if the whole circle is within the polygon.
    //Solution: Triangulate, use barycentric coordinates
    public static CollisionResult PolygonCircleCollision(Vector2 position, float radius, Vector2[] cornerPoints)
    {
        float minDistanceToSide = float.MaxValue;
        Vector2 basePoint = new Vector2(0, 0);
        //float minDistanceToSide = LengthVec2(PointLineDistance(this.Position(), cornerPoints[0], cornerPoints[1]));
        for (int i = 0; i < cornerPoints.Length; i++)
        {
            Vector2 basis = ClosestPointOnLine(position, cornerPoints[i], cornerPoints[(i + 1) % cornerPoints.Length]);
            if (Vector2.Distance(position, basis) < minDistanceToSide)
            {
                minDistanceToSide = Vector2.Distance(position, basis);
                basePoint = basis;
            }
        }
        Vector2 circlePointClosestToPolygon = CircleClosestPoint(position, radius, basePoint);
        return new CollisionResult(minDistanceToSide <= radius, circlePointClosestToPolygon, basePoint, Vector2.Distance(circlePointClosestToPolygon, basePoint));
    }
    //Each column represents one triangle.
    //This should only work for convex polygons.
    public static Vector2[,] NaiveTriangulation(Vector2[] cornerPoints)
    {
        //Debug.Log(cornerPoints.Length);
        Vector2[,] output = new Vector2[cornerPoints.Length - 2, 3];
        for(int i = 2; i < cornerPoints.Length; i++)
        {
            output[i - 2, 0] = cornerPoints[0];
            output[i - 2, 1] = cornerPoints[1];
            output[i - 2, 2] = cornerPoints[i];
        }
        return output;
    }
    //Using barycentric coordinates
    //compare http://blackpawn.com/texts/pointinpoly/default.html
    public static bool PointInTriangle(Vector2 point, Vector2 a, Vector2 b, Vector2 c)
    {
        // Compute vectors 
        Vector2 v0 = c - a;
        Vector2 v1 = b - a;
        Vector2 v2 = point - a;

        // Compute dot products
        float dot00 = Vector2.Dot(v0, v0);
        float dot01 = Vector2.Dot(v0, v1);
        float dot02 = Vector2.Dot(v0, v2);
        float dot11 = Vector2.Dot(v1, v1);
        float dot12 = Vector2.Dot(v1, v2);

        // Compute barycentric coordinates
        float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
        float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
        float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

        // Check whether point is in triangle
        return (u >= 0) && (v >= 0) && (u + v < 1);
    }
    //This method fails for non-convex polygons.
    public static bool PointInPolygon(Vector2 point, Vector2[] cornerPoints)
    {
        Vector2[,] triangulation = NaiveTriangulation(cornerPoints);
        for(int i = 0; i < triangulation.GetLength(0); i++)
        {
            bool collidesWithTriangle = PointInTriangle(point, triangulation[i, 0], triangulation[i, 1], triangulation[i, 2]);
            if (collidesWithTriangle)
            {
                return true;
            }
        }
        return false;
    }
    
    public static CollisionResult CircleCollision(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB)
    {
        float distanceCenters = Vector2.Distance(centerA, centerB);
        bool collision = distanceCenters < radiusA + radiusB;
        float distance = distanceCenters - (radiusA + radiusB);
        Vector2 closestPointA = centerA + (centerB - centerA) / distanceCenters * radiusA;//not tested yet.
        Vector2 closestPointB = centerB + (centerA - centerB) / distanceCenters * radiusB;
        return new CollisionResult(collision, closestPointB, closestPointA, distance);
    }
    //Computes the closest point of a circle to a given (other) point
    public static Vector2 CircleClosestPoint(Vector2 centerCircle, float radiusCircle, Vector2 otherPoint)
    {
        bool inside = Vector2.Distance(centerCircle, otherPoint) < radiusCircle;
        if (inside)
        {
            return otherPoint;
        }
        return centerCircle + (otherPoint - centerCircle).normalized * radiusCircle;
    }
}

