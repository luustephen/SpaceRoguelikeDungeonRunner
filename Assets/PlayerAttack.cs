using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

    public KeyCode primaryAttack, secondaryAttack;
    private bool primaryOnCooldown, secondaryOnCooldown;
    public float primaryAttackCooldown, secondaryAttackCooldown;
    private Transform attackHitbox;
    private SpriteRenderer attackSprite;
    private BoxCollider attackCollider;

	// Use this for initialization
	void Start () {
        /* 
         * primaryAttack = KeyCode.Mouse0
         * secondaryAttack = KeyCode.Mouse1;
         */
        primaryOnCooldown = false;
        secondaryOnCooldown = false;
        primaryAttackCooldown = .2f;
        secondaryAttackCooldown = .3f;
        attackHitbox = gameObject.transform.GetChild(1);
        attackSprite = attackHitbox.gameObject.GetComponent<SpriteRenderer>();
        attackCollider  = attackHitbox.gameObject.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(primaryAttack))            //Start cooldown on primary attack
        {
            if (!primaryOnCooldown && !secondaryOnCooldown)
            {
                primaryOnCooldown = true;
                Vector3 mousePosition = Input.mousePosition;
                float angle = Mathf.Atan2(mousePosition.y - Screen.height / 2, mousePosition.x - Screen.width / 2);
                float y = Mathf.Sin(angle);
                float x = Mathf.Cos(angle);
                attackHitbox.Translate(new Vector3(x, y, 0));
                attackHitbox.Rotate(new Vector3(0 ,0, Mathf.Rad2Deg * angle + 90));
                attackSprite.enabled = true;
                attackCollider.enabled = true;
                StartCoroutine("PrimaryCooldown", angle);
            }
        }

        if (Input.GetKeyDown(secondaryAttack))          //Start cooldown on secondary attack
        {
            if (!secondaryOnCooldown && !primaryOnCooldown)
            {
                secondaryOnCooldown = true;
                StartCoroutine("SecondaryCooldown");
            }
        }
    }

    IEnumerator PrimaryCooldown(float angle)
    {
        yield return new WaitForSeconds(primaryAttackCooldown);
        float y = Mathf.Sin(angle);
        float x = Mathf.Cos(angle);
        attackSprite.enabled = false;
        attackCollider.enabled = false;
        attackHitbox.Rotate(new Vector3(0, 0, Mathf.Rad2Deg * -angle - 90));
        attackHitbox.Translate(new Vector3(-x, -y, 0));
        primaryOnCooldown = false;
    }

    IEnumerator SecondaryCooldown()
    {
        yield return new WaitForSeconds(secondaryAttackCooldown);
        secondaryOnCooldown = false;
    }
}
