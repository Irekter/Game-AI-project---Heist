﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;

public class QLearning : MonoBehaviour {

    Transform target;
    NavMeshAgent agent;
    int action;
    double[,] Q = new double[STATES,POSSIBLE_MOVES];
    double[,] R = { {2,-1, 1}
                   ,{2, -1, 1}
                   ,{2,-1, 1}
                   ,{2,-1, 1}
                   ,{2,0, 1}
                   ,{-1,2, 1}};


    public static QLearning instance;
    public bool Done;

    // ACTIONS:
    public const int GO_TO_TARGET = 0;
    public const int GO_TO_EXIT = 1;
    public const int PICK_UP_ITEM = 2;
    public const int DROP_TREASURE = 3;
    public const int SKIP_TARGET = 4;
    public const int FLEE = 5;

    const int POSSIBLE_MOVES = 6;
    const int STATES = 512;
    const string path = "./model.txt";
    Vector3 start_pos;

    void Awake()
    {
        instance = this;
        agent = GetComponent<NavMeshAgent>();
    }


    // Use this for initialization
    void Start() {
        action = 0;
        start_pos = gameObject.transform.position;
    }

    Random rd = new Random();
    public bool busy;

    // Update is called once per frame
    void Update() {
        if (Done)
        {
            EnvReset();
        }

        if (Player.instance.visited_all_state() == 1)
            Player.instance.flee = true;

        // selects action using q-learning
        if (!busy)
        {
            target = Astar.instance.targetSelector();
            action = move_decider();
            busy = true;
        }

        // executes the actions
        Tasks(action);
    }


    int move_decider()
    {
        action = QLearner();
        return action;
    }


    // HYPERPARAMETERS:
    double gamma = 0.3;
    double epsilon = 0.2;
    double alpha = 0.2;

    int QLearner()
    {
        
        int rnd = Random.Range(0,2);
     
        int currentstate = Player.instance.get_state();

        // random action according to epsilon-greedy 
        if (rnd < epsilon)
            action = Random.Range(0, POSSIBLE_MOVES);
        else
            action = getAction(currentstate);
        Debug.Log("action: " + action);

        int nextstate = Player.instance.get_next_state(action);

        double reward = rewardFunction(action);

        double currentQ = Q[currentstate, action];
        double maxQ = getMaxQ(nextstate);

        double q = (1 - alpha) * currentQ + alpha * (reward + gamma * maxQ);

        Q[currentstate, action] = q;

        // epsilon decaying
        epsilon = 0.9 * epsilon;
        return action;
    }


    double rewardFunction(int selected_action)
    {
        double reward = 0; 

        if(Player.instance.weight_state() == 1)
        {
            if (selected_action == GO_TO_EXIT)
                reward += 2;
            if (selected_action == FLEE)
                reward += 1;
        }

        if(Player.instance.at_open_treasure_state() == 1)
        {
            if (selected_action == PICK_UP_ITEM)
                reward += 2;
        }

        if(Player.instance.detection_state() == 1)
        {
            if (selected_action == SKIP_TARGET)
                reward += 2;
        }

        if(Player.instance.at_exit_state() == 1)
        {
            if (selected_action == DROP_TREASURE)
                reward += 2;
            if (selected_action == GO_TO_TARGET)
                reward += 1;
        }

        if (Player.instance.time_state() == 0)
        {
            if (selected_action == GO_TO_TARGET)
                reward += 2;
        }
        else
        {
            if (selected_action == GO_TO_EXIT)
                reward += 1;
            if (selected_action == FLEE)
                reward += 2;
        }

        if(Player.instance.value_degradation_state() == 1)
        {
            if (selected_action == GO_TO_TARGET)
                reward += 2;
        }
        else
        {
            if (selected_action == SKIP_TARGET)
                reward += 2;    
        }

        if(Player.instance.visited_all_state() == 1)
        {
            if (selected_action == FLEE)
                reward += 2;
        }

        return reward;
    }

    int getAction(int currentstate)
    {
        double maxVal = double.MinValue;
        int final_action = 0;

        for (int i = 0; i < POSSIBLE_MOVES; i++)
        {
            double qaction = Q[currentstate,i];
            if (qaction > maxVal)
            {
                maxVal = qaction;
                final_action = i;
            }
        }

        return final_action;
    }

    double getMaxQ(int state)
    {
        double maxVal = double.MinValue;

        for (int i = 0; i < POSSIBLE_MOVES; i++)
        {
            double qvalue = Q[state, i];
            if (qvalue > maxVal)
            {
                maxVal = qvalue;
            }
        }

        return maxVal;
    }

    public void EnvReset()
    {
        createModel(Q);
        transform.position = start_pos;
        busy = false;
        Done = false;
        Player.instance.player_reset();
        Timer.instance.timer_reset();
        Astar.instance.reset();
    }

    void Tasks(int selected_action)
    {
        // go to selected target and collect gold
        if (selected_action == GO_TO_TARGET)
        {
            if (target != null)
            {
                Player.instance.next_treasure = target.gameObject;
                Grid.instance.final_path = Astar.instance.pathfinder(agent.transform.position, target.position);
                move.instance.autoMove(Grid.instance.final_path);
            }
            else
                busy = false;
        }

        // go to exit
        if (selected_action == GO_TO_EXIT)
        {
            target = Astar.instance.exit.transform;
            Grid.instance.final_path = Astar.instance.pathfinder(agent.transform.position, target.position);
            move.instance.autoMove(Grid.instance.final_path);

            if(Vector3.Distance(agent.transform.position, target.position) <= 1)
                busy = false;
        }

        // pick items with smart exchange behavior
        if (selected_action == PICK_UP_ITEM)
        {
            Astar.instance.exchange_loot(); 
        }

        // drops treasure
        if(selected_action == DROP_TREASURE)
        {
            Player.instance.drop_treasure_at_exit();
        }

        // skip target
        if (selected_action == SKIP_TARGET)
        {
            Astar.instance.skip_target();
        }

        if(selected_action == FLEE)
        {
            target = Astar.instance.exit.transform;
            Grid.instance.final_path = Astar.instance.pathfinder(agent.transform.position, target.position);
            move.instance.autoMove(Grid.instance.final_path);

            if(Vector3.Distance(agent.transform.position, target.position) <= 1)
                Done = true;
        }
    }


    void createModel(double[,] Q)
    {
        StringBuilder strb = new StringBuilder();

        for (int i = 0; i < STATES;i++)
        {
            for (int j = 0; j < POSSIBLE_MOVES;j++)
            {
                strb.Append(Q[i, j]+"\n");
            }
        }

        File.WriteAllText(path,strb.ToString());
    }

}
