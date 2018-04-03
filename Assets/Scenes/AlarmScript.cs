using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmScript : MonoBehaviour {


	void Start () {

     
        
    }
	
	// Update is called once per frame
	void Update () {
        //Reference for alarm
        GameObject sw = GameObject.Find("Switch");
        Button button = sw.GetComponent<Button>();


        //Getting referenceof timescript
        GameObject time = GameObject.Find("Timer");
        Timer timer = time.GetComponent<Timer>();
        //Check if switch is open 
        
        if(button.activated==false)
        {
            timer.timelimit -= 10;

        }
	}
}
