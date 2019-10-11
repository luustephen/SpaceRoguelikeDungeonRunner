using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    private List<Item> itemInventory;

	// Use this for initialization
	void Start () {
        itemInventory = new List<Item>();
	}
	
	// Update is called once per frame
	void Update () {
        foreach(Item printitem in itemInventory)
        {
            print(printitem.name);
        }
	}

    public bool Add(Item addedItem)
    {
        if (itemInventory.Contains(addedItem))//Increase quantity of item if already held
        {
            itemInventory[itemInventory.IndexOf(addedItem)].quantity++;
        }
        else //or add it if it doesn't exist
            itemInventory.Add(addedItem);
        return true;
    }
}
