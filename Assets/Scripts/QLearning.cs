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
    double[,] Q = new double[6,2];
    double[,] R = { {2,-1 },{2, -1},{1,-1},{-1,2 },{-1,2 },{-1,2} };


    // ACTIONS:
    const int GO_TO_TARGET = 0;
    const int GO_TO_EXIT = 1;
    const int POSSIBLE_MOVES = 2;

    Vector3 start_pos;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }


    // Use this for initialization
    void Start() {
        targets = Astar.instance.targets;
        action = 0;
        start_pos = gameObject.transform.position;
    }

    Random rd = new Random();

    // Update is called once per frame
    void Update() {
        if (Player.instance.gold_weight_till_now == 35)
        {
            EnvReset();
        }
        target = Astar.instance.targetSelector();
       
        // selects action using q-learning
        int action = move_decider(Player.instance.percent_full());

        // executes the actions
        Tasks(action);
    }


    int move_decider(int percent)
    {
        //action = QLearner(percent);
        return GO_TO_TARGET;
    }

    // HYPERPARAMETERS:
    double gamma = 0.3;
    double epsilon = 0.2;
    double alpha = 0.2;

    int QLearner(int percent)
    {
        
        int rnd = Random.Range(0,POSSIBLE_MOVES);
       
        int currentstate = define_state(percent);

        if (rnd < epsilon)
            action = Random.Range(0, POSSIBLE_MOVES);
        else
            action = getAction(currentstate);

        int nextstate = getNextState();

        double reward = R[currentstate,action];

        double currentQ = Q[currentstate, action];
        double maxQ = getMaxQ(nextstate);

        double q = (1 - alpha) * currentQ + alpha * (reward + gamma * maxQ);

        Q[currentstate, action] = q;

        return action;
    }
    

    int getNextState()
    {
        int percent = Player.instance.get_player_gold_weight() + Player.instance.last_loot.loot_weight;
        percent = (100 * percent)/ Player.instance.CAPACITY;

        return define_state(percent);
    }


    int define_state(int percent)
    {
        int plyr_state = (percent*5)/100;

        if (plyr_state > 5)
            plyr_state = 5;

        return plyr_state;
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
    }

    void Tasks(int action)
    {
        // go to selected target and collect gold
        if (action == GO_TO_TARGET)
        {
            agent.SetDestination(target.position);
        }

        // go to exit
        if (action == GO_TO_EXIT)
        {
            agent.SetDestination(Astar.instance.exit.transform.position);
        }

        //// drop treasure
        //if (action == "droptreasure")
        //{
        //    Player.instance.drop_treasure_at_exit();
        //}
    }
}
