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
    private float time_to_exit;
    public int agent_type = 1;
    NavMeshAgent agent;
    private float delay_time = 0;
    private bool looping = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }


    void Start() 
	{
		instance = this;
		current_weight = 0;
        this.player_loot = new List<Loot>();
    }

    private void Update()
    {
        float velocity = agent.velocity.magnitude;
        //time_to_exit = Vector3.Distance(current_target.position, gameObject.transform.position) / velocity;
        //Debug.Log(time_to_exit);
    }

    public void dropTreasureAtExit()
    {
        current_weight = 0;
        player_loot.Clear();
    }

    public bool pickNewItem(Treasure t)
    {
       Loot loot = new Loot(t.item_value, t.item_weight);
       t.visited = true;

       if ((current_weight + loot.item_weight) <= CAPACITY) 
	   { 
			current_weight += loot.item_weight;
			player_loot.Add(loot);
            t.empty_treasure();
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
        t.visited = true;

        // normal pickup
        if ((current_weight + loot.item_weight) <= CAPACITY) 
		{
            delay_time = 5;
            while (looping) { };
            current_weight += loot.item_weight;
			player_loot.Add(loot);
            t.empty_treasure();
            return true;
	    }
        
        foreach (Loot lu in player_loot) 
		{
            
            if (lu.item_worth < loot.item_worth) 
			{
		     	if((current_weight + loot.item_weight - lu.item_weight) <= CAPACITY) 
				{   
                    current_weight = current_weight + loot.item_weight - lu.item_weight;
					lu.item_weight = loot.item_weight;
					lu.item_value = loot.item_value;   
					lu.item_worth = loot.item_worth;
                    t.empty_treasure();
                    return true;
                } 
			}
		}
		return false;
	}
}

