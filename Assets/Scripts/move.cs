using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class move : MonoBehaviour {

    public Transform player;
    Grid grid;
    private NavMeshAgent agent;
    Astar astar;

    private void Awake()
    {
        grid = GetComponent<Grid>();
	    agent = player.GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        autoMove(grid.final_path);
    }

    void autoMove(List<Node> mover)
    {
        if (mover.Count >= 2)
        {
            agent.destination = mover[1].position;
            mover.RemoveAt(0);
        }
    }
}
