using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour {

    private Inventory entityInventory = null;
    private Item pickedItem;

	// Use this for initialization
	void Start () {
        if ((entityInventory = gameObject.GetComponent<Inventory>())) //If there is no inventory, dont pick stuff up
            this.enabled = false;
           
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "PickupItem" && (pickedItem = collision.GetComponent<Item>()))
        {
            Destroy(collision.gameObject);
            entityInventory.Add(pickedItem);
        }
    }
}
