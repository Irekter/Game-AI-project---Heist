using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class QLearning : MonoBehaviour {

    Transform target;
    NavMeshAgent agent;
    GameObject[] targets;
    int action;
    double[,] Q = new double[6,POSSIBLE_MOVES];
    double[,] R = { {2,-1, 1}
                   ,{2, -1, 1}
                   ,{2,-1, 1}
                   ,{2,-1, 1}
                   ,{2,0, 1}
                   ,{-1,2, 1}};
    public static QLearning instance;
    

    // ACTIONS:
    const int GO_TO_TARGET = 0;
    const int GO_TO_EXIT = 1;
    const int PICK_UP_ITEM = 2;
    const int POSSIBLE_MOVES = 3;
    Vector3 start_pos;

    void Awake()
    {
        instance = this;
        agent = GetComponent<NavMeshAgent>();
    }


    // Use this for initialization
    void Start() {
        targets = Astar.instance.targets;
        action = 0;
        start_pos = gameObject.transform.position;
    }

    Random rd = new Random();
    public bool busy;

    // Update is called once per frame
    void Update() {
        if (Player.instance.get_gold_weight_at_exit() == 32)
        {
            EnvReset();
        }

        // selects action using q-learning
        if (!busy)
        {
            action = move_decider();
            busy = true;
            // executes the actions
            Tasks(action);
        }

        if(action != 2)
        {
            Tasks(action);

        }


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
        
        int rnd = Random.Range(0,POSSIBLE_MOVES);
     
        int currentstate = Player.instance.weight_state();

        if (rnd < epsilon)
            action = Random.Range(0, POSSIBLE_MOVES);
        else
            action = getAction(currentstate);
        Debug.Log("action: " + action);

        int nextstate = getNextState();

        double reward = R[currentstate,action];

        double currentQ = Q[currentstate, action];
        double maxQ = getMaxQ(nextstate);

        double q = (1 - alpha) * currentQ + alpha * (reward + gamma * maxQ);

        Q[currentstate, action] = q;

        epsilon = 0.9 * epsilon;
        //Debug.Log(epsilon);
        return action;
    }
    

    int getNextState()
    {
        int current_weight_state = Player.instance.weight_state();
        if (current_weight_state > 4)
        {
            return 5;
        }
        return (current_weight_state + 1);
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
        transform.position = start_pos;
        Player.instance.player_reset();
        Timer.instance.timer_reset();
        Astar.instance.reset();
    }

    void Tasks(int action)
    {
        // go to selected target and collect gold
        if (action == GO_TO_TARGET)
        {
            target = Astar.instance.targetSelector();
            if (target != null)
            {
                Grid.instance.final_path = Astar.instance.pathfinder(agent.transform.position, target.position);
                move.instance.autoMove(Grid.instance.final_path);
            }
            else
                busy = false;
        }

        // go to exit
        if (action == GO_TO_EXIT)
        {
            target = Astar.instance.exit.transform;
            Grid.instance.final_path = Astar.instance.pathfinder(agent.transform.position, target.position);
            move.instance.autoMove(Grid.instance.final_path);
            Player.instance.drop_treasure_at_exit();
        }

        // drop treasure
        if (action == PICK_UP_ITEM)
        {
            Astar.instance.exchange_loot(); 
        }
    }
}
