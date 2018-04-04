using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : IComparable <Loot>
{
	public int item_weight;
	public int item_value;   
    public float item_worth;
	
    public Loot(int _value_item, int _weight)
    {
 		this.item_weight = _weight;
	    this.item_value = _value_item;   
		if(_weight != 0) 
		{
			this.item_worth =  (float)_value_item / (float)_weight;
		} 
		else 
		{
			this.item_worth = 0;
		}
    }
	
    public int CompareTo(Loot l)
    {
        if (this.item_worth < l.item_worth)
		{
            return -1;
		}
		if (this.item_worth > l.item_worth)
		{
            return 1;
		}
		return 0;
    }
}
