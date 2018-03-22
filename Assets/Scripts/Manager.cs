using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

    Grid grid;
    public Transform player;
    Node target;

    // Use this for initialization
    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    // Update is called once per frame
    void Update()
    {
        if (grid.final_path.Count >= 1)
        {
            Node currentnode = grid.final_path[0];
            grid.final_path.RemoveAt(0);

            player.position = Vector3.MoveTowards(player.position, currentnode.position , 0.1f);
        }
    }


}
