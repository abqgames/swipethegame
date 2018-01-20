using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineStick : MonoBehaviour {
    private Rigidbody2D rb;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if ( coll.gameObject.layer == LayerMask.NameToLayer("Player") && coll.GetComponent<LaunchPlayer>().stuckObject == null )
        {
            HingeJoint2D joint = coll.GetComponent<HingeJoint2D>();
            joint.enabled = true;
            joint.connectedBody = rb;
            coll.GetComponent<LaunchPlayer>().stuckObject = gameObject;
            
        }
    }
    void OnTriggerExit2D(Collider2D coll)
    {
        LaunchPlayer l = coll.GetComponent<LaunchPlayer>();
        if (l != null && l.stuckObject == gameObject)
        {
            l.stuckObject = null;
            Debug.Log("aaaaaaaa");
        }
    }
}
