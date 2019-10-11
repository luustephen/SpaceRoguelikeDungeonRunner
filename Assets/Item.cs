using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item : MonoBehaviour {

    public const int PASSIVE = 0;
    public const int ACTIVE = 1;
    public const int KNIFE = 0;
    public const int ARMCANNON = 1;

    public int quantity;
    public enum Part
    {
        KNIFE,
        ARMCANNON
    };
    public enum Type
    {
        PASSIVE,
        ACTIVE
    };
    public Type type;
    public Part part;
    public string name;


    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
