using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
	public int gold_weight = 5;
	public int gold_value = 10;   
    public float looting_time = 2;
    public float breaking_time = 2;
	public bool alarmed = false;
	public bool secured = false;
    public static Treasure instance;

    private void Start()
    {
        instance = this;
        treasure_reset();
    }

	private void set_looting_time()
    {
		looting_time = (float)gold_weight / (float)20;
    }

    public void empty_treasure()
	{
		Player.instance.add_loot_timing (looting_time);
		//gold_value = 0;
        //gold_weight = 0;
		//looting_time = 0;
	}
	
	public Loot open_treasure()
	{
		Player.instance.add_loot_timing (breaking_time);
		Loot treasure_loot = new Loot(gold_value, gold_weight);
		breaking_time = 0;
		return treasure_loot;
	}

    public void treasure_reset()
    {
        set_looting_time();
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
