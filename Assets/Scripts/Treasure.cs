using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
	public int item_weight = 0;
	public int item_value = 0;   
    public int looting_time = 0;

    private float item_worth = 0;

	public bool visited = false;
	public bool alarmed = false;
	public bool secured = false;
	public bool opened = false;

    private void Start()
    {
        setItemWorth();
    }


    private void Update()
    {
        setItemWorth();
    }

    void setItemWorth()
    {
        if (item_weight != 0)
        {
            item_worth = item_value / item_weight;
        }
        else
        {
            item_worth = 0;
        }
    }


    public void empty_treasure()
	{
		item_value = 0;
        item_weight = 0;
		opened = true;
		looting_time = 0;		
	}
	
	
	// Varun's Alarmed Treasure Code
	/*
	
	public GameObject sw;
    public GameObject time;
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        //Reference for alarm
        sw = GameObject.Find("Switch");
        Button button = sw.GetComponent<Button>();


        //Getting referenceof timescript
        time = GameObject.Find("Timer");
        Timer timer = time.GetComponent<Timer>();
        //Check if switch is open 
        
        if(button.activated==false)
        {
            timer.timelimit -= 10;

        }
	}
	
	*/
}
