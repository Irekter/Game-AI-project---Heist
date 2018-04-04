using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
	public int gold_weight = 5;
	public int gold_value = 10;   
    public int looting_time = 2;
    public int breaking_time = 2;
    private float gold_worth = 0;
	public bool alarmed = false;
	public bool secured = false;

    private void Start()
    {
        set_gold_worth();
		set_looting_time();
    }

    private void set_gold_worth()
    {
        if (gold_weight != 0)
        {
            gold_worth = gold_value / gold_weight;
        }
        else
        {
            gold_worth = 0;
        }
    }
	
	private void set_looting_time()
    {
		looting_time = gold_weight / 10;
    }

    public void empty_treasure()
	{
		Timer.instance.timelimit -= looting_time;
		gold_value = 0;
        gold_weight = 0;
		looting_time = 0;
	}
	
	public Loot open_treasure()
	{
		Loot treasure_loot = new Loot(gold_value, gold_weight);
		Timer.instance.timelimit -= breaking_time;
		breaking_time = 0;
		return treasure_loot;
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
