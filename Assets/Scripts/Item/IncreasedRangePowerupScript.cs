using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreasedRangePowerupScript : Item
{
    private ElementalEffects elementScript;
    private GameObject playerWeapon;
    private BoxCollider2D playerWeaponHitbox;
    private GameObject player;
    public float sizeIncrease = 1.3f;
    private bool firstpass = true;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        foreach (Transform child in player.transform)
        {
            if (child.tag == "Player Attack")
            {
                playerWeapon = child.gameObject;
                playerWeaponHitbox = playerWeapon.GetComponent<BoxCollider2D>();
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void UseItem()
    {
        if (playerWeapon != null && firstpass)
        {
            playerWeapon.transform.localScale = playerWeapon.transform.localScale * sizeIncrease;
            playerWeaponHitbox.size = playerWeaponHitbox.size * sizeIncrease;
            firstpass = !firstpass;
        }
    }
}
