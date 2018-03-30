﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public class Astar : MonoBehaviour {

    public static Astar instance;
    public Transform start;

    Transform target;
    Grid grid;
    public GameObject[] targets;
    public GameObject exit;
    List<GameObject> trgts;
    public float targetradius = 2f;
    public GameObject[] coins;

    public int curr_weight;
    public int CAPACITY = 10;
    public int WEIGTH_VALUE = 10;
    public LayerMask obs;

    public List<Node> intpath;

	void Start() {
		instance = this;
		curr_weight = 0;
	}

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
        if (exit != null && exit.transform.childCount > 0)
        {
            //if (curr_weight < CAPACITY)
            //{
                target = targetSelector();
            //}
            //else
            //{
            //    target = exit.transform;
            //}
            grid.final_path = pathfinder(start.position, target.position);
        }
        else
            Application.Quit();
    }


    public Transform targetSelector()
    {
        float mindistance = int.MaxValue;
        List<Transform> visited = new List<Transform>();
        GameObject toberemoved = null;
        GameObject coin=null;

		if (trgts.Count == 0) 
		{
			trgts.Add (exit);
		}


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
            //System.Threading.Thread.Sleep(1000);
			      visited.Add (target);
			      trgts.Remove (toberemoved);
			      coin = toberemoved.transform.GetChild (0).gameObject;
			      Destroy (coin);
			      mindistance = int.MaxValue;
			      curr_weight += WEIGTH_VALUE;
        }
        return target;
    }

    public List<Node> pathfinder(Vector3 start_pos, Vector3 target_pos)
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
                return Reconstruct_path(startnode, targetnode);
            }

            List<Node> neighbors = new List<Node>();
            neighbors = grid.getNeighbors(currentnode);

            foreach (Node n in neighbors)
            {
				if (closed_set.Contains (n) || n.isobstacle) 
				{
					continue;
				}

                float newPathCost = currentnode.gscore + distance(currentnode, n);

                if (!open_set.Contains(n) || newPathCost < currentnode.gscore)
                {
                    n.gscore = newPathCost;
                    n.hscore = distance(n, targetnode);
                    n.parent = currentnode;

					if (!open_set.Contains (n)) 
					{
						open_set.Add (n);
					}
                }
            }
        }

        Debug.Log("No Path");
        return null;
    }

    float distance(Node node1, Node node2)
    {
        return Vector3.Distance(node1.position, node2.position);
    }

    List<Node> Reconstruct_path(Node start, Node target)
    {
            List<Node> path = new List<Node>();
            Node current = target;

            while (current != start)
            {
                path.Add(current);
                current = current.parent;
            }
            path.Reverse();
            return smooth_path(path);
    }

    List<Node> smooth_path(List<Node> path)
    {
        List<Node> smooth_path = new List<Node>();
  
        Node start = path[0];
        smooth_path.Add(start);
        path.RemoveAt(0);
        path.Reverse();
        
        RaycastHit info;

        while (path.Count > 0)
        {
            Node last = smooth_path[smooth_path.Count - 1];
            Node next = path[path.Count - 1];
          

            foreach (Node node in path)
            {
                Ray ray = new Ray(last.position, (node.position - last.position).normalized);
                
                if (Physics.SphereCast(ray, 0.5f, out info, Vector3.Distance(node.position, last.position), obs))
                    continue;

                next = node;
                break;
            }

            smooth_path.Add(next);
            path.RemoveRange(path.IndexOf(next), path.Count - path.IndexOf(next));
        }

       return smooth_path;
    }

}
