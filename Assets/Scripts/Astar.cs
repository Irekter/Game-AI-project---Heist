using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public class Astar : MonoBehaviour {

    const float TARGET_RADIUS = 1f;
    public static Astar instance;
    public Transform start;
    public Transform target;
    Grid grid;
    public GameObject[] targets;
    public GameObject exit;
    public List<GameObject> trgts;
    List<GameObject> opened_treasures;
    public GameObject[] coins;
    public LayerMask obs;
    public bool AutoMove;
    public List<Node> intpath;

	void Start() 
	{
		instance = this;
        opened_treasures = new List<GameObject>();
	}

	private void Awake()
    {
        reset();
        grid = GetComponent<Grid>();
    }


    private void Update()
    {
        open_treasure();
    }

    public Transform targetSelector()
    {
        float mindistance = int.MaxValue;
        target = null;

        foreach (GameObject possible_trgt in trgts)
        {
            float currentDistance = Vector3.Distance(possible_trgt.transform.position, start.position);
            if (currentDistance < mindistance)
            {
                target = possible_trgt.transform;
                mindistance = currentDistance;
            }
        }
        if(target!=null)
            Player.instance.set_current_treasure(target.gameObject);
        return target;
    }


    public void open_treasure()
    {
        GameObject toberemoved = null;

        if (target != null)
        {
            if (Vector3.Distance(target.position, start.position) <= TARGET_RADIUS)
            {
                toberemoved = target.gameObject;

                trgts.Remove(toberemoved);

                // looting the reached target
                if (target.gameObject != exit)
                {
                    // returns loot attributes and sets player breaking time and treasure breaking time set to zero
                    target.GetComponent<Treasure>().open_treasure();
                    QLearning.instance.busy = false;
                }
            }
        }
    }

    public void pick_up_loot()
    {
        if (Vector3.Distance(start.position, target.position) <= 1)
        {
            // after opening check if we can take the treasure
            if (Player.instance.pick_new_loot())
            {
                // performs looting
                target.GetComponent<Treasure>().empty_treasure();
            }
            else
            {
                // can't loot now so we keep track to loot in next iteration
                GameObject toberemoved = Player.instance.get_current_treasure();
                if (toberemoved.GetComponent<Treasure>().gold_weight <= Player.instance.CAPACITY)
                {
                    // tracks treasures that were opened but not looted yet
                    opened_treasures.Add(toberemoved);
                }
            }
        }
    }

    public void exchange_loot()
    {
        GameObject toberemoved = Player.instance.get_current_treasure();
        if (toberemoved != null)
        {
            // after opening check if we can take the treasure
            if (Player.instance.exchange_loot())
            {
                // performs looting
                toberemoved.GetComponent<Treasure>().empty_treasure();
            }
            else
            {
                Player.instance.remove_current_treasure();
                // can't loot now so we keep track to loot in next iteration
                if (toberemoved.GetComponent<Treasure>().gold_weight <= Player.instance.CAPACITY)
                {
                    // tracks treasures that were opened but not looted yet
                    opened_treasures.Add(toberemoved);
                }
            }
        }
        else
            QLearning.instance.busy = false;
    }


    public void skip_target()
    {
        if(trgts.Count()>0)
            trgts.Remove(target.gameObject);
        QLearning.instance.busy = false;
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
		return null;
    }

    float distance(Node node1, Node node2)
    {
        return Vector3.Distance(node1.position, node2.position);
    }

    List<Node> Reconstruct_path(Node start_pos, Node target_pos)
    {
        List<Node> path = new List<Node>();
        Node current = target_pos;

        while (current != start_pos)
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

    public void reset()
    {
        targets = GameObject.FindGameObjectsWithTag("End");
        exit = GameObject.FindGameObjectWithTag("Exit");
        coins = GameObject.FindGameObjectsWithTag("Coin");
        trgts = targets.ToList();

        // resets all the treasures 
        foreach (GameObject t in trgts)
        {
            t.GetComponent<Treasure>().treasure_reset();
        }
    }
}
