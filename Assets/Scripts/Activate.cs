using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activate : MonoBehaviour {

    public bool ON = false;
    private void OnCollisionEnter(Collision collisionInfo)
    {
        if(collisionInfo.collider.tag=="Button")
        {
            Debug.Log("Collided with box");
            ON = true;
        }
    }
}
