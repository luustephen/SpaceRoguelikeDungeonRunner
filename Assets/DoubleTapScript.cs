using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleTapScript : Item
{

    private GameObject player;
    private PlayerAttack playerAttackScript;
    private bool firstpass = true;

    // Start is called before the first frame update
    void Start()
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
        if (firstpass)
        {
            playerAttackScript.IncrementShots();
            firstpass = !firstpass;
        }
    }
}
