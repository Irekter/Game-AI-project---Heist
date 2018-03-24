using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMove : MonoBehaviour {

    Grid grid;
    Astar astar;
    public Transform player;
    public Transform exit;
    Node target;
    public float speed = 1f;
    public int endxnodeearly = 2;

    // Use this for initialization
    void Awake()
    {
        grid = GetComponent<Grid>();
    }


    // Update is called once per frame
    void Update()
    {
        automatedMovement();
    }

    void automatedMovement()
    {
        if (grid.final_path.Count >= endxnodeearly)
        {
            Node currentnode = grid.final_path[0];
            grid.final_path.RemoveAt(0);

            player.position = Vector3.MoveTowards(player.position, currentnode.position, speed);
        }
    }
}
