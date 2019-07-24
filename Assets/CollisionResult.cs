using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//Contains information on whether, where and by how far things collide.
public class CollisionResult
{
    public bool IsCollided { get; set; }
    public Vector2 CollisionPointThisObject { get; set; }
    public Vector2 CollisionPointOtherObject { get; set; }
    public float Distance { get; set; }
    public CollisionResult(bool isCollided, Vector2 collisionPoint, float distance)
    {
        this.IsCollided = isCollided;
        this.CollisionPointOtherObject = collisionPoint;
        this.Distance = distance;
    }
    public CollisionResult(bool isCollided, Vector2 collisionPointThisObject, Vector2 collisionPointOtherObject, float distance)
    {
        this.IsCollided = isCollided;
        this.CollisionPointThisObject = collisionPointThisObject;
        this.CollisionPointOtherObject = collisionPointOtherObject;
        this.Distance = distance;
    }
}
