﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planner : MonoBehaviour {

    List<GameObject> selectedtarget = new List<GameObject>();
    Renderer renderer;
    public Material newmaterial;


    // Update is called once per frame
    void Update () {

        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "End" && !selectedtarget.Contains(hit.transform.gameObject))
                {
                    selectedtarget.Add(hit.transform.gameObject);
                    renderer = hit.transform.gameObject.GetComponent<Renderer>();
                    renderer.material = newmaterial;
                }
            }
        }
        
    }

}