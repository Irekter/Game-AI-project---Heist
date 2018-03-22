using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Astar : MonoBehaviour {

    public Transform start;
    public Transform target;
    Grid grid;
    public GameObject[] targets;
   

    private void Awake()
    {
        targets = GameObject.FindGameObjectsWithTag("End");
        grid = GetComponent<Grid>();
        //prique = GetComponent<PriorityQueue<Node>>();
    }

    private void Update()
    {
        float mindistance = int.MaxValue;
        List<GameObject> visited = new List<GameObject>();
        
        foreach(GameObject end in targets)
        {
            if (!visited.Contains(end))
            {
                if (Vector3.Distance(end.transform.position, start.position) < mindistance)
                {
                    target = end.transform;
                    mindistance = Vector3.Distance(target.position, start.position);
                }
            }
            if (start.position == end.transform.position)
                visited.Add(end);
        }
        pathfinder(start.position, target.position);
    }


    void pathfinder(Vector3 start_pos, Vector3 target_pos)
    {
        Node startnode = grid.worldToNode(start.position);
        Node targetnode = grid.worldToNode(target.position);
        PriorityQueue<Node> open_set = new PriorityQueue<Node>();
        //List<Node> open_set = new List<Node>();

        startnode.hscore = distance(startnode, targetnode);
        startnode.gscore = 0;
        //startnode.fscore = startnode.gscore + startnode.hscore;

        List<Node> closed_set = new List<Node>();

        open_set.Add(startnode);

        while (open_set.Count() != 0)
        {

            Node currentnode = open_set.Poll();

            //foreach (Node node in open_set)
            //{
            //    if (node.fscore <= currentnode.fscore && node.hscore < currentnode.hscore)
            //    {
            //        currentnode = node;
            //    }
            //}

            //open_set.Remove(currentnode);
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




                //Node currentnode = open_set.Poll();

                //if (currentnode.Equals(targetnode))
                //{
                //    Reconstruct_path(path, targetnode);
                //    return;
                //}



                //List<Node> neighbors = new List<Node>();
                //neighbors = grid.getNeighbors(currentnode);

                //float tentative_g = 0;

                //foreach (Node n in neighbors)
                //{
                //    if (closed_set.Contains(n))
                //        continue;

                //    tentative_g = n.gscore + distance(currentnode, n);

                //    if (!open_set.Contains(n) || tentative_g < n.gscore)
                //    {
                //        path[n] = currentnode;
                //        n.gscore = tentative_g;
                //        n.fscore = n.gscore + distance(n, targetnode);

                //        if (!open_set.Contains(n))
                //            open_set.Add(n);
                //    }
                //}
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



        //List<Node> total_path = new List<Node>();
        //total_path.Add(current);

        //while (path.ContainsKey(current))
        //{
        //    current = path[current];
        //    total_path.Add(current);
        //}

        //grid.final_path = total_path;
    }
}
