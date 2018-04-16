using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
	public int gold_weight = 5;
	public int gold_value = 10; 
	public int current_gold_value = 0;
    public float breaking_time = 2;
	public float looting_time = 2;
	public bool alarmed = false;
	public bool secured = false;
	private bool decaying;

    private void Start()
    {
		set_looting_time();
		current_gold_value = gold_value;
		decaying = true;
    }
	
	private void Update() {
		if (decaying) {
			set_current_gold_value ();
		}
	}

	private void set_current_gold_value() {
		//float decrease = (Timer.instance.resetTimer - Timer.instance.timelimit) / Timer.instance.resetTimer;
		//decrease = decrease / 200;
		//current_gold_value = (int)((float)gold_value * (float)(1 - decrease));
	}

	private void set_looting_time()
    {
		looting_time = (float) gold_weight / (float) 10;
    }

    public void empty_treasure()
	{
		if (looting_time > 0) {
			Player.instance.set_loot_time (looting_time);
		}
		if (!Player.instance.is_learning()) {
			gold_value = 0;
			gold_weight = 0;
		}
		current_gold_value = 0;
		looting_time = 0;
	}
	
	public void open_treasure()
	{
		if (breaking_time > 0) {
			Player.instance.set_break_time (breaking_time);
		}
		breaking_time = 0;
	}

	public Loot get_treasure_loot()
	{
		return new Loot(current_gold_value, gold_weight);
	}

	public Vector3 get_treasure_position()
	{
		return transform.position;
	}

	public void treasure_reset() {
		current_gold_value = gold_value;
		breaking_time = 2;
		decaying = true;
		set_looting_time ();
	}
}
