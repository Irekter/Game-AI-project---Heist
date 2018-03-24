using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    public Rigidbody rbody;
    public float force;

	// Update is called once per frame
	void Update () {

        if (Input.GetKey("right"))
        {
            rbody.AddForce(force * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey("left"))
        {
            rbody.AddForce(-force * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey("up"))
        {
            rbody.AddForce(0, 0, force * Time.deltaTime);
        }
        if (Input.GetKey("down"))
        {
            rbody.AddForce(0, 0, -force * Time.deltaTime);
        }
    }

}
