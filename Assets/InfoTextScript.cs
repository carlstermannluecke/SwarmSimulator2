using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoTextScript : MonoBehaviour {
    public GameObject setup;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Setup s = setup.GetComponent<Setup>();
        TunnelArena ta = s.myArena;
        
	}
}
