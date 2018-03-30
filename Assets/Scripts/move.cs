using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class move : MonoBehaviour {

    public Transform player;
    private Animator anim;
    Grid grid;
    private NavMeshAgent agent;
    Astar astar;

    private void Awake()
    {
        anim = player.GetComponent<Animator>();
        grid = GetComponent<Grid>();
        agent = player.GetComponent<NavMeshAgent>();
        astar = player.GetComponent<Astar>();
    }



    void Update()
    {
        autoMove(grid.final_path);
    }

    void autoMove(List<Node> mover)
    {
        Debug.Log(mover.Count);
        if (mover.Count >= 2)
        {
            anim.SetInteger("moving", 1);
            agent.destination = mover[1].position;
            mover.RemoveAt(0);
        }
        //else
        //{
        //    Astar.instance.curr_weight = 0;
        //}
    }
}
