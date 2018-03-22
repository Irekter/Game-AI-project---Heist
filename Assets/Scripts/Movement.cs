using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    public Rigidbody rigidbody;
    public float force;
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey("right"))
        {
            rigidbody.AddForce(force * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey("left"))
        {
            rigidbody.AddForce(- force * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey("up"))
        {
            rigidbody.AddForce(0, 0, force * Time.deltaTime);
        }
        if (Input.GetKey("down"))
        {
            rigidbody.AddForce(0, 0, -force * Time.deltaTime);
        }
    }
}
