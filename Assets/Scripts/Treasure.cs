using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure
{
	public Vector3 position;
	public int item_weight;
	public int item_value;   
    public int looting_time;
	public float item_worth;
	public bool visted;
	public bool alarmed;
	public bool secured;
	public bool opened;
	
	
    public Treasure(Vector3 _pos, int _value_item, int _weight, int _looting_time, bool _alarmed, bool _secured)
    {
 		this.position = _pos;
		this.item_weight = _weight;
	    this.item_value = _value_item;   
		if(_value_item != 0) 
		{
			this.item_worth = _weight / _value_item;
		} 
		else 
		{
			this.item_worth = 0;
		}
		this.visted = false;
		this.alarmed = _alarmed;
		this.secured = _secured;
		this.opened = false;
		this.looting_time = _looting_time;
    }
	
	void empty_treasure()
	{
		this.item_value = 0;
		this.item_weight = 0;
		this.item_worth = 0;
		this.visted = true;
		this.opened = true;
		this.looting_time = 0;		
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
