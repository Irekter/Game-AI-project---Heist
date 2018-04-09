using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class QLearning : MonoBehaviour {
    Transform target;
    NavMeshAgent agent;
    List<string> possible_moves = new List<string>();
   


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }


    // Use this for initialization
    void Start() {
        possible_moves.Add("gototarget");
        possible_moves.Add("gotoexit");
        possible_moves.Add("droptreasure");
    }

    Random rd = new Random();

    // Update is called once per frame
    void Update() {
        target = Astar.instance.targetSelector();
        //string action = possible_moves[Random.Range(0, 3)];


        // selects action using q-learning
        string action = move_decider(target, Vector3.Distance(transform.position, target.position));

        Debug.Log(action);

        // executes the actions
        Tasks(action);
    }


    string move_decider(Transform target, float distance_from_target)
    {
        string action = QLearner(target,distance_from_target);
        return action;
    }

    string QLearner(Transform target, float distance_from_target)
    {
        int POSSIBLE_MOVES = possible_moves.Count;
        int rnd = Random.Range(0,POSSIBLE_MOVES);

        return possible_moves[rnd];
    }


    void init()
    {

    }


    void Tasks(string action)
    {
        // go to selected target and collect gold
        if (action == "gototarget")
        {
            agent.SetDestination(target.position);
        }

        // go to exit
        if (action == "gotoexit")
        {
            agent.SetDestination(Astar.instance.exit.transform.position);
        }

        // drop treasure
        if (action == "droptreasure")
        {
            Player.instance.drop_treasure_at_exit();
        }
    }
}
