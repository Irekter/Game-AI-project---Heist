using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : IComparable <Treasure>
{
	public Vector3 position;
	public int item_weight;
	public int item_value;   
    public float item_worth;
	public bool is_visted;
	public bool is_alarmed;
	public bool is_secured;
	public bool is_empty;
	public bool is_opened;
	
    public Treasure(Vector3 _pos, int _value_item, int _weight, bool _alarmed, bool _secured, bool _filled)
    {
 		this.position = _pos;
		this.item_weight = _weight;
	    this.item_value = _value_item;   
		if(_value_item != 0) {
			this.item_worth = _weight / _value_item;
		} 
		else {
			this.item_worth = 0;
		}
		this.is_visted = false;
		this.is_alarmed = _alarmed;
		this.is_secured = _secured;
		this.is_empty = !_filled;
		this.is_opened = false;
    }
	
	void rob_treasure()
	{
		this.item_value = 0;
		this.item_weight = 0;
		this.item_worth = 0;
		this.is_visted = true;
		this.is_empty = true;
		this.is_opened = true;	
	}
	
    public int CompareTo(Treasure t)
    {
        if (this.item_worth < t.item_worth)
            return -1;
        else if (this.item_worth > t.item_worth)
            return 1;
        else
            return 0;
    }
}
