using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour {
	
    public static Player instance;
	public int CAPACITY = 21;
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
	private Loot current_loot;
    public Loot last_loot;
    public int gold_weight_till_now;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator> ();
    }

	
    void Start() 
	{
		instance = this;
        exit_target = GameObject.FindGameObjectWithTag("Exit").transform.position;
        this.player_loot = new List<Loot>();
        last_loot = new Loot();
        player_reset();
    }

    private void Update()
    {
		update_time_to_exit();
		update_flee_time();
		update_player_motion();
    }

	private void update_time_to_exit()
	{
		float speed = agent.speed;
		if(speed != 0) 
		{
			time_to_exit = Vector3.Distance(agent.transform.position, exit_target)/speed;
			time_to_exit += (float)1.5;
		} 
		else 
		{
			time_to_exit = 0;
		}
	}

	private void update_flee_time() 
	{
		if (time_to_exit >= Timer.instance.timelimit) 
		{
			flee = true;
		}
        else 
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
			loot_timing = 0;
			agent.speed = 4;
			if (current_loot != null) {
				destroyCoin();
				add_loot (current_loot);
                last_loot = current_loot;
				current_loot = null;
			}

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
        gold_weight_till_now += gold_weight;
        gold_weight = 0;
        player_loot.Clear();
        QLearning.instance.busy = false;
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

            //GameObject coin = current_treasure.transform.GetChild (0).gameObject;
            //Destroy (coin);
            QLearning.instance.busy = false;
			current_treasure = null;
		}
	}

	public void add_loot(Loot treasure_loot)
	{
		gold_weight += treasure_loot.loot_weight;
		gold_value += treasure_loot.loot_value;
		player_loot.Add(treasure_loot);
	}

	public bool pick_new_loot(Loot treasure_loot)
	{
		if (is_accepting_loot(treasure_loot.loot_weight)) 
		{ 
			current_loot = treasure_loot;
			return true;
		}

        QLearning.instance.busy = false;
		return false;
	}
		
	public bool exchange_loot(Loot treasure_loot) 
	{
        // Pickup the item if we have space
		Loot it_loot = new Loot();
		bool found_exchange = false; 

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
					found_exchange = true;
					if ((it_loot.loot_value == 0) || (it_loot.CompareTo (lu) == 1))
					{
						it_loot = lu; 
					} 
                }
			}
		}
		if (found_exchange) 
		{
			current_loot = treasure_loot;
			player_loot.Remove(it_loot);
			gold_weight -= it_loot.loot_weight;
			gold_value -= it_loot.loot_value;
		}
		return found_exchange;
	}

    public void player_reset()
    {
        gold_weight = 0;
        gold_value = 0;
        player_loot.Clear();
        flee = false;
        time_to_exit = 10;
        loot_timing = 0;
        current_treasure = null;
        current_loot = null;
        gold_weight_till_now = 0;
        agent.velocity = Vector3.zero;
        agent.transform.Rotate(new Vector3(0,-190,0));
    }
}

