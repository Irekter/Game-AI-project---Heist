using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {
	
    public static Player instance;
	private int CAPACITY = 22;
    private int current_weight;
	private List<Loot> player_loot; 
   
	void Start() 
	{
		instance = this;
		current_weight = 0;
	}

    public bool pickNewItem(Treasure t)
    {
       Loot l = new Loot(t.item_value, t.item_weight);
	   if((current_weight + l.item_weight) <= CAPACITY) 
	   { 
			current_weight += l.item_weight;
			player_loot.Add(l);
			return true;
	   } 
	   return false;
    }
	
	public bool isFull()
	{
		if(current_weight >= CAPACITY) 
		{
			return true;
		}
		return false;
	}
	
	public int percentFull()
	{
		return ((current_weight * 100) / CAPACITY);
	}
	
	public bool exchange_item(Treasure t) 
	{
		Loot l = new Loot(t.item_value, t.item_weight);
		if((current_weight + l.item_value) <= CAPACITY) 
		{ 
			current_weight += l.item_weight;
			player_loot.Add(l);
			return  true;
	    } 		
		foreach(Loot lu in player_loot) 
		{
			if(lu.item_worth < l.item_worth) 
			{
		     	if(lu.item_weight <= l.item_weight) 
				{
					current_weight = current_weight + l.item_weight - lu.item_weight;
					lu.item_weight = l.item_weight;
					lu.item_value = l.item_value;   
					lu.item_worth = l.item_worth;
					return true;
				} 
			}
		}
		return false;
	}
}

