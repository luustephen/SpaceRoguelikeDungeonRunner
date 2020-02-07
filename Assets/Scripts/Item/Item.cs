using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item : MonoBehaviour {

    /*ELEMENTAL TLDR
 * Fire Water Lightning
 * FWL
 * 000 = 0 = None
 * 100 = 4 = Fire
 * 010 = 2 = Water
 * 001 = 1 = Lightning
 * 110 = 6 = Steam
 * 101 = 5 = Fiery Lightning
 * 011 = 3 = Water Lightning
 * 111 = 7 = Storm?
 * 
 * 
 * */

    public const int ELEMENTLESS = 0;
    public const int FIRE = 4;
    public const int WATER = 2;
    public const int LIGHTNING = 1;
    public const int FIREWATER = 6;
    public const int WATERLIGHTNING = 3;
    public const int FIRELIGHTNING = 5;
    public const int FIREWATERLIGHTNING = 7;

    public const int PASSIVE = 0;
    public const int ACTIVE = 1;
    public const int WEAPON = 0;
    public const int ARMCANNON = 1;

    public enum Part
    {
        WEAPON,
        ARMCANNON
    };
    public enum Type
    {
        PASSIVE,
        ACTIVE
    };
    public Type type;
    public Part part;
    public int quantity = 1;


    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void UseItem()
    {

    }

}
