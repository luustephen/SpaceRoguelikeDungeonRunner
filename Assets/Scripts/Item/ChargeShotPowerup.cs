using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeShotPowerup : Item
{
    private ElementalEffects elementScript;
    private GameObject player;
    private PlayerAttack playerAttackScript;
    private bool firstpass = true;

    // Use this for initialization
    void Start() //Get player attack script
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerAttackScript = player.GetComponent<PlayerAttack>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void UseItem() //Set player projectile to have fire
    {
        if (playerAttackScript != null && firstpass)
        {
            playerAttackScript.SetChargeShot(true);
            firstpass = !firstpass;
        }
    }
}
