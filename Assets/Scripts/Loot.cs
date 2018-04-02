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
		if(_value_item != 0) {
			this.item_worth = _weight / _value_item;
		} 
		else {
			this.item_worth = 0;
		}
    }
	
    public int CompareTo(Loot l)
    {
        if (this.item_worth < l.item_worth)
            return -1;
        else if (this.item_worth > l.item_worth)
            return 1;
        else
            return 0;
    }
}
