using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	
    public static Player instance;
	public int CAPACITY = 21;
	public int agent_type = 1;

	public bool training;
	public bool flee;

	public int gold_value = 10;
	int gold_weight = 10;
	int gold_weight_at_exit;

	private float time_to_exit;
	private float loot_time;
	private float break_time;

	private Vector3 exit_target;
    private NavMeshAgent agent;
	private Animator anim;
	private GameObject current_treasure;

	private Loot current_loot;
    public Loot last_loot;
    public GameObject next_treasure;
	private List<Loot> player_loot;
    public Image lootBar;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator> ();
        instance = this;
    }
		
    void Start() 
	{
        exit_target = GameObject.FindGameObjectWithTag("Exit").transform.position;
        this.player_loot = new List<Loot>();
        last_loot = new Loot();
        player_reset();
    }

    void Update()
    {
		update_time_to_exit();
		update_flee_flag();
		update_player_motion();
        LootUI();
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
		agent.transform.Rotate(new Vector3(0,0,0));
	}

	void update_time_to_exit()
	{
		float speed = agent.speed;
		if(speed > 0) 
		{
			time_to_exit = Vector3.Distance(agent.transform.position, exit_target)/speed;
			time_to_exit += (float)1.5;
		} 
		else 
		{
			time_to_exit = 0;
		}
	}

	void update_flee_flag() 
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

	void update_player_motion() 
	{
		if (break_time > 0) {
            if(break_time <= Time.deltaTime)
            {
                QLearning.instance.busy = false;
                resume_player_motion();
            }
            else
            {
                stop_player_motion();
            }
            break_time -= Time.deltaTime;
		}

		else if (loot_time > 0) 
		{
            if (loot_time <= Time.deltaTime)
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
            loot_time -= Time.deltaTime;
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
        else
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

	void destroyCoin()
	{
            GameObject coin = current_treasure.transform.GetChild (0).gameObject;
            Destroy (coin);
            QLearning.instance.busy = false;
			current_treasure = null;
            current_loot = null;
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



    // returns 1 if full
    // else 0
    public int weight_state() {
        if(is_full())
        {
            return 1;
        }
        return 0;
	}


    // returns 0 if distance to exit is greater than distance to the target
    // else return 1
    public int dist_to_trgt_state()
    {
        if (current_treasure != null)
        {
            float dist_to_trgt = Vector3.Distance(transform.position, current_treasure.transform.position);
            float dist_to_exit = Vector3.Distance(transform.position, exit_target);

            if (dist_to_trgt < dist_to_exit)
                return 0;
        }
        return 1;
    }


    // if the time to target is lesser than the timer return 0
    // else return 1
    public int time_state()
    {
        float time_to_trgt;
        float speed = agent.speed;
        if (speed > 0 && current_treasure != null)
        {
            time_to_trgt = Vector3.Distance(transform.position, current_treasure.transform.position) / speed;
            time_to_trgt += current_treasure.GetComponent<Treasure>().breaking_time;
            time_to_trgt += Vector3.Distance(exit_target, current_treasure.transform.position) / speed;
        }
        else
            return 1;

        if (Timer.instance.timelimit > time_to_trgt)
            return 0;
        
        return 1;
    }

    // if flee flag set return 1
    // else return 0
	public int flee_state() {
		if (flee) {
			return 1;
		}
		return 0;
	}


    // if the player is chasing a secured treasure then return 1
    // else return 0
    public int detection_state()
    {
        if(current_treasure != null && current_treasure.GetComponent<Treasure>().secured){
            return 1;
        }
        return 0;
    }


    // returns 1 when near the current treasure
    // else returns 0
    public int at_open_treasure_state()
    {
        if(current_treasure != null && Vector3.Distance(transform.position,current_treasure.transform.position) <= 1)
        {
            return 1;
        }
        return 0;
    }

    // returns 1 if at exit 
    // else return 0
    public int at_exit_state()
    {
        if (Vector3.Distance(transform.position, exit_target) <= 1)
        {
            return 1;
        }
        return 0;
    }

    // returns 1 when the gold remainging percentage is greater than 20%    //collect treasure
    // else return 0    // goto exit
    public int value_degradation_state()
    {
        int gold_remaining_percent = (int)(100 * (Timer.instance.timelimit / Timer.instance.resetTimer));

        if (gold_remaining_percent >= 20)
            return 1;

        return 0;
    }


    public int visited_all_state()
    {
        if (Astar.instance.trgts.Count == 0)
            return 1;
        return 0;
    }

    public int get_state()
    {
        int state = 0;
        int power = 0;

        if (weight_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (dist_to_trgt_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (time_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (flee_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (detection_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (at_open_treasure_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (at_exit_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (value_degradation_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (visited_all_state() == 1)
            state += (int)Math.Pow(2, power);

        return state;
    }

    public int get_next_state(int action)
    {
        // move = go to target
        if (action == QLearning.GO_TO_TARGET)
        {
            return go_to_target_next_state();
        }
        if(action == QLearning.GO_TO_EXIT)
        {
            return go_to_exit_next_state();
        }
        if(action == QLearning.PICK_UP_ITEM)
        {
            return pick_up_next_state();
        }
        if(action == QLearning.DROP_TREASURE)
        {
            return drop_treasure_next_state();
        }
        if(action == QLearning.SKIP_TARGET)
        {
            return skip_trgt_next_state();
        }

        return get_state();
    }

    public int go_to_target_next_state()
    {
        int state = 0;
        int power = 0;

        // weight state set to 1
        state += (int)Math.Pow(2, power);
        power++;

        // dist to target set to 0
        power++;

        // time to target set to 0
        power++;


        if (flee_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;

        if ((detection_state() == 1) || (current_treasure.GetComponent<Treasure>().secured))
            state += (int)Math.Pow(2, power);
        power++;


        // at_open_treasure set to 1
        state += (int)Math.Pow(2, power);
        power++;


        // at_exit set to 0
        power++;


        if (value_degradation_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (visited_all_state() == 1)
            state += (int)Math.Pow(2, power);

        return state;
    }

    public int go_to_exit_next_state()
    {
        int state = 0;
        int power = 0;

        if (weight_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;

        // returns 1 as the distance to exit is negligible
        state += (int)Math.Pow(2, power);
        power++;


        if (time_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;

        // flee state set to 0 as at exit
        power++;


        if (detection_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;

        // set to 0 as not at treasure
        power++;

        // at exit state always 1
        state += (int)Math.Pow(2, power);
        power++;

        // remains same
        if (value_degradation_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (visited_all_state() == 1)
            state += (int)Math.Pow(2, power);

        return state;
    }

    public int drop_treasure_next_state()
    {
        int state = 0;
        int power = 0;

        power++;
        if (dist_to_trgt_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (time_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (flee_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (detection_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (at_open_treasure_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (at_exit_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (value_degradation_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (visited_all_state() == 1)
            state += (int)Math.Pow(2, power);

        return state;
    }

    public int pick_up_next_state()
    {
        int state = 0;
        int power = 0;

     
        state += (int)Math.Pow(2, power);
        power++;
        if (dist_to_trgt_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (time_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (flee_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (detection_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (at_open_treasure_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (at_exit_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (value_degradation_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (visited_all_state() == 1)
            state += (int)Math.Pow(2, power);

        return state;
    }

    public int skip_trgt_next_state()
    {
        int state = 0;
        int power = 0;

        if (weight_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (dist_to_trgt_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (time_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (flee_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (detection_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (at_open_treasure_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (at_exit_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (value_degradation_state() == 1)
            state += (int)Math.Pow(2, power);
        power++;
        if (visited_all_state() == 1)
            state += (int)Math.Pow(2, power);

        return state;
    }


    // looting circle
    public void LootUI()
    {
        if (current_treasure != null)
        {
            float numerator = (current_treasure.GetComponent<Treasure>().breaking_time - break_time);
            lootBar.fillAmount = numerator / current_treasure.GetComponent<Treasure>().breaking_time;
        }
    }
}

