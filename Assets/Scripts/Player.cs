using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour {
	
    public static Player instance;
	public int CAPACITY = 20;
    public int current_weight;
	private List<Loot> player_loot;
    Treasure current_target;

    //private float time_to_exit
    //{
    //    get {
    //        return Vector3.Distance() / agent.velocity 
    //    }
    //}
    NavMeshAgent agent;

	void Start() 
	{
		instance = this;
		current_weight = 0;
	}
	
	
	public Player(int cap){
		this.CAPACITY = cap;
		this.current_weight = 0;
		this.player_loot = new List<Loot>();
	}

    public bool pickNewItem(Treasure t)
    {
       Loot loot = new Loot(t.item_value, t.item_weight);
	   if((current_weight + loot.item_weight) <= CAPACITY) 
	   { 
			current_weight += loot.item_weight;
			player_loot.Add(loot);
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
		Loot loot = new Loot(t.item_value, t.item_weight);
		if((current_weight + loot.item_value) <= CAPACITY) 
		{ 
			current_weight += loot.item_weight;
			player_loot.Add(loot);
			return  true;
	    } 		
		foreach(Loot lu in player_loot) 
		{
			if(lu.item_worth < loot.item_worth) 
			{
		     	if(lu.item_weight <= loot.item_weight) 
				{
					current_weight = current_weight + loot.item_weight - lu.item_weight;
					lu.item_weight = loot.item_weight;
					lu.item_value = loot.item_value;   
					lu.item_worth = loot.item_worth;
					return true;
				} 
			}
		}
		return false;
	}
}

