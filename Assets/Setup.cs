using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour {
    public TunnelArena myArena;
    // Use this for initialization
    void Start () {
        Debug.Log("Setup started");
        //GizmoManager.Instance.GizmoSubscribers += DrawGizmos;

        //Point in Triangle Test:
        Debug.Log("True expected: Is " + GeometryUtility.PointInTriangle(new Vector2(3, 3), new Vector2(1, 1), new Vector2(5, 1), new Vector2(3, 4)));
        Debug.Log("False expected: Is " + GeometryUtility.PointInTriangle(new Vector2(3, 3), new Vector2(1, 1), new Vector2(5, 1), new Vector2(3, 2)));
        //Point in Polygon Test:
        Debug.Log("True expected: Is " + GeometryUtility.PointInPolygon(new Vector2(3, 3), new Vector2[] { new Vector2(1, 1), new Vector2(5, 1), new Vector2(5, 5), new Vector2(1, 5) }));
        Debug.Log("False expected: Is " + GeometryUtility.PointInPolygon(new Vector2(2, 4), new Vector2[] { new Vector2(1, 1), new Vector2(5, 1), new Vector2(5, 5), new Vector2(3.5f, 3.5f) }));
        //TunnelArena arena = new TunnelArena();
        //arena.AddWallsDefault();
        //arena.AllWalls.ForEach(wall => wall.DrawGizmos());
        //arena.AllWalls.ForEach(wall => Debug.Log(wall.ToString()));
        //arena.PlaceCoptersDefault(5);
        //arena.GetAllCopters().ForEach(copter => copter.DrawGizmos());
        myArena = new TunnelArena(1.3f, 1.7f);//1.3, 3.7
        //bool leaderPlaced = myArena.PlaceLeader(new Vector2(1.2f, 3.3f), 0.5f);
        bool leaderPlaced = myArena.PlaceLeader(new Vector2(1.2f, 2.7f), 0.5f);
        Debug.Log(leaderPlaced);
        myArena.PlaceCoptersDefault(20);
        //myArena.PlaceLeader();
        myArena.Target = new Vector2(-8.2f, 0.8f);//-8.2, 1.8
        GizmoManager.Instance.GizmoSubscribers += DrawText;
    }
	
	// Update is called once per frame
	void Update () {
        Swarming.LeaderMovement(myArena, 0.05f);
        Swarming.FollowerMovement(myArena, 0.05f);
        
        //FollowerMovement();
        //GizmoManager gm = GizmoManager.Instance;
        //gm.SetColor(Color.red);
        //gm.DrawSolidSphere(new Vector3(3.0f, 3.0f, 0.0f), 2, "2");
        //gm.EnableDrawGroup("2", true);
        //Debug.Log("Sphere should be drawn.");
        //bool shouldDraw = true;
        //GizmoManager.Instance.EnableDrawGroup("2", shouldDraw);
        //shouldDraw = !shouldDraw;
        
    }
    private void LeaderMovement()
    {
        LeaderMovement(0.05f, 0.01f, 0.5f);
    }
    /// <summary>
    /// Uses  constant attraction to the target and repulsion II (repel when close), SI-2-12, against both other copters and walls.
    /// </summary>
    /// <param name="attraction"></param>
    /// <param name="repulsionStrength"></param>
    /// <param name="repulsionDistance"></param>
    private void LeaderMovement(float attraction, float repulsionStrength, float repulsionDistance)
    {
        //Attraction:
        Vector2 direction = ((myArena.Target) - (myArena.GetLeader().Position()));
        
        Vector2 normedDirection = direction / (Mathf.Max(Vector2.Distance(myArena.Target, myArena.GetLeader().Position()), 1.0f));
        Vector2 attractionMovement =  normedDirection * (attraction);
        //Repulsion:
        Vector2 repulsionMovement = new Vector2(0.0f, 0.0f);
        //Repulsion from other copters:
        //foreach (CircleCopter copter in myArena.GetAllCopters())
        //{
        //    CollisionResult cr = myArena.GetLeader().CollidesWithCopter(copter);
        //    Vector2 repulsionDirection = cr.CollisionPointThisObject - cr.CollisionPointOtherObject;
        //    Vector2 repulsionVector = repulsionStrength * Mathf.Exp(-0.5f * cr.Distance * cr.Distance / (repulsionDistance * repulsionDistance)) * repulsionDirection;
        //    repulsionMovement += repulsionVector;
        //}
        //Repulsion from walls (doesn't work yet):
        //foreach (WallElement wall in myArena.AllWalls)
        //{
        //    CollisionResult cr = myArena.GetLeader().PolygonCircleCollision(wall.CornerPoints);
        //    Vector2 repulsionDirection = cr.CollisionPointThisObject - cr.CollisionPointOtherObject;
        //    Vector2 repulsionVector = repulsionStrength * Mathf.Exp(-0.5f * cr.Distance * cr.Distance / (repulsionDistance * repulsionDistance)) * repulsionDirection;
        //    repulsionMovement += repulsionVector;
        //}
        //Position Update:
        myArena.GetLeader().SetPosition(myArena.GetLeader().Position() + attractionMovement + repulsionMovement);
    }
    private void FollowerMovement()
    {
        FollowerMovement(0.05f, 0.01f, 0.5f);
    }
    private void FollowerMovement(float attraction, float repulsionStrength, float repulsionDistance)
    {
        Copter leader = myArena.GetLeader();
        
        foreach (CircleCopter thisCopter in myArena.GetAllCopters())
        {
            //Attraction:
            Vector2 toLeader = leader.Position() - thisCopter.Position();
            Vector2 toLeaderNormed = toLeader / (Vector2.Distance(leader.Position(), thisCopter.Position()));
            Vector2 attractionMovement = toLeaderNormed * attraction;
            //Repulsion from leader:
            Vector2 repulsionFromLeader = repulsionStrength * Mathf.Exp(-0.5f * (toLeader.magnitude * toLeader.magnitude) / (repulsionDistance * repulsionDistance)) * toLeader;

            //Attraction to and repulsion from other copters:
            Vector2 repulsionMovement = new Vector2(0.0f, 0.0f);
            foreach (CircleCopter otherCopter in myArena.GetAllCopters())
            {
                if(thisCopter != otherCopter)
                {
                    CollisionResult cr = thisCopter.CollidesWithCopter(otherCopter);
                    Vector2 repulsionDirection = cr.CollisionPointThisObject - cr.CollisionPointOtherObject;
                    Vector2 repulsionVector = repulsionStrength * Mathf.Exp(-0.5f * cr.Distance * cr.Distance / (repulsionDistance * repulsionDistance)) * repulsionDirection;
                    repulsionMovement += repulsionVector - attraction * repulsionDirection;
                }
            }
            //Position update:
            Vector2 update = (attractionMovement + repulsionFromLeader + repulsionMovement).normalized * 0.05f;
            thisCopter.SetPosition(thisCopter.Position() + update);
        }
    }
    public void DrawText(GizmoManager gizmoManager)
    {
        gizmoManager.SetColor(Color.red);
        gizmoManager.DrawText("Text", new Vector3(3.0f, 3.0f, 0.0f), new Vector3(1, 1, 1), "InfoText");
    }
    public void DrawGizmos(GizmoManager gizmoManager)
    {
        gizmoManager.SetColor(Color.black);
        gizmoManager.DrawSolidSphere(new Vector3(3.0f, 3.0f, 0.0f), 2, "2");
    }
}
