using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Treasure : MonoBehaviour
{
	public int gold_weight = 5;
	public int gold_value = 10;   

    public float looting_time = 2;
    public float breaking_time = 2;
    public float temp_looting_time = 0;

	public bool alarmed = false;
	public bool secured = false;

    //public GameObject sw = GameObject.FindGameObjectWithTag ("Button");
    

    private void Start()
    {
		set_looting_time();
    }

    private void Update()
    {
      
    }
    private void set_looting_time()
    {
		looting_time = (float)gold_weight / (float)10;
    }

    public void empty_treasure()
	{
		Player.instance.add_loot_timing (looting_time);
		gold_value = 0;
        gold_weight = 0;
		looting_time = 0;
	}
	
	public Loot open_treasure()
	{
		Player.instance.add_loot_timing (breaking_time);
		Loot treasure_loot = new Loot(gold_value, gold_weight);
		breaking_time = 0;
		return treasure_loot;
	}
		
	// Varun's Alarmed Treasure Code
	
	public void alarmedChest()
    {

    }

   
}
