using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Swarming
{
    public static void LeaderMovement(TunnelArena myArena, float speed)
    {
        Copter leader = myArena.GetLeader();
        Vector2 movementVector = new Vector2(0.0f, 0.0f);
        movementVector += SwarmingFormulas.AttractToTarget(leader, myArena.GetTarget(), 0.5f);
        movementVector += AvoidWalls(leader, myArena, 1, 0.1f);
        if(movementVector.magnitude > 1.0f)
        {
            movementVector.Normalize();
        }
        leader.SetPosition(leader.Position() + (movementVector * speed));
    }
    private static Vector2 AvoidWalls(Copter copter, TunnelArena myArena)
    {
        return AvoidWalls(copter, myArena, 1, 0.5f);
    }
    private static Vector2 AvoidWalls(Copter copter, TunnelArena myArena, float repulsionMagnitude, float repulsionRadius)
    {
        Vector2 movementVector = new Vector2(0.0f, 0.0f);
        foreach (WallElement wall in myArena.AllWalls)
        {
            CollisionResult cr = copter.PolygonCircleCollision(wall.CornerPoints);
            movementVector += SwarmingFormulas.RepelWhenClose(cr.CollisionPointThisObject, cr.CollisionPointOtherObject, repulsionMagnitude, repulsionRadius);
        }
        return movementVector;
    }
    public static void FollowerMovement(TunnelArena myArena, float speed)
    {
        List<Copter> followers = myArena.GetAllCopters();//Is the leader part of this list? No!
        //bool isLeaderPartOfList = followers.Contains(myArena.GetLeader());
        //Debug.Log(isLeaderPartOfList);
        foreach(Copter follower in followers)
        {
            Vector2 movementVector = new Vector2(0.0f, 0.0f);
            movementVector += AvoidWalls(follower, myArena, 30, 0.5f);//With repulsionMagnitude = 3 and radius = 0.5f they still sometimes move through walls.
            movementVector += SwarmingFormulas.Function3(follower, myArena.GetLeader(), 0.5f, 20.0f, 0.6f);
            foreach(Copter otherCopter in followers)
            {
                if(otherCopter != follower)
                {
                    movementVector += SwarmingFormulas.Function3(follower, otherCopter, 0.3f, 40.0f, 0.6f);
                }
            }
            if (movementVector.magnitude > 1.0f)
            {
                movementVector.Normalize();
            }
            follower.SetPosition(follower.Position() + movementVector * speed);
        }

    }
}

