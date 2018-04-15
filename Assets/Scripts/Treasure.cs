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
	private float looting_time = 2;
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
		current_gold_value = (int)(gold_value * (Timer.instance.timelimit / Timer.instance.resetTimer));
	}

	private void set_looting_time()
    {
		looting_time = (float) gold_weight / (float) 10;
    }

    public void empty_treasure()
	{
		Player.instance.add_loot_time(looting_time);
		if (!Player.instance.is_learning()) {
			current_gold_value = 0;
			gold_value = 0;
			gold_weight = 0;
			looting_time = 0;
		}
	}
	
	public void open_treasure()
	{
		Player.instance.add_break_time(breaking_time);
		if (!Player.instance.is_learning()) {
			breaking_time = 0;
		}
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
