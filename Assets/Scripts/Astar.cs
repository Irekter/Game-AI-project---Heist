using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar : MonoBehaviour {

    public Transform start;
    Transform target;
    Grid grid;
    public GameObject[] targets;
    public GameObject exit;
    List<GameObject> trgts;
    public float targetradius=2f;
    public GameObject[] coins;

    private void Awake()
    {
        targets = GameObject.FindGameObjectsWithTag("End");
        exit = GameObject.FindGameObjectWithTag("Exit");
        coins = GameObject.FindGameObjectsWithTag("Coin");
        trgts = targets.ToList();
        grid = GetComponent<Grid>();
        
    }

    private void Update()
    {
        target = targetSelector();
        pathfinder(start.position, target.position);
    }

    public Transform targetSelector()
    {
        float mindistance = int.MaxValue;
        List<Transform> visited = new List<Transform>();
        GameObject toberemoved = null;

        // FIX THIS ASAP
        if (trgts.Count == 0 && exit != null)
            trgts.Add(exit);
        else
            Application.Quit();

        foreach (GameObject end in trgts)
        {
            if (Vector3.Distance(end.transform.position, start.position) < mindistance)
            {
                target = end.transform;
                toberemoved = end;
                mindistance = Vector3.Distance(target.position, start.position);
            }
        }

        if (Vector3.Distance(target.position, start.position) <= targetradius)
        {
            System.Threading.Thread.Sleep(1000);
            visited.Add(target);
            trgts.Remove(toberemoved);
            Destroy(toberemoved);
            mindistance = int.MaxValue;
        }

        return target;
    }

    public void pathfinder(Vector3 start_pos, Vector3 target_pos)
    {
        Node startnode = grid.worldToNode(start_pos);
        Node targetnode = grid.worldToNode(target_pos);
        PriorityQueue<Node> open_set = new PriorityQueue<Node>();

        startnode.hscore = distance(startnode, targetnode);
        startnode.gscore = 0;

        List<Node> closed_set = new List<Node>();
        open_set.Add(startnode);

        while (open_set.Count() != 0)
        {

            Node currentnode = open_set.Poll();
            closed_set.Add(currentnode);

            if (currentnode == targetnode)
            {
                Reconstruct_path(startnode, targetnode);
                return;
            }

            List<Node> neighbors = new List<Node>();
            neighbors = grid.getNeighbors(currentnode);

            foreach (Node n in neighbors)
            {
                if (closed_set.Contains(n) || n.isobstacle)
                    continue;

                float newPathCost = currentnode.gscore + distance(currentnode, n);

                if (!open_set.Contains(n) || newPathCost < currentnode.gscore)
                {
                    n.gscore = newPathCost;
                    n.hscore = distance(n, targetnode);
                    n.parent = currentnode;

                    if (!open_set.Contains(n))
                        open_set.Add(n);
                }
            }
        }
    }

    float distance(Node node1, Node node2)
    {
        return Vector3.Distance(node1.position, node2.position);
    }

    void Reconstruct_path(Node start, Node target)
    {
            List<Node> path = new List<Node>();
            Node current = target;

            while (current != start)
            {
                path.Add(current);
                current = current.parent;
            }
            path.Reverse();
            grid.final_path = path;
    }
}
