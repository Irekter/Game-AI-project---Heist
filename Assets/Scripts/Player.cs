using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour {
	
    public static Player instance;
	public int CAPACITY = 21;
	public int agent_type = 1;

	public bool training = true;
	private bool flee;

	private int gold_value;
	private int gold_weight;
	private int gold_weight_at_exit;

	private float time_to_exit;
	private float loot_time;
	private float break_time;

	private Vector3 exit_target;
    private NavMeshAgent agent;
	private Animator anim;
	private GameObject current_treasure;

	private Loot current_loot;
    public Loot last_loot;
	public Treasure next_treasure;
	private List<Loot> player_loot;

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
		update_flee_flag();
		update_player_motion();
    }

	public void player_reset()
	{
		flee = false;
		gold_weight = 0;
		gold_value = 0;
		gold_weight_at_exit = 0;

		player_loot.Clear();

		time_to_exit = 0;
		break_time = 0;
		loot_time = 0;

		current_treasure = null;
		current_loot = null;
		last_loot = null;

		agent.velocity = Vector3.zero;
		agent.transform.Rotate(new Vector3(0,-190,0));
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

	private void update_flee_flag() 
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
		if (break_time > 0) {
            if(break_time <= Time.fixedDeltaTime)
            {
                QLearning.instance.busy = false;
                resume_player_motion();
            }
            else
            {
                stop_player_motion();
            }
            break_time -= Time.fixedDeltaTime;
		}

		else if (loot_time > 0) 
		{
            if (loot_time <= Time.fixedDeltaTime)
            {
                resume_player_motion();
                if (current_loot != null)
                {
                    Debug.Log("Adding Loot");
                    add_loot(current_loot);
                    last_loot = current_loot;
                    destroyCoin();
                }
            }
            else
            {
                stop_player_motion();
            }
            loot_time -= Time.fixedDeltaTime;
        }

	}

	public void stop_player_motion()
	{
		anim.SetInteger("moving", 0);	
		agent.velocity = Vector3.zero;
		agent.speed = 0;
	}

	public void resume_player_motion()
	{
		anim.SetInteger("moving", 1);
		loot_time = 0;
		agent.speed = 4;
	}

	public void flee_player() {
		resume_player_motion ();
	}

	public void add_loot_time(float timelimit) 
	{
		loot_time += timelimit;
	}

	public void add_break_time(float timelimit) 
	{
		break_time += timelimit;
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
		return loot_time;
	}

	public int get_gold_weight_at_exit()
	{
		return gold_weight_at_exit;
	}

    public void drop_treasure_at_exit()
    {
        if (Vector3.Distance(transform.position, exit_target) <= 1)
        {
            Debug.Log("i am here");
            gold_weight_at_exit += gold_weight;
            gold_weight = 0;
            player_loot.Clear();
            QLearning.instance.busy = false;
        }
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

	public bool is_learning()
	{
		return training;
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
            QLearning.instance.busy = false;
			current_treasure = null;
            current_loot = null;
        }
	}

	public void add_loot(Loot treasure_loot)
	{
        Debug.Log("Loot: " + treasure_loot.loot_weight);

        gold_weight += treasure_loot.loot_weight;
		gold_value += treasure_loot.loot_value;
		player_loot.Add(treasure_loot);
	}

	public bool pick_new_loot()
	{
		if (current_treasure != null) {
			Loot treasure_loot = current_treasure.GetComponent<Treasure>().get_treasure_loot(); 
			if (is_accepting_loot (treasure_loot.loot_weight)) { 
				current_loot = treasure_loot;
                Debug.Log("Picked: " + current_loot.loot_value);
                return true;
			}

			QLearning.instance.busy = false;
		}
		return false;
	}
		
	public bool exchange_loot() 
	{
        // Pickup the item if we have space
		Loot it_loot = new Loot();
		bool found_exchange = false; 

		if(pick_new_loot()) 
		{
			return true;
		}

        if (current_treasure != null)
        {
            Loot treasure_loot = current_treasure.GetComponent<Treasure>().get_treasure_loot();
            foreach (Loot lu in player_loot)
            {
                if (lu.loot_worth < treasure_loot.loot_worth)
                {
                    if ((gold_weight + treasure_loot.loot_weight - lu.loot_weight) <= CAPACITY)
                    {
                        found_exchange = true;
                        if ((it_loot.loot_value == 0) || (it_loot.CompareTo(lu) == 1))
                        {
                            it_loot = lu;
                        }
                    }
                }
            }
            if (found_exchange)
            {
                current_loot = treasure_loot;
                Debug.Log("Exchanged: " + current_loot.loot_value);
                player_loot.Remove(it_loot);
                gold_weight -= it_loot.loot_weight;
                gold_value -= it_loot.loot_value;
            }
        }
		return found_exchange;
	}


    public GameObject get_current_treasure()
    {
        return current_treasure;
    }

    public void remove_current_treasure()
    {
        current_treasure = null;
    }

    // State 


    public int weight_state() {
		float state = (float)(gold_weight * 100) / (float)CAPACITY;
		state = (int)(state / 20);
        if(state > 5)
        {
            state = 5;
        }
        return (int)state;
	}

	public int flee_state() {
		if (flee) {
			return 1;
		}
		return 0;
	}

}

