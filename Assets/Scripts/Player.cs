using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour {
	
    public static Player instance;
	public int CAPACITY = 20;
    private int gold_weight;
	private int gold_value;
	private List<Loot> player_loot;
    private Vector3 exit_target;
    private float time_to_exit;
    public bool flee;
	public int agent_type = 1;
    NavMeshAgent agent;
	private float loot_timing;
	private Animator anim;
	private GameObject current_treasure;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator> ();
    }

	
    void Start() 
	{
		instance = this;
		gold_weight = 0;
		gold_value = 0;
        this.player_loot = new List<Loot>();
		exit_target = GameObject.FindGameObjectWithTag ("Exit").transform.position;
		flee = false;
		time_to_exit = 10;
		loot_timing = 0;
		current_treasure = null;
    }

    private void Update()
    {
		update_time_to_exit();
		update_flee_time();
		update_player_motion();
    }

	private void update_time_to_exit()
	{
		float velocity = agent.velocity.magnitude;
		if(velocity != 0) 
		{
			time_to_exit = ((agent.transform.position - exit_target).magnitude)/velocity;
			time_to_exit += (float)1.5;
		} 
		else 
		{
			time_to_exit = 10;
		}
	}

	private void update_flee_time() 
	{
		if (time_to_exit >= Timer.instance.timelimit) 
		{
			flee = true;
		} else 
		{
			flee = false;
		}
	}

	private void update_player_motion() 
	{
		if (loot_timing > 0) 
		{
			loot_timing -= Time.fixedDeltaTime;
			if (flee) 
			{
				anim.SetInteger("moving", 1);
				agent.speed = 4;
			} 
			else 
			{
				anim.SetInteger("moving", 0);	
				agent.velocity = Vector3.zero;
				agent.speed = 0;
			}
		}
		else
		{
			anim.SetInteger("moving", 1);
			Debug.Log ("Moving");
			loot_timing = 0;
			agent.speed = 4;
			destroyCoin();
		}   
	}

	public void add_loot_timing(float timelimit) {
		loot_timing += timelimit;
	}
		
	public int get_player_gold_weight()
	{
		return gold_weight; 
	}

	public int get_player_gold_value()
	{
		return gold_value; 
	}

	public float get_time_to_exit()
	{
		return time_to_exit; 
	}

	public float get_looting_time()
	{
		return loot_timing;
	}

    public void drop_treasure_at_exit()
    {
        gold_weight = 0;
        player_loot.Clear();
    }
		
	public bool is_accepting_loot(int loot_weight)
	{
		if((gold_weight + loot_weight) <= CAPACITY) 
		{
			return true;
		}
		return false;
	}
	
	
	public bool is_full()
	{
		if(gold_weight >= CAPACITY) 
		{
			return true;
		}
		return false;
	}
	
	public int percent_full()
	{
		return ((gold_weight * 100) / CAPACITY);
	}

	public void set_current_treasure(GameObject treasure) 
	{
		current_treasure = treasure;
	}

	private void destroyCoin()
	{
		if (current_treasure != null) {
			GameObject coin = current_treasure.transform.GetChild (0).gameObject;
			Destroy (coin);
			current_treasure = null;
		}
	}

	public bool pick_new_loot(Loot treasure_loot)
	{
		if (is_accepting_loot(treasure_loot.loot_weight)) 
		{ 
			gold_weight += treasure_loot.loot_weight;
			gold_value += treasure_loot.loot_value;
			player_loot.Add(treasure_loot);
			return true;
		} 
		return false;
	}
		
	public bool exchange_loot(Loot treasure_loot) 
	{
        // Pickup the item if we have space
		if(pick_new_loot(treasure_loot)) 
		{
			return true;
		}
        foreach (Loot lu in player_loot) 
		{
			if (lu.loot_worth < treasure_loot.loot_worth) 
			{
				if((gold_weight + treasure_loot.loot_weight - lu.loot_weight) <= CAPACITY) 
				{   
					gold_weight = gold_weight + treasure_loot.loot_weight - lu.loot_weight;
					gold_value = gold_value + treasure_loot.loot_value - lu.loot_value;
					lu.loot_weight = treasure_loot.loot_weight;
					lu.loot_value = treasure_loot.loot_value;   
					lu.loot_worth = treasure_loot.loot_worth;
                    return true;
                } 
			}
		}
		return false;
	}
}

