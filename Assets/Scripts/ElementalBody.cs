using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalBody : MonoBehaviour {

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

    private ElementalEffects elementScript;
    private ElementalEffects playerElementScript;
    private GameObject player;
    private PlayerHealth playerHealth;
    private float burnTick; //Speed at which burn affects health
    private bool isBurning = false;

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        elementScript = gameObject.GetComponent<ElementalEffects>();
        playerElementScript = transform.parent.GetComponent<ElementalEffects>(); //Pick up the element of the player
    }
	
	// Update is called once per frame
	void Update () {
        if (elementScript && playerElementScript)
        {
            elementScript.element = playerElementScript.element;
        }
        if((elementScript.element & FIRE) == FIRE && !isBurning)
        {
            isBurning = true;
            StartCoroutine(Burn(3,5));
        }
    }

    IEnumerator Burn(float time, int tickDmg)                //Slowly take health from player until burn subsides
    {
        if (time > 0)
        {
            while (time-- > 0)
            {
                playerHealth.currentHealth = playerHealth.currentHealth - tickDmg;
                print(playerHealth.currentHealth);
                yield return new WaitForSeconds(1);
            }
        }
        isBurning = false;
        elementScript.element = elementScript.element & 3;
        playerElementScript.element = playerElementScript.element & 3;
    }
}
