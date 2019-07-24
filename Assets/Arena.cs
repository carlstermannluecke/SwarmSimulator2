using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Arena {
    public List<WallElement> AllWalls { get; set; }
    
    public abstract List<Copter> GetAllCopters();
    public abstract Copter GetLeader();
    public abstract Vector2 GetTarget();
    public abstract void AddWallsDefault();
    public abstract void PlaceCoptersDefault(int numberOfCopters);

    public bool CollisionCopterArena(Copter copter)
    {
        return AllWalls.Any(wall => copter.CollidesWithWallElement(wall));
    }
    //This is a naive O(n^2) implementation. It can be accelerated using divide and conquer.
    public bool CollisionWithOtherCopters(Copter copter)
    {
        List<Copter> copters = GetAllCopters();
        if(GetLeader() != null)
        {
            copters.Add(GetLeader());
        }
        return copters.Any(c => copter.CollidesWithCopter(c).IsCollided);
    }
}
