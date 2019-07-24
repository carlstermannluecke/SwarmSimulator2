using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// This arena has the size of -11.5 to 11.5 in x and -5 to 5 in y dimension.
/// It has one tunnel in the center linking the left and right part of the arena.
/// It uses CircleCopter.
/// </summary>
public class TunnelArena : Arena
{
    public List<CircleCopter> AllCopters { get; set; }
    public CircleCopter Leader { get; set; }
    public override Copter GetLeader()
    {
        return Leader;
    }
    public Vector2 Target { get; set; }
    public override Vector2 GetTarget()
    {
        return Target;
    }
    private float tunnelWidth;
    public float WidthOfTunnel
    {
        get { return tunnelWidth; }
        set { tunnelWidth = value; AllWalls.Clear(); AddWallsDefault(LengthOfTunnel, value); }
    }
    private float tunnelLength;
    public float LengthOfTunnel
    {
        get { return tunnelLength; }
        set { tunnelLength = value; AllWalls.Clear(); AddWallsDefault(value, WidthOfTunnel); }
    }
    public TunnelArena(float length, float width)
    {
        AllCopters = new List<CircleCopter>();
        tunnelLength = length;
        tunnelWidth = width;
        AddWallsDefault();
    }
    /// <summary>
    /// Places walls to a tunnel-like arena with specific dimensions.
    /// </summary>
    /// <param name="lengthOfTunnel"></param>
    /// <param name="widthOfTunnel"></param>
    public void AddWallsDefault(float lengthOfTunnel, float widthOfTunnel)
    {
        AllWalls = new List<WallElement>();
        List<Vector2> WallElementLeft = new List<Vector2>();
        WallElementLeft.Add(new Vector2(-11.5f, 5.0f));
        WallElementLeft.Add(new Vector2(-10.5f, 5.0f));
        WallElementLeft.Add(new Vector2(-10.5f, -5.0f));
        WallElementLeft.Add(new Vector2(-11.5f, -5.0f));
        AllWalls.Add(new WallElement(WallElementLeft.ToArray()));
        List<Vector2> WallElementRight = new List<Vector2>();
        WallElementRight.Add(new Vector2(10.5f, 5.0f));
        WallElementRight.Add(new Vector2(11.5f, 5.0f));
        WallElementRight.Add(new Vector2(11.5f, -5.0f));
        WallElementRight.Add(new Vector2(10.5f, -5.0f));
        AllWalls.Add(new WallElement(WallElementRight.ToArray()));
        List<Vector2> WallElementTop = new List<Vector2>();
        WallElementTop.Add(new Vector2(-10.5f, 5.0f));
        WallElementTop.Add(new Vector2(10.5f, 5.0f));
        WallElementTop.Add(new Vector2(10.5f, 4.0f));
        WallElementTop.Add(new Vector2(-10.5f, 4.0f));
        AllWalls.Add(new WallElement(WallElementTop.ToArray()));
        List<Vector2> WallElementBottom = new List<Vector2>();
        WallElementBottom.Add(new Vector2(-10.5f, -4.0f));
        WallElementBottom.Add(new Vector2(10.5f, -4.0f));
        WallElementBottom.Add(new Vector2(10.5f, -5.0f));
        WallElementBottom.Add(new Vector2(-10.5f, -5.0f));
        AllWalls.Add(new WallElement(WallElementBottom.ToArray()));
        List<Vector2> WallElementUpperTunnel = new List<Vector2>();
        WallElementUpperTunnel.Add(new Vector2(-lengthOfTunnel / 2, 5.0f));
        WallElementUpperTunnel.Add(new Vector2(lengthOfTunnel / 2, 5.0f));
        WallElementUpperTunnel.Add(new Vector2(lengthOfTunnel / 2, widthOfTunnel / 2));
        WallElementUpperTunnel.Add(new Vector2(-lengthOfTunnel / 2, widthOfTunnel / 2));
        AllWalls.Add(new WallElement(WallElementUpperTunnel.ToArray()));
        List<Vector2> WallElementLowerTunnel = new List<Vector2>();
        WallElementLowerTunnel.Add(new Vector2(-lengthOfTunnel / 2, -widthOfTunnel / 2));
        WallElementLowerTunnel.Add(new Vector2(lengthOfTunnel / 2, -widthOfTunnel / 2));
        WallElementLowerTunnel.Add(new Vector2(lengthOfTunnel / 2, -5.0f));
        WallElementLowerTunnel.Add(new Vector2(-lengthOfTunnel / 2, -5.0f));
        AllWalls.Add(new WallElement(WallElementLowerTunnel.ToArray()));
    }

    public override void AddWallsDefault()
    {
        AddWallsDefault(LengthOfTunnel, WidthOfTunnel);
    }

    public override List<Copter> GetAllCopters()
    {
        //List has to be copied, because it can't be implicitly converted from CircleCopter to Copter.
        List<Copter> output = new List<Copter>();
        foreach(CircleCopter copter in AllCopters)
        {
            output.Add(copter);
        }
        return output;
    }
    /// <summary>
    /// This method places "numberOfCopters" many copters with a radius of 0.5f.
    /// </summary>
    /// <param name="numberOfCopters"></param>
    public override void PlaceCoptersDefault(int numberOfCopters)
    {
        PlaceCoptersRadius(numberOfCopters, 0.2f);
    }
    public void PlaceCoptersRadius(int numberOfCopters, float radius)
    {
        for (int i = 0; i < numberOfCopters; i++)
        {
            bool succeeded = false;
            while (!succeeded)
            {
                //1st step: Get position proposal and make a copter:
                Vector2 proposal = PositionProposal();
                CircleCopter copter = new CircleCopter(proposal, radius);
                //2nd step: Check for collision with walls:
                succeeded = !CollisionCopterArena(copter);
                //3rd step: Check for collision with other copters:
                succeeded = succeeded && !CollisionWithOtherCopters(copter);
                if (succeeded)
                {
                    copter.SetVisible(Color.green);
                    AllCopters.Add(copter);
                }
            }
        }
    }
    /// <summary>
    /// Gives a random position that should be within the range of the left room of the arena.
    /// </summary>
    /// <returns></returns>
    private Vector2 PositionProposal()
    {
        System.Random r = new System.Random();//Should I use UnityEngine.Random? Wahrscheinlich schon.
        double x = r.NextDouble() * (10.5 - LengthOfTunnel / 2) + LengthOfTunnel / 2;
        double y = r.NextDouble() * (4 - -4) + -4;
        return new Vector2((float) x, (float) y);
    }
    public void PlaceLeader()
    {
        PlaceLeader(0.5f);
    }
    public void PlaceLeader(float radius)
    {
        bool succeeded = false;
        while (!succeeded)
        {
            //1st step: Get position proposal and make a copter:
            Vector2 proposal = PositionProposal();
            CircleCopter copter = new CircleCopter(proposal, radius);
            //2nd step: Check for collision with walls:
            succeeded = !CollisionCopterArena(copter);
            //3rd step: Check for collision with other copters:
            succeeded = succeeded && !CollisionWithOtherCopters(copter);
            if (succeeded)
            {
                Leader = copter;
                Leader.SetVisible(Color.red);
            }
        }
    }
    public bool PlaceLeader(Vector2 position, float radius)
    {
        bool succeeded = PositionAvailable(position, radius);
        if (!succeeded)
        {
            return false;
        }
        Leader = new CircleCopter(position, radius);
        Leader.SetVisible(Color.red);
        return true;
    }
    public bool PositionAvailable(Vector2 proposal, float radius)
    {
        bool succeeded = false;
        CircleCopter copter = new CircleCopter(proposal, radius);
        //2nd step: Check for collision with walls:
        succeeded = !CollisionCopterArena(copter);
        //3rd step: Check for collision with other copters:
        succeeded = succeeded && !CollisionWithOtherCopters(copter);
        return succeeded;
    }
}