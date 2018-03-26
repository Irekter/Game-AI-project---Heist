using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class move : MonoBehaviour {

    public Transform player;
    private Animator anim;
    Grid grid;
    private NavMeshAgent agent;


    private void Awake()
    {
        anim = player.GetComponent<Animator>();
        grid = GetComponent<Grid>();
		agent = player.GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        autoMove();
    }

    void autoMove()
    {
		if (grid.final_path.Count >= 1) {
			anim.SetInteger ("moving", 1);
			Node currentnode = grid.final_path [0];
			grid.final_path.RemoveAt (0);
			agent.destination = currentnode.position;
		} else {
			Astar.instance.curr_weight = 0;
		}
    }

}
