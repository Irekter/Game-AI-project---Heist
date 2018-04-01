using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : IComparable <Loot>
{
	public int item_weight;
	public int item_value;   
    public float worth;
	public bool is_visted;
	public bool is_alarmed;
	public bool is_secured;
	public bool is_empty;
	public bool is_opened;
	
    public Loot(int value_item, int weight, bool alarmed, bool secured, bool filled)
    {
        item_weight = weight;
	    item_value = value_item;   
		worth = item_value / item_weight;
		is_visted = false;
		is_alarmed = alarmed;
		is_secured = secured;
		is_empty = !filled;
		is_opened = false;
    }

    public int CompareTo(Loot l)
    {
        if (this.worth < l.worth)
            return -1;
        else if (this.worth > l.worth)
            return 1;
        else
            return 0;
    }
}
